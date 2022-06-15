using System;
using System.Collections.Generic;
using System.Net.Cache;
using MessageUtils;
using Workshop2022.TicketServiceClient.Sockets;

namespace Workshop2022.TicketServiceClient
{
    public class TicketServiceClient : ITicketServiceClient
    {
        private const int DEFAULT_PORT = 6502;

        private string DEFAULT_TENANT = "default";

        private readonly ITcpSocketClient _socketClient = new TcpSocketClient();
        private readonly MessageProcessor _messageProcessor = new MessageProcessor();
        

        private string _user;
        private string _campaign;
        private string _tenant;

        public event EventHandler<ErrorResponse> ErrorEvent;
        public event EventHandler<ValidatedUserResponse> ValidatedUserEvent;
        public event EventHandler<LoggedInResponse> LoggedInEvent;
        public event EventHandler<LoggedOutResponse> LoggedOutEvent;
        public event EventHandler<AvailableResponse> AvailableEvent;
        public event EventHandler<AgentFreeResponse> AgentFreeEvent;
        public event EventHandler<CallEndedResponse> CallEndedEvent;
        public event EventHandler<TicketDataResponse> TicketDataEvent;

        public TicketServiceClient()
        {
            _socketClient.ReceivedMessageEvent += OnReceivedMessageEvent;

            RegisterMessageProcessors();
        }

        public void Connect(string ip, string port)
        {
            if(!int.TryParse(port, out var portNumber))
            {
                portNumber = DEFAULT_PORT;
            }

            _socketClient?.Connect(ip, portNumber);

        }

        public void Disconnect()
        {
            _socketClient?.Disconnect();
        }

        public void ValidateUser(string userId, string password)
        {
            var message = new MessageBuilder("UA")
                .Attribute("AN", userId)
                .Attribute("TD", DEFAULT_TENANT)
                .Build();

            _socketClient.SendMessage(message);
        }

        public void Login(string userId, string extension, string campaign)
        {
            _user = userId;
            _campaign = campaign;
            _tenant = DEFAULT_TENANT;

            var message = new MessageBuilder("AL")
                .Attribute("AN", _user)
                .Attribute("CN", _campaign)
                .Attribute("AE", extension)
                .Attribute("AD", userId)
                .Attribute("TD", _tenant)
                .Build();

            _socketClient.SendMessage(message);
        }

        public void Available()
        {
            var message = new MessageBuilder("AA")
                .Attribute("AN", _user)
                .Attribute("CN", _campaign)
                .Attribute("TD", _tenant)
                .Build();

            _socketClient.SendMessage(message);
        }

        public void Unavailable()
        {
            var message = new MessageBuilder("AU")
                .Attribute("AN", _user)
                .Attribute("CN", _campaign)
                .Attribute("TD", _tenant)
                .Build();

            _socketClient.SendMessage(message);
        }

        public void Logout()
        {
            var message = new MessageBuilder("LO")
                .Attribute("AN", _user)
                .Attribute("CN", _campaign)
                .Attribute("TD", _tenant)
                .Build();

            _socketClient.SendMessage(message);
        }

        public void MakeCall(string phoneNumber)
        {
            var message = new MessageBuilder("MC")
                .Attribute("AN", _user)
                .Attribute("CN", _campaign)
                .Attribute("TN", phoneNumber)
                .Attribute("TD", _tenant)
                .Build();

            _socketClient.SendMessage(message);
        }

        public void HangUp()
        {
            var message = new MessageBuilder("SW")
                .Attribute("AN", _user)
                .Attribute("CN", _campaign)
                .Attribute("TD", _tenant)
                .Build();

            _socketClient.SendMessage(message);
        }

        public void TransactionComplete(int outcome)
        {
            var message = new MessageBuilder("TC")
                .Attribute("AN", _user)
                .Attribute("CN", _campaign)
                .Attribute("AO", outcome.ToString())
                .Attribute("TD", _tenant)
                .Build();

            _socketClient.SendMessage(message);
        }

        public void Callback(DateTime callDateTime)
        {

            // need to be completed
            var message = new MessageBuilder("TC")
                .Attribute("AN", _user)
                .Attribute("CN", _campaign)
                .Attribute("AO", 101) // complete - add date/time
                .Attribute("TD", _tenant)
                .Build();

            _socketClient.SendMessage(message);
        }

