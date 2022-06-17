using System;
using System.Text;

namespace Workshop2022.TicketServiceClient
{
    public class MessageBuilder : IMessageBuilder
    {
        private readonly StringBuilder _builder;
        private string _tenant;

        public MessageBuilder()
        {
        }

        public MessageBuilder(string messageHeader, string tenant = null)
        {
            _builder = new StringBuilder(messageHeader);
            _tenant = tenant;
        }


        public IMessageBuilder Header(string header, string tenant = null)
        {
            _tenant = tenant;

            _builder.AppendLine(header);

            return this;
        }

        public IMessageBuilder Attribute(string key, object value = null)
        {
            if (value is null) 
            {
                _builder.AppendFormat("\\{0}", key);
            }
            else
            {
                _builder.AppendFormat("\\{0}{1}", key, value);
            }

            return this;
        }

        public IMessageBuilder AttributeIf(string key, object value, Func<bool> condition)
        {
            if (!condition())
            {
                return this;
            }

            _builder.AppendFormat("\\{0}{1}", key, value);

            return this;
        }

        public string Build()
        {
            if (!_builder.ToString().Contains("\\TD"))
            {
                _builder.AppendFormat(_tenant ?? "\\TDdefault");
            }

            return _builder.ToString();
        }

    }
}
