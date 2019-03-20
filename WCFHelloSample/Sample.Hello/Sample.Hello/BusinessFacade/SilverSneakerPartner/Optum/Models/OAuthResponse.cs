using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.Optum.Models
{
    /*
     {
       access_token: e820940c-2459-48de-a4e6-435d378c8374,
       token_type: Bearer,
       expires_in: 3600,
       scope: oob
     }
     */
    public class OAuthResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }
}