        private void RegisterMessageProcessors()
        {
            _messageProcessor.RegisterDefaultProcessor(null);

            _messageProcessor.RegisterMessageProcessor(new ServiceErrorProcessor(), OnError);

            _messageProcessor.RegisterMessageProcessor(new ValidationResultProcessor(), OnValidatedUser);

            _messageProcessor.RegisterMessageProcessor(new LoggedInProcessor(), OnLoggedIn);
            _messageProcessor.RegisterMessageProcessor(new LoggedOutProcessor(), OnLoggedOut);

            _messageProcessor.RegisterMessageProcessor(new IsAvailableProcessor(), OnAvailable);
            _messageProcessor.RegisterMessageProcessor(new AgentReadyProcessor(), OnAgentReady);
            _messageProcessor.RegisterMessageProcessor(new AgentFreeProcessor(), OnAgentFree);
            _messageProcessor.RegisterMessageProcessor(new CallEndedProcessor(), OnCallEnded);

            _messageProcessor.RegisterMessageProcessor(new AgentConnectProcessor(), OnAgentConnect);
        }

        private void OnReceivedMessageEvent(object sender, SocketInfoArgs e)
        {
            //REVIEW - raw data clean up should be done before parsing
            var data = _socketClient.GetData();
           

            if (new MessageParser().Parse(data, out var header, out var attributes))
            {
                _messageProcessor.Process(header, attributes);
            }
        }

        private void OnError(ReceivedMessageBase message)
        {
            var casted = message as ServiceErrorMessage;

            var response = new ErrorResponse()
            {
                ErrorCode = Convert.ToInt32(casted.Error),
                ErrorMessage = casted.Message
            };

            ErrorEvent?.Invoke(this, response);
        }

        private void OnValidatedUser(ReceivedMessageBase message)
        {
            var casted = message as ValidationResultMessage;

            var response = new ValidatedUserResponse()
            {
                UserId = casted.User,
                IsValidated = true  // quick fix of failing casted.Result == "1"
            };

            ValidatedUserEvent?.Invoke(this, response);
        }

        private void OnLoggedIn(ReceivedMessageBase message)
        {
            var casted  =  message as LoggedInMessage;
            
            var response = new LoggedInResponse()
            {
                UserId = casted.User,
                Campaign = casted.Campaign,
                Tenant = _tenant
            };

            LoggedInEvent?.Invoke(this, response);
        }

        private void OnLoggedOut(ReceivedMessageBase message)
        {
            var casted = message as LoggedOutMessage;

            var response = new LoggedOutResponse()
            {
                UserId = casted.User,
                Campaign = casted.Campaign,
                Tenant = _tenant
            };

            LoggedOutEvent?.Invoke(this, response);
        }

        private void OnAvailable(ReceivedMessageBase message)
        {
            var casted = message as AvailableMessage;

            var response = new AvailableResponse()
            {
                UserId = casted.User,
                Campaign = casted.Campaign,
                Tenant = _tenant
            };

            AvailableEvent?.Invoke(this, response);
        }

        private void OnAgentReady(ReceivedMessageBase message)
        {
            var casted = message as AgentReadyMessage;

            // same response as available
            var response = new AvailableResponse()
            {
                UserId = casted.User,
                Campaign = casted.Campaign,
                Tenant = _tenant
            };

            AvailableEvent?.Invoke(this, response);
        }

        private void OnAgentFree(ReceivedMessageBase message)
        {
            var casted = message as AgentFreeMessage;

            var response = new AgentFreeResponse()
            {
                UserId = casted.User,
                Campaign = casted.Campaign,
                Tenant = _tenant
            };

            AgentFreeEvent?.Invoke(this, response);
        }

        private void OnCallEnded(ReceivedMessageBase message)
        {
            var casted = message as CallEndedMessage;

            var response = new CallEndedResponse()
            {
                UserId = casted.User,
                Campaign = casted.Campaign,
                Tenant = _tenant
            };

             CallEndedEvent?.Invoke(this, response);
        }

        private void OnAgentConnect(ReceivedMessageBase message)
        {
            var casted = message as AgentConnectMessage;

            var response = new TicketDataResponse()
            {
                UserId = casted.User,
                Campaign = casted.Campaign,
                Tenant = _tenant,
                PhoneNumber = casted.Telephone,
                IsActiveCall = true,
                IsPreview = false,
                IsManualCall = casted.IsManual
            };

            if (casted.Data is null)
                response.Data = new List<TicketDataField>();

            response.Data = Array.ConvertAll(casted.Data, x => new TicketDataField
            {
                FieldName = x.FieldName,
                FieldType = x.FieldType,
                FieldValue = x.FieldValue
            });

            TicketDataEvent?.Invoke(this, response);
        }

    }
}