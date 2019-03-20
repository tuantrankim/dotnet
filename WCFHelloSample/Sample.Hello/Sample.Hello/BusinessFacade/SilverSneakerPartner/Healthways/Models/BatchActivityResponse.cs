using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.Healthways.Models
{
    /*
        {
            "TotalNumberOfVisits": 6,
            "BatchGUID": "b2a1da96-7f6b-495a-a853-c289612dfbd2",
            "APIResponseStatus": "Success",
            "BatchStatus": "InProcess"
        }
     */
    public class BatchActivityResponse
    {
        public int TotalNumberOfVisits { get; set; }
        public string BatchGUID { get; set; }
        public string APIResponseStatus { get; set; }
        public string BatchStatus { get; set; }

    }
}
