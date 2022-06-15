using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Workshop2022.TicketServiceClient
{
    public abstract class ReceivedMessageBase
    {
    }

    public interface IMessageProcessor
    {
        bool Process(string header, IDictionary<string, string> attributes, out ReceivedMessageBase message);
        string Header { get; }

    }

    public abstract class ReceivedMessageProcessorBase<T> : IMessageProcessor
        where T: ReceivedMessageBase, new()
    {
        protected Dictionary<string, (string fieldName, Type type)> MessageFields = new Dictionary<string, (string fieldName, Type type)>();
        protected T Message;

        protected ReceivedMessageProcessorBase()
        {
            Message = new T();
        }

        private protected ReceivedMessageProcessorBase(string header) : this()
        {
            this.Header = header;
        }

        public string Header { get; }

        internal void Map(Expression<Func<T,object>> fieldMap,  string attrib, Type type)
        {
            var me = (MemberExpression)fieldMap.Body;
            var pi = (PropertyInfo)me.Member;

            MessageFields.Add(attrib, (pi.Name, type));
        }

        public virtual bool Process(string header, IDictionary<string, string> attributes, out ReceivedMessageBase message)
        {
            message = null;

            try
            {
                foreach (var kvp in MessageFields)
                {
                    if (attributes.TryGetValue(kvp.Key, out var value))
                    {
                        var field = typeof(T).GetProperty(kvp.Value.fieldName);
                        field.SetValue(Message, Convert.ChangeType(value, kvp.Value.type));
                    }
                }

                message = Message;

                return true;
            }
            catch (Exception ex)
            {
                // implement exception handling 
            }

            return false;
        }
    }

    public class ServiceErrorMessage : ReceivedMessageBase
    {
        public string Error { get; set; }
        public string Message { get; set; }
    }

    public class ServiceErrorProcessor : ReceivedMessageProcessorBase<ServiceErrorMessage>
    {
        public ServiceErrorProcessor() : base("ER")
        {
            Map(f => f.Error, "EC", typeof(string));
            Map(f => f.Message, "FM", typeof(string));
        }
    }

    public class ValidationResultMessage : ReceivedMessageBase
    {
        public string User { get; set; }
        public string Result { get; set; }
    }

    public class ValidationResultProcessor : ReceivedMessageProcessorBase<ValidationResultMessage>
    {
        public ValidationResultProcessor() : base("US")
        {
            Map(f => f.User, "AN", typeof(string));
            Map(f => f.Result, "PG", typeof(string));
        }
    }

    public class LoggedInMessage: ReceivedMessageBase
    {
        public string User { get; set; }
        public string Campaign { get; set; }
    }

    public class LoggedInProcessor : ReceivedMessageProcessorBase<LoggedInMessage>
    {
        public LoggedInProcessor() : base("LI")
        {
            Map(f => f.User, "AN", typeof(string));
            Map(f => f.Campaign, "CN", typeof(string));
        }
    }

    public class LoggedOutMessage : ReceivedMessageBase
    {
        public string User { get; set; }
        public string Campaign { get; set; }
    }

    public class LoggedOutProcessor : ReceivedMessageProcessorBase<LoggedOutMessage>
    {
        public LoggedOutProcessor() : base("NO")
        {
            Map(f => f.User, "AN", typeof(string));
            Map(f => f.Campaign, "CN", typeof(string));
        }
    }

    public class AgentFreeMessage : ReceivedMessageBase
    {
        public string User { get; set; }
        public string Campaign { get; set; }
    }
    
    public class AgentFreeProcessor : ReceivedMessageProcessorBase<AgentFreeMessage>
    {
        public AgentFreeProcessor() : base("AF")
        {
            Map(f => f.User, "AN", typeof(string));
            Map(f => f.Campaign, "CN", typeof(string));
        }
    }

    public class AgentReadyMessage : ReceivedMessageBase
    {
        public string User { get; set; }
        public string Campaign { get; set; }
    }

    public class AgentReadyProcessor : ReceivedMessageProcessorBase<AgentReadyMessage>
    {
        public AgentReadyProcessor() : base("AR")
        {
            Map(f => f.User, "AN", typeof(string));
            Map(f => f.Campaign, "CN", typeof(string));
        }
    }


    public class CallEndedMessage : ReceivedMessageBase
    {
        public string User { get; set; }
        public string Campaign { get; set; }
    }

    public class CallEndedProcessor : ReceivedMessageProcessorBase<CallEndedMessage>
    {
        public CallEndedProcessor() : base("CE")
        {
            Map(f => f.User, "AN", typeof(string));
            Map(f => f.Campaign, "CN", typeof(string));
        }
    }

    public class AvailableMessage : ReceivedMessageBase
    {
        public string User { get; set; }
        public string Campaign { get; set; }
    }

    public class IsAvailableProcessor : ReceivedMessageProcessorBase<AvailableMessage>
    {
        public IsAvailableProcessor() : base("NA")
        {
            Map(f => f.User, "AN", typeof(string));
            Map(f => f.Campaign, "CN", typeof(string));
        }
    }

    public class AgentConnectMessage : ReceivedMessageBase
    {
        public struct DataField
        {
            public string FieldName;
            public string FieldType;
            public string FieldValue;

            public static DataField Parse(string fieldEntry)
            {
                var arr = fieldEntry.Split('~');

                return new DataField
                {
                    FieldName = arr[0],
                    FieldType = arr[1],
                    FieldValue = arr[2]
                };
            }
        }

        public string User { get; set; }
        public string Campaign { get; set; }
        public string Telephone { get; set; }
        public DataField[] Data { get; set; }

    }

    public class AgentConnectProcessor : ReceivedMessageProcessorBase<AgentConnectMessage>
    {
        public AgentConnectProcessor() : base("AC")
        {
            Map(f => f.User, "AN", typeof(string));
            Map(f => f.Campaign, "CN", typeof(string));
            Map(f => f.Telephone, "TN", typeof(string));

            //AS1\BT44699.046632\CB\TC\PT44699.046632
        }

        public override bool Process(string header, IDictionary<string, string> attributes, out ReceivedMessageBase message)
        {
            if (!base.Process(header, attributes, out message))
            {
                return false;
            }

            try
            {
                var data = attributes.ContainsKey("DT")
                    ? attributes["DT"]
                    : null;

                var fields = data.Split(new[] {'|'}, options: StringSplitOptions.RemoveEmptyEntries);

                Message.Data = fields.Select(AgentConnectMessage.DataField.Parse).ToArray();

                message = Message;
                
                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        }
    }

    public class MessageProcessor
    {
        private readonly Dictionary<string, (IMessageProcessor processor, Action<ReceivedMessageBase> action)> _messageProcessors = new Dictionary<string, (IMessageProcessor, Action<ReceivedMessageBase>)>();
        private Action _defaultAction;

        public void RegisterMessageProcessor(IMessageProcessor processor, Action<ReceivedMessageBase> action)
        {
            _messageProcessors.Add(processor.Header, (processor, action));
        }

        public void RegisterDefaultProcessor(Action action)
        {
            _defaultAction = action;
        }

        public bool Process(string header, IDictionary<string, string> attributes)
        {
            if (_messageProcessors.ContainsKey(header)
                && _messageProcessors[header].processor.Process(header, attributes, out var message))
            {
                _messageProcessors[header].action(message);
            }
            else
            {
                //FIX
                //_defaultAction();
            }

            return false;
        }
    }

}
