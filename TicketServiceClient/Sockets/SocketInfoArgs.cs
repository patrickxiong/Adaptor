using System;

namespace Workshop2022.TicketServiceClient.Sockets
{
    public class SocketInfoArgs : EventArgs
   {
       public string sData { get; set; }
       public int iErrorNum { get; set; }
       public string sErrorDesc { get; set; }

       public SocketInfoArgs()
       {
           sData = "";
           iErrorNum = 0;
           sErrorDesc = "";
       }

       public SocketInfoArgs(string psData)
       {
           sData = psData;
       }

       public SocketInfoArgs(int piErr, string psErr)
       {
           iErrorNum = piErr;
           sErrorDesc = psErr;
       }

   }

}
