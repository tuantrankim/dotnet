using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.Healthways.Models
{
    public class IneligibleMember
    {
        public Activity MemberDetails { get; set; }
        public String EligibilityStatus { get; set; }
        public String Reason { get; set; }
    }
}
