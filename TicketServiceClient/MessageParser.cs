using System.Collections.Generic;

namespace Workshop2022.TicketServiceClient
{
    public class MessageParser
    {
        public bool Parse(string recvMessage, out string header, out IDictionary<string, string> attributes)
        {
            //Example: LI\ANuser_name\CNcampaign_name\TDtenant\YAingore_the_content

            attributes = new Dictionary<string, string>();

            var arr = recvMessage.Split('\\');

            header = arr.Length > 0 ? arr[0] : null ;

            for (var i = 1; i < arr.Length; i++)
            {
                var elem = arr[i];

                if (elem.Length < 2)
                {
                    return false;
                }
                if (elem.Length == 2)
                {
                    attributes.Add(elem.Substring(0, 2), null);
                }
                else
                {
                    attributes.Add(elem.Substring(0, 2), elem.Substring(2).Trim());
                }
            }

            return !string.IsNullOrEmpty(header) && attributes.Count > 2;
        }
    }
}
