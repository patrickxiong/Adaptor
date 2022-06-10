using System;

namespace Workshop2022.TicketServiceClient
{
    public interface IMessageBuilder
    {
        IMessageBuilder Header(string header, string tenant = null);
        IMessageBuilder Attribute(string key, object value);
        IMessageBuilder AttributeIf(string key, object value, Func<bool> condition);

        string Build();

    }
}