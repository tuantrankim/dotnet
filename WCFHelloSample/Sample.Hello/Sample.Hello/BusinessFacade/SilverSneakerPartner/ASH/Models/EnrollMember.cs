using Sample.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.ASH.Models
{
    public class EnrollMember : BaseResponse
    {
        public string EnrollmentRequestId { get; set; }
        public string FirstName { get; set; }
        public string lastName { get; set; }
        public string ProgramEffectiveDate { get; set; }
        public string ProgramTerminationDate { get; set; }

        public int EnrollResultCode { get; set; }
        public bool IsSuccess()
        {
            DateTime programEffectiveDate = DateTime.MaxValue;
            if (!DateTime.TryParse(ProgramEffectiveDate, out programEffectiveDate)) programEffectiveDate = DateTime.MaxValue;
            if (EnrollResultCode == 200 && programEffectiveDate.Date <= DateTime.Today)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}



