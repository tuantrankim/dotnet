using Sample.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.ASH
{
    public class Service : BaseService
    {
        //string baseUrl = "https://ashfitness.net/api";
        //string Action = "/verify";
        //string username = "lafituser";
        //string password = "-uwcc+#n#42$4jmj";

        //string First = "Fember01";
        //string Last = "test295581";
        //string BirthDate = "1/1/1950";
        //string ZipCode = "92101";

        /*
         //VerifyMemberAsync JSON return
          
           {
              "responseStatus": {
                "message": "Eligible"
              },
              "memberId": 29105902,
              "firstName": "MemberEighteen",
              "lastName": "Behealthy",
              "effectiveDate": "1/1/2015 12:00:00 AM",
              "terminationDate": "Open",
              "productName": "Silver&Fit",
              "emailAddress": "",
              "zipCode": "92101"
            }
         */
        /*
         
          
           {
              "responseStatus": {
                "message": "Eligible"
              },
              "memberId": 31957328,
              "firstName": "Member05",
              "lastName": "test316147",
              "effectiveDate": "1/1/2015 12:00:00 AM",
              "terminationDate": "Open",
              "productName": "Active&Fit",
              "emailAddress": "",
              "zipCode": "92101",
              "facilities": [
                {
                  "docId": 1187341,
                  "clinicId": 358337,
                  "facilityName": "LA Fitness - HAWTHORNE - 147TH ST.",
                  "facilityAddress": "4949 W 147th St Hawthorne CA 90250"
                }
              ]
            }
         */
        public Service(string baseUrl, string userName, string passWord) : base(baseUrl, Authentication.Basic, userName, passWord) { }
        public async Task<Models.Member> VerifyMemberAsync(int changeSourceId, Models.Member request)
        {
            string action = "verify";
            return await PostJsonAsync<Models.Member, Models.Member>(changeSourceId, action, request).ConfigureAwait(false);
        }
        /*
         //TermedMemberAsync JSON return
         [
          {
            "fitnessId": 23266535,
            "firstName": "LINDA",
            "lastName": "CARTER",
            "memberProgramName": "Silver&Fit",
            "effectiveDate": "/Date(1448956800000-0000)/",
            "formattedTerminationDate": "2015-12-31T23:59:00",
            "terminationDate": "/Date(1451635140000-0000)/"
          },
          {
            "fitnessId": 31552206,
            "firstName": "Helen",
            "lastName": "Keilman",
            "memberProgramName": "Silver&Fit",
            "effectiveDate": "/Date(1451635200000-0000)/",
            "formattedTerminationDate": "2016-04-30T23:59:00",
            "terminationDate": "/Date(1462085940000-0000)/"
          }
         ] 
         */
        public async Task<List<Models.TermedMember>> TermedMemberAsync(int changeSourceId)
        {
            string action = "termed";
            return await PostJsonAsync<List<Models.TermedMember>, Object>(changeSourceId, action, null).ConfigureAwait(false);
        }

        /*    
        {
            "FitnessId": "int",
            "DocId": "int",
            "ClinicId": "int"
        }
        
         E.g.:
         
        {
          "fitnessId": 31957329,
          "firstName": "Member06",
          "lastName": "test316147",
          "programEffectiveDate": "2016-10-11T16:24:27.0900000-07:00"
        }
          */
        public async Task<Models.EnrollMember> EnrollMemberAsync(int changeSourceId, Models.EnrollMember request)
        {
            string action = "enroll";
            return await PostJsonAsync<Models.EnrollMember, Models.EnrollMember>(changeSourceId, action, request).ConfigureAwait(false);
        }
    }
}
