using Sample.Hello.BusinessFacade.SilverSneakerPartner.ASH.Models;
using Sample.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.Optum.Models
{
    public class MemberResult
    {
        public bool error { get; set; }
        public string error_message { get; set; }
        public List<Result> result { get; set; }
    }
}
