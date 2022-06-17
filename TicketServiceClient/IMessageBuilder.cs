using System;

namespace Workshop2022.TicketServiceClient
{
    public interface IMessageBuilder
    {
        IMessageBuilder Header(string header, string tenant = null);
        IMessageBuilder Attribute(string key, object value = null);
        IMessageBuilder AttributeIf(string key, object value, Func<bool> condition);

        string Build();

    }
}