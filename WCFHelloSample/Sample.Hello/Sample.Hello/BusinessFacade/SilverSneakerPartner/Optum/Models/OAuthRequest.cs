using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.Optum.Models
{
    /*
     {
        client_id:Sample.activityfeed
        client_secret:e1bec439-4c47-4c27-9ddb-682e40ffc839
        grant_type:client_credentials
        scope:insert_single insert_batch get_ineligible
     }
     */
    public class OAuthRequest
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
        public string scope { get; set; }
    }
}
