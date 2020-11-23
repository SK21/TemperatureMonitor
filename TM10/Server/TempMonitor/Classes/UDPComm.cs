using System;
using System.Net;
using System.Net.Sockets;
using TempMonitor.Classes;

namespace TempMonitor
{
    public class UDPComm
    {
        public bool isUDPSendConnected;
        private readonly FormMain mf;

        private byte[] buffer = new byte[1024];

        private string cLocalIP;

        private IPAddress epIP = IPAddress.Parse(Properties.Settings.Default.DestinationIP);

        private HandleDataDelegateObj HandleDataDelegate = null;

        private Socket recvSocket;

        private Socket sendSocket;

        public UDPComm(FormMain CallingForm)
        {
            mf = CallingForm;
        }

        // new data event
        public delegate void NewDataDelegate(byte[] Data);

        // Status delegate
        private delegate void HandleDataDelegateObj(int port, byte[] msg);

        public string LocalIP { get { return cLocalIP; } }

        //sends byte array
        public void SendUDPMessage(byte[] byteData)
        {
            if (isUDPSendConnected)
            {
                try
                {
                    IPEndPoint EndPt = new IPEndPoint(epIP, 8120);

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

                // Initialise the IPEndPoint for the server and listen on port 2388
                IPEndPoint recv = new IPEndPoint(IPAddress.Any, 2388);
                cLocalIP = recv.Address.ToString();

                // Associate the socket with this IP address and port
                recvSocket.Bind(recv);

                // Initialise the IPEndPoint for the server to send on port 2188
                IPEndPoint server = new IPEndPoint(IPAddress.Any, 2188);
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
                mf.Tls.TimedMessageBox("SendData Error", e.Message);
            }
        }
    }
}