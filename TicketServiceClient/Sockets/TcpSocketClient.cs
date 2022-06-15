using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MessageUtils;

namespace Workshop2022.TicketServiceClient.Sockets
{
    public class TcpSocketClient : ITcpSocketClient, IDisposable
    {

        public const long MAX_DATA_BUFFER_SIZE = 16384;
        public const int DATA_BUFF_SIZE = 255;

        public event EventHandler<SocketInfoArgs> ReceivedMessageEvent;
        public event EventHandler<SocketInfoArgs> ErrorEvent;
        public event EventHandler<SocketInfoArgs> OnSocketClosed;


        private Queue<string> m_DataStack = new Queue<string>();

        // private byte[] m_dataBuffer = new byte[10];
        private IAsyncResult m_result;
        private AsyncCallback m_pfnCallBack;
        private Socket m_clientSocket;
        private bool _connectedFlag;

        private AutoResetEvent sychEvent;

        private string m_sData = "";

        public TcpSocketClient()
        {
            sychEvent = new AutoResetEvent(false);
        }

        public void Dispose()
        {
            if (m_clientSocket.Connected)
            {
                m_clientSocket.Close();
            }
            m_clientSocket = null;
        }


        ~TcpSocketClient()
        {

            if (m_clientSocket != null)
            {
                if (m_clientSocket.Connected)
                {
                    m_clientSocket.Close();
                }
                m_clientSocket = null;

            }
        }

        public Socket GetSocket()
        {
            return m_clientSocket;
        }

        public bool IsConnected()
        {
            return _connectedFlag;
        }


        private void ClearDataBuffer()
        {
            m_sData = "";
        }

        public string GetData()
        {
            var data = m_DataStack.Dequeue();
            return data.EndsWith("\0")
                ? data?.Substring(0, data.Length - 1)
                : data;
        }
        

        private void StackDataBuffer(char[] parrBuff)
        {

            string sData = new String(parrBuff);

            // -- identify remove trailing \0 (always exist as a result of the conversion!)
            int j = sData.Length;
            while (j >= 0 && sData[--j] != '\0') ;

            if (j > 0) sData = sData.Substring(0, j);

            // -- add previous data to the new one
            m_sData = m_sData + sData;

            // -- if the data are a complete command - pass the processing
            string sBuff = "";
            for (int k = 0; k < m_sData.Length; k++)
                if (m_sData[k] != '\0')   // -- examine end-of-command sequence
                    sBuff += m_sData[k];
                else
                {
                    sBuff += "\0";
                    m_DataStack.Enqueue(sBuff);

                    sBuff = "";
                        
                    //REVIEW - why the event was not a good place for data ?

                    if (ReceivedMessageEvent != null)
                        ReceivedMessageEvent(this, new SocketInfoArgs());
                }

            // -- place unprocessed characters into the shared buffer
            m_sData = sBuff;

        }

        public void Connect(string psServerAddr, long plPort)
        {
            // See if we have text on the IP and Port text fields
            if (psServerAddr == "" || plPort == 0)
            {
                throw new Exception("IP Address and Port Number are required to connect to the Server\n");
            }
            try
            {
                // Create the socket instance
                m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Cet the remote IP address
                IPAddress ip = IPAddress.Parse(psServerAddr);
                int iPortNo = System.Convert.ToInt16(plPort);   // Using Convert to ensure compatibility
                // Create the end point 
                IPEndPoint ipEnd = new IPEndPoint(ip, iPortNo);
                // Connect to the remote host
                m_clientSocket.Connect(ipEnd);
                if (m_clientSocket.Connected)
                {
                    _connectedFlag = true;
                    ClearDataBuffer();

                    // -- start data reception loop
                    WaitForData();
                }
                else
                    _connectedFlag = false;

            }
            catch (SocketException se)
            {
                string str;
                str = "\nConnection failed, is the server running?\n" + se.Message;
                if (ErrorEvent != null)
                    ErrorEvent(this, new SocketInfoArgs(100, se.Message));
                else
                    throw new Exception(str);
            }

        }



