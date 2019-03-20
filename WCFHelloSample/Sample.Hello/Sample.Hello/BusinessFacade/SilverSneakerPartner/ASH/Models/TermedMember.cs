using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.ASH.Models
{
    public class TermedMember : BaseResponse
    {
        public int FitnessId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MemberProgramName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public String FormattedTerminationDate { get; set; }
    }
}
