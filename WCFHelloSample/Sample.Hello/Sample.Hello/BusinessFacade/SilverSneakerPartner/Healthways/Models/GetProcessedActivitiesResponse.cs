using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.Healthways.Models
{
    /*
     
     * {
    "Request": {
        "BatchGUID": "44a75839-8a2b-4142-b71f-29f4d530bae3"
    },
    "Response": {
        "MemberActivities": [
            {
                "MemberDetails": {
                    "FirstName": "Peter",
                    "LastName": "Cortez",
                    "DateOfBirth": null,
                    "Email": null,
                    "HomePhoneNumber": null,
                    "MobilePhoneNumber": null,
                    "TivityHealthCardNumber": "2300103780571119",
                    "LocationCardNumber": "",
                    "YourClubID": "065",
                    "ActivityDateTime": "2018-02-28T10:08:07",
                    "TivityHealthLocationID": "26557",
                    "ReferenceID": null
                },
                "EligibilityStatus": "EligibilityStatusNotFound",
                "Reason": "Eligibility Status Reason Not Found"
            },
            {
                "MemberDetails": {
                    "FirstName": "Diana",
                    "LastName": "Gonzalez",
                    "DateOfBirth": null,
                    "Email": null,
                    "HomePhoneNumber": null,
                    "MobilePhoneNumber": null,
                    "TivityHealthCardNumber": "2300104355769030",
                    "LocationCardNumber": "",
                    "YourClubID": "065",
                    "ActivityDateTime": "2018-02-28T10:08:07",
                    "TivityHealthLocationID": "26557",
                    "ReferenceID": null
                },
                "EligibilityStatus": "EligibilityStatusNotFound",
                "Reason": "Eligibility Status Reason Not Found"
            },
            {
                "MemberDetails": {
                    "FirstName": "Jose",
                    "LastName": "Coro",
                    "DateOfBirth": null,
                    "Email": null,
                    "HomePhoneNumber": null,
                    "MobilePhoneNumber": null,
                    "TivityHealthCardNumber": "2300109675437568",
                    "LocationCardNumber": "",
                    "YourClubID": "065",
                    "ActivityDateTime": "2018-02-28T10:08:07",
                    "TivityHealthLocationID": "26557",
                    "ReferenceID": null
                },
                "EligibilityStatus": "EligibilityStatusNotFound",
                "Reason": "Eligibility Status Reason Not Found"
            },
            {
                "MemberDetails": {
                    "FirstName": "PIL",
                    "LastName": "CHONG",
                    "DateOfBirth": null,
                    "Email": null,
                    "HomePhoneNumber": null,
                    "MobilePhoneNumber": null,
                    "TivityHealthCardNumber": "32587400",
                    "LocationCardNumber": "",
                    "YourClubID": "065",
                    "ActivityDateTime": "2018-02-28T10:08:07",
                    "TivityHealthLocationID": "26557",
                    "ReferenceID": null
                },
                "EligibilityStatus": "EligibilityStatusNotFound",
                "Reason": "Eligibility Status Reason Not Found"
            },
            {
                "MemberDetails": {
                    "FirstName": "Elvin",
                    "LastName": "Cabrera",
                    "DateOfBirth": null,
                    "Email": null,
                    "HomePhoneNumber": null,
                    "MobilePhoneNumber": null,
                    "TivityHealthCardNumber": "0555003195859496",
                    "LocationCardNumber": "",
                    "YourClubID": "065",
                    "ActivityDateTime": "2018-02-28T10:08:07",
                    "TivityHealthLocationID": "26557",
                    "ReferenceID": null
                },
                "EligibilityStatus": "EligibilityStatusNotFound",
                "Reason": "Eligibility Status Reason Not Found"
            },
            {
                "MemberDetails": {
                    "FirstName": "Joseph",
                    "LastName": "Miller",
                    "DateOfBirth": null,
                    "Email": null,
                    "HomePhoneNumber": null,
                    "MobilePhoneNumber": null,
                    "TivityHealthCardNumber": "0555003111022964",
                    "LocationCardNumber": "",
                    "YourClubID": "065",
                    "ActivityDateTime": "2018-02-28T10:08:07",
                    "TivityHealthLocationID": "26557",
                    "ReferenceID": null
                },
                "EligibilityStatus": "EligibilityStatusNotFound",
                "Reason": "Eligibility Status Reason Not Found"
            }
        ],
        "MemberCounts": {
            "TotalNumberOfActivitiesReceived": 6,
            "CountOfIneligibleMembers": 6,
            "CountOfEligibleMembers": 0,
            "CountOfPendingMembers": 0
        },
        "BatchStatus": "Processed",
        "InvalidInputs": []
    },
    "APIStatus": "Success"
}
     
     */
    public class GetProcessedActivitiesResponse
    {
        public GetProcessedActivitiesRequest Request{ get; set; }
        public ProcessedActivityResponse Response { get; set; }


        public String APIStatus { get; set; }

        
    }

    public class MembersCountResponse
    {
        public int TotalNumberOfActivitiesReceived {get; set;}
        public int CountOfIneligibleMembers {get; set;}
        public int CountOfEligibleMembers {get; set;}
        public int CountOfPendingMembers {get; set;}

    }
    public class ProcessedActivityResponse
    {
        public List<ActivityStatus> MemberActivities { get; set; }
        public MembersCountResponse MemberCounts { get; set; }
        public String BatchStatus { get; set; }
        //public List<String> InvalidInputs {get; set;}
    }
}
