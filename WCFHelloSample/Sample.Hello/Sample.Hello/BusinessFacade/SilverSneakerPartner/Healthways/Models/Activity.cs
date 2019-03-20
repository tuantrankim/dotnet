using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.Healthways.Models
{
    /* SubmitBatchActivity request
    [
    {
    "FirstName": "Peter",
    "LastName": "Parker",
    "DateOfBirth": null,
    "LocationCardNumber": "33779999",
    "TivityHealthCardNumber":"2300999999999999",
    "ActivityDateTime": "11/15/2017 10:08:07 AM",
    "TivityHealthLocationID": null,
    "YourClubID":"065",
    "Email":"peterparker@test.com",
    "HomePhoneNumber": "999-999-9999",
    "MobilePhoneNumber": "999-999-9999"
    },
    {
    "FirstName": "Bruce",
    "LastName": "Wayne",
    "DateOfBirth": null,
    "LocationCardNumber": "33776777",
    "TivityHealthCardNumber":"2300999999999999",
    "ActivityDateTime": "11/15/2017 11:08:07 AM",
    "TivityHealthLocationID": null,
    "YourClubID":"065",
    "Email":"brucewayne@test.com",
    "HomePhoneNumber": "999-999-9999",
    "MobilePhoneNumber": "999-999-9999"
    }
    ]
      */
    public class Activity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string LocationCardNumber { get; set; }
        public string TivityHealthCardNumber { get; set; }
        public DateTime ActivityDateTime { get; set; }
        public string TivityHealthLocationID { get; set; }
        public string YourClubID { get; set; }
        public string ReferenceID { get; set; }
        public string Email { get; set; }
        public string HomePhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }

    }

    public class ActivityStatus 
    {
        public Activity MemberDetails { get; set; }
        public String EligibilityStatus {get; set;}  //E.g.: "EligibilityStatusNotFound"
        public String Reason {get; set;} //E.g.: "Eligibility Status Reason Not Found"
    }
}
