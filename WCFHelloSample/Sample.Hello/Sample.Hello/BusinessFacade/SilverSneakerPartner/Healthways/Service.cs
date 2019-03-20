using Sample.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.Healthways
{
    public class Service : BaseService
    {
        public Service(string baseUrl, Authentication authentication = Authentication.None, string oAuthAccessToken = "") : base(baseUrl, authentication, "","", oAuthAccessToken) { }

        public async Task<Models.OAuthResponse> GetToken(int changeSourceId, Models.OAuthRequest request)
        {
            string action = "token";
            return await PostAsync<Models.OAuthResponse, Models.OAuthRequest>(changeSourceId, action, request).ConfigureAwait(false);
        }

        public async Task<Models.BatchActivityResponse> SubmitBatchActivity(int changeSourceId, List<Models.Activity> request)
        {
            string action = "SubmitBatchActivity";
            return await PostJsonAsync<Models.BatchActivityResponse, List<Models.Activity>>(changeSourceId, action, request).ConfigureAwait(false);
        }

        //public async Task<List<Models.IneligibleMember>> GetIneligibleMembers(int changeSourceId, Models.GetIneligibleMembersRequest request)
        //{
        //    string action = "GetIneligibleMembers";
        //    return await GetAsync<List<Models.IneligibleMember>, Models.GetIneligibleMembersRequest>(changeSourceId, action, request).ConfigureAwait(false);
        //}

        public async Task<Models.GetProcessedActivitiesResponse> GetProcessedActivities(int changeSourceId, Models.GetProcessedActivitiesRequest request)
        {
            string action = "GetProcessedActivities";
            return await GetAsync<Models.GetProcessedActivitiesResponse, Models.GetProcessedActivitiesRequest>(changeSourceId, action, request).ConfigureAwait(false);
        }
        
    }
}
