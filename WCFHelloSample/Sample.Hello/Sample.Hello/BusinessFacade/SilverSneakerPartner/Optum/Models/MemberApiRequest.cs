using Sample.Hello.BusinessFacade.SilverSneakerPartner.ASH.Models;
using Sample.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner.Optum.Models
{
    /*
     Sample JSON Request
        {
        "confirmationId ":"S000001234"	
        }
      
     MemberApiResponseObject:
     Sample JSON Response – Member Eligible
        {
	        “confirmationId”:”S00001234”,
	        “firstName”:”LINDA”,
	        “lastName”:”RICHARDS”,
	        “programIdentifier”:”MS”,
	        “site”:”S”,
	        “zipcode”:”98026”,
	        “eligible”:true,
	        “error”:false
        }
     
     Sample JSON Response – Member Not Found.
        {
	        “eligible”:false,
	        “error”:true,
	        “errorCode”:”150”
	        “exceptionMessage”:” Member Not Found”
        }
     */
    public class MemberApiRequest
    {
	        public string confirmationId {get;set;}
    }
}
