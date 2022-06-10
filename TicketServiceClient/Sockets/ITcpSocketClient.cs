using System;
using Workshop2022.TicketServiceClient.Sockets;

namespace MessageUtils
{
    public interface ITcpSocketClient
    {
        void Connect(string ipAddress, long port);
        void Disconnect();
        void SendMessage(string message);

        string GetData();  // review!

        event EventHandler<SocketInfoArgs> ErrorEvent; 
        event EventHandler<SocketInfoArgs> ReceivedMessageEvent;
    }
}