        public void SendMessage(string psMsg)
        {

            try
            {
                object objData = (object)psMsg;
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString() + "\0");
                //byte[] byData = System.Text.Encoding.Unicode.GetBytes(objData.ToString() + "\0");
                if (m_clientSocket != null)
                {
                    m_clientSocket.Send(byData);
                }
            }
            catch (SocketException se)
            {
                if (ErrorEvent != null)
                    ErrorEvent(this, new SocketInfoArgs(100, se.Message));
                else
                    throw new Exception(se.Message);

                throw new Exception(se.Message);
            }
        }


        public void WaitForData()
        {
            try
            {

                // -- thread sych resolution - PS
                sychEvent.Reset();


                if (m_pfnCallBack == null)
                {
                    m_pfnCallBack = new AsyncCallback(OnDataReceived);
                }
                SocketPacket socketPacket = new SocketPacket();
                socketPacket.socket = m_clientSocket;

                // Start listening to the data asynchronously
                m_result = m_clientSocket.BeginReceive(socketPacket.dataBuffer,
                    0, socketPacket.dataBuffer.Length,
                    SocketFlags.None,
                    m_pfnCallBack,
                    socketPacket);
            }
            catch (SocketException se)
            {
                if (ErrorEvent != null)
                    ErrorEvent(this, new SocketInfoArgs(se.Message));
                else
                    throw new Exception(se.Message);

                throw new Exception(se.Message);
            }

        }

        public class SocketPacket
        {
            public System.Net.Sockets.Socket socket;
            public byte[] dataBuffer = new byte[DATA_BUFF_SIZE];
        }

        public void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                SocketPacket socketData = (SocketPacket)asyn.AsyncState;

                int iRx = 0;
                // Complete the BeginReceive() asynchronous call by EndReceive() method
                // which will return the number of characters written to the stream 
                // by the client
                iRx = socketData.socket.EndReceive(asyn);
                char[] chars = new char[iRx + 1];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                //System.Text.Decoder d = System.Text.Encoding.Unicode.GetDecoder();
                int charLen = d.GetChars(socketData.dataBuffer,
                    0, iRx, chars, 0);

                StackDataBuffer(chars);

                // -- submit call back until the next data arrive
                WaitForData();

            }
            catch (ObjectDisposedException)
            {
                if (OnSocketClosed != null)
                    OnSocketClosed(this, new SocketInfoArgs());
                else
                    throw new Exception("OnDataReceived: Socket has been closed!");
            }
            catch (SocketException se)
            {
                if (ErrorEvent != null)
                    ErrorEvent(this, new SocketInfoArgs(100, se.Message));
                else
                    throw new Exception(se.Message);

            }
        }


        public void Disconnect()
        {
            if (m_clientSocket != null)
            {
                if (m_clientSocket.Connected)
                {
                    m_clientSocket.Close();
                    m_clientSocket = null;
                }
            }
        }

        //----------------------------------------------------	
        // This is a helper function used (for convenience) to 
        // get the IP address of the local machine
        //----------------------------------------------------
        //public String GetIP()
        //{
        //   String strHostName = "localhost"; //Dns.GetHostName();

        //   // Find host by name
        //   IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

        //   // Grab the first IP addresses
        //   String IPStr = "";
        //   foreach (IPAddress ipaddress in iphostentry.AddressList)
        //   {
        //      IPStr = ipaddress.ToString();
        //      return IPStr;
        //   }
        //   return IPStr;
        //}


        public void CloseSocket()
        {
            if (m_clientSocket != null)
            {
                m_clientSocket.Close();
                m_clientSocket = null;
            }
        }

        public string GetRemoteIP()
        {
            return ((IPEndPoint)m_clientSocket.RemoteEndPoint).Address.ToString();

        }
        public string GetLocalIP()
        {
            return ((IPEndPoint)m_clientSocket.LocalEndPoint).Address.ToString();
        }

        public long GetRemotePort()
        {
            return (long)(((IPEndPoint)m_clientSocket.RemoteEndPoint).Port);
        }
        public long GetLocalPort()
        {
            return (long)(((IPEndPoint)m_clientSocket.LocalEndPoint).Port);
        }

        public string GetProtocolType()
        {
            return m_clientSocket.ProtocolType.ToString();
        }

    }
}