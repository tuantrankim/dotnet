using Sample.Hello.Common.Data;
using Sample.Hello.Common.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner
{
    abstract class SilverSneakerPartnerBase:ISilverSneakerPartner
    {
        #region Member variables
        
        string product;
        string company;
        string defaultErrorMessage;
        SilverSneakerType silverSneakerType;

        #endregion Member variables

        #region Constructors
        public SilverSneakerPartnerBase() { }
        public SilverSneakerPartnerBase(SilverSneakerType silverSneakerType, string company, string product)
        {
            this.silverSneakerType = silverSneakerType;
            this.product = product;
            this.company = company;
        }

        #endregion Constructors

        #region Properties
        public string Product {
            get{return product;}
            set{product = value;}
        }
        public string Company {
            get { return company; }
            set { company = value; }
        }

        public SilverSneakerType SSType {
            get { return silverSneakerType; }
            set { silverSneakerType = value; }
        }
        public string DefaultErrorMessage
        {
            get {
                return company + " was unable to verify eligibility.\r\n"
                                                                                    + "Guest needs to call the " + company + "\r\n"
                                                                                    + "customer service number \r\n"
                                                                                    + "located on the back of their ID card.";
            }
        }
        
        #endregion Properties
        
        #region Abstract methods
        public abstract SilverSneakerMembership VerifyFromPartner(int changeSourceId, Common.Data.SilverSneakerRequest silverSneakerRequest, out SilverSneakerResult silverSneakerResult);
        public abstract void EnrollFromPartner(int changeSourceId, ref Common.Data.SilverSneakerMembership ssm, out SilverSneakerResult silverSneakerResult);

        public abstract SilverSneakerCancelResult CancelSilverSneakerMembers(int changeSourceId);
        public abstract SilverSneakerBillingActivityResult SilverSneakerBillingActivity(int changeSourceId);

        #endregion
    }
}
