using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.Optum.Models
{
    public class Result
    {
        public string member_id { get; set; }
        public bool eligible { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string zipcode { get; set; }
        public string site { get; set; }
        public string program_identifier { get; set; }
    }
}
