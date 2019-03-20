using Sample.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.ASH.Models
{
    public class Member : BaseResponse
    {
        public int? MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string ZipCode { get; set; }
        public string ProductName { get; set; }
        public string EffectiveDate { get; set; }
        public string TerminationDate { get; set; }
        public string FacilityID { get; set; }
        //Old ASH API before 12/15/2016 for Hello 22.1.0.0 backward
        public List<Facility> Facilities { get; set; }
        public Facility GetFacility()
        {
            if (Facilities == null) return null;
            else
            {
                return Facilities.Where(f => f.FacilityName.ToLower().StartsWith("la fitness")).FirstOrDefault();
            }
        }
        //New ASH API
        public int VerifyResultCode { get; set; }
        public string EnrollmentRequestId { get; set; }

        public bool IsEligible()
        {
            //New API. old API VerifyResultCode == 0
            /*
             A VerifyResultCode of 200 indicates the member is verified as eligible and need to enroll.
             A VerifyResultCode of 201 indicates the member is verified as eligible but does not need to enroll.
             */
            if (VerifyResultCode == 200 || VerifyResultCode == 201) return true;
            else if(VerifyResultCode > 0) return false;
            
            //Old API             
            if (this.ResponseStatus != null
                && this.ResponseStatus.Errors == null
                && this.ResponseStatus.Message == "Eligible"
                )
            {
                return true;
            }
            else
            {
                //LogHelper.LogError("Sample.Hello.Exception", "Sample.Hello", message, null, null, false, null, System.Reflection.MethodInfo.GetCurrentMethod().ToString(), null);
                return false;
            }
        }
        
    }

    public class Facility
    {
        public int? DocId { get; set; }
        public int? ClinicId { get; set; }
        public string FacilityName { get; set; }
        public string FacilityAddress { get; set; }
    }
}
