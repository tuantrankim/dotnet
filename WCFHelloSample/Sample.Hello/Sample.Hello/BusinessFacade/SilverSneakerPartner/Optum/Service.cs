using Sample.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.Optum
{
    public class Service : BaseService
    {
        //string baseUrl = "https://qa.pwrfitness.com/api/partner";
        //string api_key = "74c6714291ac722884cf053d70caab8a40907ef8";

        //string memberID = "{ \"member_id\": [\"S000000003\"] }";

        /*
         //VerifyMemberAsync JSON return
          
            (Eligible Member)
            {  
               "error":false,
               "result":[  
                  {  
                     "member_id":"S000000003",
                     "first_name":"Bernard",
                     "last_name":"Wheeler",
                     "zipcode":21801,
                     "site":"S",
                     "program_identifier":"MS",
                     "eligible":true
                  }
               ]
            }

            (Ineligible Member)
            {  
               "error":false,
               "result":[  
                  {  
                     "member_id":"A000000003",
                     "eligible":false
                  }
               ]
            }

            (Error returned from partner service)
            {  
               "error":true,
               "error_message":"Bad API key"
            }
         */

        public Service(string baseUrl) : base(baseUrl, Authentication.None) { }
        public Service(string baseUrl, Authentication authentication = Authentication.None, string oAuthAccessToken = "") : base(baseUrl, authentication, "", "", oAuthAccessToken) { }
        public async Task<Models.MemberResult> VerifyMemberAsync(int changeSourceId, string apiKey, Models.Request request)
        {
            string action = @"/partner?api_key=" + apiKey;
            return await PostJsonAsync<Models.MemberResult, Models.Request>(changeSourceId, action, request).ConfigureAwait(false);
        }


        public async Task<Models.OAuthResponse> GetToken(int changeSourceId, Models.OAuthRequest request)
        {
            string action = "token";
            return await PostAsync<Models.OAuthResponse, Models.OAuthRequest>(changeSourceId, action, request).ConfigureAwait(false);
        }

        //public async Task<Models.BatchActivityResponse> SubmitBatchActivity(int changeSourceId, List<Models.Activity> request)
        //{
        //    string action = "SubmitBatchActivity";
        //    return await PostJsonAsync<Models.BatchActivityResponse, List<Models.Activity>>(changeSourceId, action, request).ConfigureAwait(false);
        //}

        //public async Task<Models.MemberApiResponse> GetMembersAsync(int changeSourceId, Models.MemberApiRequest request)
        //{
        //    string action = "";
        //    return await PostJsonAsync<Models.MemberApiResponse, Models.MemberApiRequest>(changeSourceId, action, request).ConfigureAwait(false);
        //}
        public async Task<Models.MemberApiResponse> GetMembersAsync(int changeSourceId, Models.MemberApiRequest request)
        {
            string action = request.confirmationId;
            return await GetAsync<Models.MemberApiResponse, Models.MemberApiRequest>(changeSourceId, action, null).ConfigureAwait(false);
        }

        //public async Task<Models.GetProcessedActivitiesResponse> GetProcessedActivities(int changeSourceId, Models.GetProcessedActivitiesRequest request)
        //{
        //    string action = "GetProcessedActivities";
        //    return await GetAsync<Models.GetProcessedActivitiesResponse, Models.GetProcessedActivitiesRequest>(changeSourceId, action, request).ConfigureAwait(false);
        //}

    }
}
