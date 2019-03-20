using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.Healthways.Models
{
    /*
     {
        "access_token": "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IkdfUzNBMjlHa3lUTkN0TUpTVDhuRVl3czZWYyIsImtpZCI6IkdfUzNBMjlHa3lUTkN0TUpTVDhuRVl3czZWYyJ9.eyJpc3MiOiJodHRwczovL2FwaWF1dGh1YXQudGl2aXR5aGVhbHRoLmNvbS9jb3JlIiwiYXVkIjoiaHR0cHM6Ly9hcGlhdXRodWF0LnRpdml0eWhlYWx0aC5jb20vY29yZS9yZXNvdXJjZXMiLCJleHAiOjE1MTI0NjgwNDMsIm5iZiI6MTUxMjQyNDg0MywiY2xpZW50X2lkIjoibGFmaXRuZXNzLmFjdGl2aXR5ZmVlZCIsInNjb3BlIjpbImdldF9pbmVsaWdpYmxlIiwiaW5zZXJ0X2JhdGNoIiwiaW5zZXJ0X3NpbmdsZSJdfQ.KO64uvkwcl-9St-v6Yfo_td7ahowniiG7aYudhZR35-tfVWvb0rZV08nmERC67s3do2mRGOHphq-Af6DLbuTE9rrzNdP1ZycjSR_lmTSg7TEO2Awph96WKgucVn7yltDDJ2N1Oo36vypQEAceyCDxx4TNXLnBiCKHAn5llI8qJYtiE2vMDES7Zw51FioQv-OsHIY1rjxZpmrZwzC3iy902f-j4L1o2bWWRId1i4UZ_d7-jkqRLiGN2BGQG2depijoHDMCMdDPOj9uO5aEZ8sucs4M3nrxY36wBa7mJtuRbC1J3YfySjVdwPlch4q7GgjL9Dz61dOQw5hnOW1GgpBSg",
        "expires_in": 43200,
        "token_type": "Bearer"
        }
     */
    public class OAuthResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
}

