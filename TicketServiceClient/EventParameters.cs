using System.Collections.Generic;

namespace Workshop2022.TicketServiceClient
{
    public class ErrorResponse
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

    }

    public class ValidatedUserResponse
    {
        public string UserId { get; set; }
        public bool IsValidated { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class LoggedInResponse
    {
        public string UserId { get; set; }
        public string Campaign { get; set; }
        public string Tenant { get; set; }
    }

    public class LoggedOutResponse
    {
        public string UserId { get; set; }
        public string Campaign { get; set; }
        public string Tenant { get; set; }
    }


    public class AvailableResponse
    {
        public string UserId { get; set; }
        public string Campaign { get; set; }
        public string Tenant { get; set; }
    }

    public class AgentFreeResponse
    {
        public string UserId { get; set; }
        public string Campaign { get; set; }
        public string Tenant { get; set; }
    }


    public class CallEndedResponse
    {
        public string UserId { get; set; }
        public string Campaign { get; set; }
        public string Tenant { get; set; }
    }

    public class TicketDataResponse
    {
        public string UserId { get; set; }
        public string Campaign { get; set; }
        public string Tenant { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsActiveCall { get; set; }

        public bool IsPreview { get; set; }

        public bool IsManualCall { get; set; }

        public IReadOnlyList<TicketDataField> Data { get; set; }
    }

    public class TicketDataField
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }    // text or numeric
        public string FieldValue { get; set; }
    }
}
