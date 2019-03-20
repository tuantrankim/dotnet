using Sample.Hello.Common.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.Common.Data
{
    [DataContract]
    public class SilverSneakerResult
    {
        [DataMember]
        public string MessageFromLAF { get; set; }
        [DataMember]
        public string MessageFromPartner { get; set; }
        //[DataMember]
        //public SilverSneakerErrorTypes ErrorType { get; set; }
        //[DataMember]
        //public ErrorCodes ErrorCode { get; set; }
    }
}

