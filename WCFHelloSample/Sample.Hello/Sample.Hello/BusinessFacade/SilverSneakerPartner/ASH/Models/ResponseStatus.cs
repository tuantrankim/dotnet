using System.Collections.Generic;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.ASH.Models
{
    public class ResponseStatus
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public List<Error> Errors { get; set; }
    }
}
