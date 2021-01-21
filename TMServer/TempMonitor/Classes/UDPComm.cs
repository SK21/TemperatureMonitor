using System;
using System.Net;
using System.Net.Sockets;
using TempMonitor.Classes;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace TempMonitor
{
    public class UDPComm
    {
        public bool isUDPSendConnected;
        private readonly FormMain mf;
        private byte[] buffer = new byte[1024];

        private IPAddress epIP;
        private Socket recvSocket;
        private Socket sendSocket;

        private HandleDataDelegateObj HandleDataDelegate = null;

        // local ports must be unique for each app on same pc
        private int ListenPort = 1688;      // arduino sends on
        private int SendToPort = 8120;      // arduino listens on
        private int SendFromPort = 1680;    

        public UDPComm(FormMain CallingForm)
        {
            mf = CallingForm;
            SetEpIP();
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChanged);
        }

        // Status delegate
        private delegate void HandleDataDelegateObj(int port, byte[] msg);

        //sends byte array
        public void SendUDPMessage(byte[] byteData)
        {
            if (isUDPSendConnected)
            {
                try
                {
                    IPEndPoint EndPt = new IPEndPoint(epIP, SendToPort);

                    // Send packet to the zero
                    if (byteData.Length != 0)
                        sendSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, EndPt, new AsyncCallback(SendData), null);
                    //mf.WriteEvent("sent udp message to control box " + byteData[2].ToString());
                }
                catch (Exception e)
                {
                    mf.Tls.WriteErrorLog("Sending UDP Message" + e.ToString());
                    mf.Tls.TimedMessageBox("Send Error", e.Message);
                }
            }
        }

        public void StartUDPServer()
        {
            try
            {
                // Initialise the delegate which updates the message received
                HandleDataDelegate = HandleData;

                // Initialise the socket
                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Initialise the IPEndPoint for the server and listen on 
                IPEndPoint recv = new IPEndPoint(IPAddress.Any, ListenPort);

                // Associate the socket with this IP address and port
                recvSocket.Bind(recv);

                // Initialise the IPEndPoint for the server to send on 
                IPEndPoint server = new IPEndPoint(IPAddress.Any, SendFromPort);
                sendSocket.Bind(server);

                // Initialise the IPEndPoint for the client - async listner client only!
                EndPoint client = new IPEndPoint(IPAddress.Any, 0);

                // Start listening for incoming data
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                                                ref client, new AsyncCallback(ReceiveData), recvSocket);
                isUDPSendConnected = true;
            }
            catch (Exception e)
            {
                mf.Tls.WriteErrorLog("UDP Server" + e);
                mf.Tls.TimedMessageBox("UDP start error: ", e.Message);
            }
        }

        private void HandleData(int Port, byte[] Data)
        {
            mf.ReceiveInfo.ParseByteData(Data);
            mf.ReceiveDiagnostics.ParseByteData(Data);
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

                //string text =  Encoding.ASCII.GetString(localMsg);

                int port = ((IPEndPoint)epSender).Port;
                // Update status through a delegate
                mf.Invoke(HandleDataDelegate, new object[] { port, localMsg });
            }
            catch (Exception e)
            {
                mf.Tls.WriteErrorLog("UDP Recv data " + e.ToString());
                mf.Tls.TimedMessageBox("ReceiveData Error", e.Message);
            }
        }

        private void SendData(IAsyncResult asyncResult)
        {
            try
            {
                sendSocket.EndSend(asyncResult);
            }
            catch (Exception e)
            {
                mf.Tls.WriteErrorLog(" UDP Send Data" + e.ToString());
            }
        }

        public string LocalIP()
        {
            try
            {
                using(Socket socket=new Socket(AddressFamily.InterNetwork,SocketType.Dgram,0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    return endPoint.Address.ToString();
                }
            }
            catch (Exception)
            {
                return "127.0.0.1";
            }
        }

        private void SetEpIP()
        {
            string Result = "";
            string IP = LocalIP();
            string[] data = IP.Split('.');
            if (data.Length == 4)
            {
                Result = data[0] + "." + data[1] + "." + data[2] + ".255";
            }

            if (IPAddress.TryParse(Result, out IPAddress Tmp))
            {
                epIP = Tmp;
            }
            else
            {
                epIP = IPAddress.Parse("127.0.0.255");
            }
        }

        private void AddressChanged(object sender, EventArgs e)
        {
            SetEpIP();
        }

        public string BroadcastIP()
        {
            return epIP.ToString();
        }
    }
}