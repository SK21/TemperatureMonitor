using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TempMonitor.Forms;

namespace TempMonitor.Classes
{
    public class UDPcomm
    {
        private readonly string cDestinationIP;
        private readonly bool cUseLoopback;
        private readonly frmMain mf;
        private byte[] buffer = new byte[1024];
        private IPAddress cEthernetEP;
        private bool cIsUDPSendConnected;
        private int cReceivePort;   // local ports must be unique for each app on same pc and each class instance

        private int cSendFromPort;
        private int cSendToPort;
        private IPAddress cWiFiEP;  // wifi endpoint address
        private string cWiFiIP;

        private HandleDataDelegateObj HandleDataDelegate = null;
        private int PGN;
        private Socket recvSocket;
        private Socket sendSocket;

        // local wifi ip address

        public UDPcomm(frmMain CallingForm, int ReceivePort, int SendToPort
            , int SendFromPort, string DestinationIP = "", bool UseLoopBack = false)
        {
            mf = CallingForm;
            cReceivePort = ReceivePort;
            cSendToPort = SendToPort;
            cSendFromPort = SendFromPort;
            cUseLoopback = UseLoopBack;
            cDestinationIP = DestinationIP;

            SetEndPoints();
        }

        private delegate void HandleDataDelegateObj(int port, byte[] msg);  // Status delegate

        public string EthernetEP
        {
            get { return cEthernetEP.ToString(); }
            set
            {
                IPAddress IP;
                string[] data;

                if (IPAddress.TryParse(value, out IP))
                {
                    data = value.Split('.');
                    cEthernetEP = IPAddress.Parse(data[0] + "." + data[1] + "." + data[2] + ".255");
                    mf.Tls.SaveProperty("EthernetEP", value);
                }
            }
        }

        public bool IsUDPSendConnected { get => cIsUDPSendConnected; set => cIsUDPSendConnected = value; }

        public string WifiEP
        {
            get { return cWiFiEP.ToString(); }
            set
            {
                IPAddress IP;
                string[] data;

                if (IPAddress.TryParse(value, out IP))
                {
                    data = value.Split('.');
                    cWiFiEP = IPAddress.Parse(data[0] + "." + data[1] + "." + data[2] + ".255");
                    cWiFiIP = value;
                    mf.Tls.SaveProperty("WifiIP", value);
                }
            }
        }

        public void Close()
        {
            recvSocket.Close();
            sendSocket.Close();
        }

        public string EthernetIP()
        {
            string Adr;
            IPAddress IP;
            string Result;

            Adr = GetLocalIPv4(NetworkInterfaceType.Ethernet);
            if (IPAddress.TryParse(Adr, out IP))
            {
                Result = IP.ToString();
            }
            else
            {
                Result = "127.0.0.1";
            }
            return Result;
        }

        public void SendUDPMessage(byte[] byteData)
        {
            //sends byte array
            if (IsUDPSendConnected)
            {
                try
                {
                    if (byteData.Length != 0)
                    {
                        // ethernet
                        IPEndPoint EndPt = new IPEndPoint(cEthernetEP, cSendToPort);
                        sendSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, EndPt, new AsyncCallback(SendData), null);

                        if (!cUseLoopback)
                        {
                            // wifi
                            EndPt = new IPEndPoint(cWiFiEP, cSendToPort);
                            sendSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, EndPt, new AsyncCallback(SendData), null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    mf.Tls.WriteErrorLog("UDPcomm/SendUDPMessage " + ex.Message);
                }
            }
        }

        public void StartUDPServer()
        {
            try
            {
                // initialize the delegate which updates the message received
                HandleDataDelegate = HandleData;

                // initialize the receive socket
                recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                recvSocket.Bind(new IPEndPoint(IPAddress.Any, cReceivePort));

                // initialize the send socket
                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Initialise the IPEndPoint for the server to send on port
                IPEndPoint server = new IPEndPoint(IPAddress.Any, cSendFromPort);
                sendSocket.Bind(server);

                // Initialise the IPEndPoint for the client - async listner client only!
                EndPoint client = new IPEndPoint(IPAddress.Any, 0);

                // Start listening for incoming data
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref client, new AsyncCallback(ReceiveData), recvSocket);
                IsUDPSendConnected = true;
            }
            catch (Exception e)
            {
                mf.Tls.WriteErrorLog("StartUDPServer: \n" + e.Message);
            }
        }

        private string GetLocalIPv4(NetworkInterfaceType _type)
        {
            // https://stackoverflow.com/questions/6803073/get-local-ip-address

            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }

        private void HandleData(int Port, byte[] Data)
        {
            try
            {
                if (Data.Length > 1)
                {
                    PGN = Data[1] << 8 | Data[0];
                    switch (PGN)
                    {
                        //case 32400:
                        //    foreach (clsProduct Prod in mf.Products.Items)
                        //    {
                        //        Prod.UDPcommFromArduino(Data, PGN);
                        //    }
                        //    break;

                        //case 32401:
                        //    mf.AnalogData.ParseByteData(Data);
                        //    break;

                        //case 32618:
                        //    mf.SwitchBox.ParseByteData(Data);
                        //    break;
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("UDPcomm/HandleData " + ex.Message);
            }
        }

        private void ReceiveData(IAsyncResult asyncResult)
        {
            try
            {
                // Initialise the IPEndPoint for the client
                EndPoint epSender = new IPEndPoint(IPAddress.Any, 0);

                // Receive all data
                int msgLen = recvSocket.EndReceiveFrom(asyncResult, ref epSender);

                byte[] localMsg = new byte[msgLen];
                Array.Copy(buffer, localMsg, msgLen);

                // Listen for more connections again...
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveData), epSender);

                int port = ((IPEndPoint)epSender).Port;
                // Update status through a delegate
                mf.Invoke(HandleDataDelegate, new object[] { port, localMsg });
            }
            catch (System.ObjectDisposedException)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("UDPcomm/ReceiveData " + ex.Message);
            }
        }

        private void SendData(IAsyncResult asyncResult)
        {
            try
            {
                sendSocket.EndSend(asyncResult);
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog(" UDP Send Data" + ex.ToString());
            }
        }

        private void SetEndPoints()
        {
            string Adr;
            IPAddress IP;
            string[] data;

            try
            {
                // ethernet
                cEthernetEP = IPAddress.Parse("192.168.1.255");
                if (IPAddress.TryParse(cDestinationIP, out IP))
                {
                    // keep pre-defined address
                    cEthernetEP = IP;
                }

                // wifi
                cWiFiIP = "127.0.0.1";
                cWiFiEP = IPAddress.Parse(cWiFiIP);
                Adr = GetLocalIPv4(NetworkInterfaceType.Wireless80211);
                if (IPAddress.TryParse(Adr, out IP))
                {
                    cWiFiIP = Adr;
                    data = Adr.Split('.');
                    cWiFiEP = IPAddress.Parse(data[0] + "." + data[1] + "." + data[2] + ".255");
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("UDPcomm/SetEndPoints " + ex.Message);
            }
        }
    }
}