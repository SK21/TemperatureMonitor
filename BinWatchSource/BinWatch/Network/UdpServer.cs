using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinWatch.Network
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; }
        public IPEndPoint RemoteEndPoint { get; }

        public PacketReceivedEventArgs(byte[] data, IPEndPoint remoteEndPoint)
        {
            Data = data;
            RemoteEndPoint = remoteEndPoint;
        }
    }

    public class UdpServer : IDisposable
    {
        public const int DefaultPort = 1600;
        public const int HeartbeatIntervalMs = 30000;

        // Command bits for PGN 30821
        public const byte CmdSendTemps = 0x01;
        public const byte CmdSendModuleDescription = 0x02;
        public const byte CmdUpdateAndSendTemps = 0x04;  // read fresh temps then send

        public event EventHandler<PacketReceivedEventArgs> PacketReceived;
        public event EventHandler<string> Error;

        public bool IsRunning { get; private set; }
        public int RawPacketsReceived { get; private set; }
        public int FilteredPacketsReceived { get; private set; }

        private UdpClient _client;
        private CancellationTokenSource _cts;
        private Timer _heartbeatTimer;
        private readonly int _port;
        private readonly IPEndPoint _broadcastEndPoint;
        private bool _disposed;

        public UdpServer(int port = DefaultPort)
        {
            _port = port;
            _broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, port);
        }

        public void Start()
        {
            if (IsRunning) return;

            _client = new UdpClient();
            _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _client.Client.Bind(new IPEndPoint(IPAddress.Any, _port));
            _client.EnableBroadcast = true;

            _cts = new CancellationTokenSource();
            Task.Run(() => ReceiveLoop(_cts.Token));

            _heartbeatTimer = new Timer(_ => SendHeartbeat(), null, 0, HeartbeatIntervalMs);

            IsRunning = true;
        }

        public void Stop()
        {
            if (!IsRunning) return;

            _heartbeatTimer?.Dispose();
            _heartbeatTimer = null;

            _cts?.Cancel();
            _client?.Close();  // unblocks ReceiveAsync

            IsRunning = false;
        }

        // PGN 30820 - Heartbeat (broadcast to every local subnet)
        public void SendHeartbeat()
        {
            byte[] data = new byte[3];
            data[0] = 100;
            data[1] = 120;
            data[2] = Crc(data, 2);
            SendToAllSubnets(data);
        }

        private void SendToAllSubnets(byte[] data)
        {
            bool sent = false;
            var sentBroadcasts = new HashSet<IPAddress>();

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {

                if (ni.OperationalStatus != OperationalStatus.Up) continue;
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel) continue;

                foreach (var ua in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ua.Address.AddressFamily != AddressFamily.InterNetwork) continue;
                    if (ua.IPv4Mask == null) continue;

                    byte[] addr = ua.Address.GetAddressBytes();
                    byte[] mask = ua.IPv4Mask.GetAddressBytes();

                    if (addr[0] == 169 && addr[1] == 254) continue;  // skip APIPA

                    byte[] bcast = new byte[4];
                    for (int i = 0; i < 4; i++) bcast[i] = (byte)(addr[i] | ~mask[i]);

                    IPAddress broadcast = new IPAddress(bcast);
                    if (!sentBroadcasts.Add(broadcast)) continue;  // skip duplicates

                    SendPacket(data, new IPEndPoint(broadcast, _port));
                    sent = true;
                }
            }
            if (!sent) SendPacket(data);  // fallback to 255.255.255.255
        }

        // PGN 30821 - Command
        public void SendCommand(byte moduleId, byte commandBits, string targetIp = null)
        {
            byte[] data = new byte[5];
            data[0] = 101;
            data[1] = 120;
            data[2] = moduleId;
            data[3] = commandBits;
            data[4] = Crc(data, 4);

            if (targetIp != null && System.Net.IPAddress.TryParse(targetIp, out var ip))
            {
                SendPacket(data, new IPEndPoint(ip, _port));
                return;
            }

            SendToAllSubnets(data);
        }

        // PGN 30822 - Set Module Description
        // targetIp: send directly to the module's IP rather than broadcasting (preferred)
        public void SendSetModuleDescription(byte[] mac, byte newId, string name, string targetIp = null)
        {
            if (mac == null || mac.Length != 6)
                throw new ArgumentException("MAC address must be 6 bytes.", nameof(mac));

            byte[] data = new byte[20];
            data[0] = 102;
            data[1] = 120;
            Array.Copy(mac, 0, data, 2, 6);
            data[8] = newId;

            byte[] nameBytes = new byte[10];
            byte[] encoded = Encoding.UTF8.GetBytes(name ?? string.Empty);
            Array.Copy(encoded, nameBytes, Math.Min(encoded.Length, 10));
            Array.Copy(nameBytes, 0, data, 9, 10);

            data[19] = Crc(data, 19);

            IPEndPoint endpoint = null;
            if (targetIp != null && System.Net.IPAddress.TryParse(targetIp, out var ip))
                endpoint = new IPEndPoint(ip, _port);

            SendPacket(data, endpoint);
        }

        // PGN 30823 - Set Sensor User Data
        public void SendSetSensorUserData(byte moduleId, byte[] romCode, byte userData0, byte userData1)
        {
            if (romCode == null || romCode.Length != 8)
                throw new ArgumentException("ROM code must be 8 bytes.", nameof(romCode));

            byte[] data = new byte[14];
            data[0] = 103;
            data[1] = 120;
            data[2] = moduleId;
            Array.Copy(romCode, 0, data, 3, 8);
            data[11] = userData0;
            data[12] = userData1;
            data[13] = Crc(data, 13);
            SendToAllSubnets(data);
        }

        private void SendPacket(byte[] data, IPEndPoint endpoint = null)
        {
            try
            {
                _client?.Send(data, data.Length, endpoint ?? _broadcastEndPoint);
            }
            catch (ObjectDisposedException) { }  // expected during shutdown
            catch (SocketException ex) { Logger.Warning($"UDP send failed: {ex.Message}"); }
        }

        private async Task ReceiveLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    UdpReceiveResult result = await _client.ReceiveAsync();
                    RawPacketsReceived++;

                    // Ignore packets we sent ourselves
                    if (IsLocalAddress(result.RemoteEndPoint.Address)) continue;

                    FilteredPacketsReceived++;
                    if (result.Buffer.Length > 2)
                        PacketReceived?.Invoke(this, new PacketReceivedEventArgs(result.Buffer, result.RemoteEndPoint));
                }
                catch (ObjectDisposedException) { break; }  // expected during shutdown
                catch (SocketException) { break; }
                catch (Exception ex)
                {
                    Logger.Error("UDP receive error", ex);
                    Error?.Invoke(this, ex.Message);
                }
            }
        }

        private static bool IsLocalAddress(IPAddress address)
        {
            if (IPAddress.IsLoopback(address)) return true;
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
                if (ip.Equals(address)) return true;
            return false;
        }

        private static byte Crc(byte[] data, int length)
        {
            byte result = 0;
            for (int i = 0; i < length; i++)
                result += data[i];
            return result;
        }

        public void Dispose()
        {
            if (_disposed) return;
            Stop();
            _cts?.Dispose();
            _client?.Dispose();
            _disposed = true;
        }
    }
}
