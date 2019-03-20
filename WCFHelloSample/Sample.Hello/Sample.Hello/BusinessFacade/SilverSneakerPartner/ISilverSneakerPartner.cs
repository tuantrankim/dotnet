using Sample.Hello.Common.Data;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner
{
    interface ISilverSneakerPartner
    {
        SilverSneakerMembership VerifyFromPartner(int changeSourceId, Common.Data.SilverSneakerRequest silverSneakerRequest, out SilverSneakerResult silverSneakerResult);
        void EnrollFromPartner(int changeSourceId, ref Common.Data.SilverSneakerMembership ssm, out SilverSneakerResult silverSneakerResult);

        SilverSneakerCancelResult CancelSilverSneakerMembers(int changeSourceId);
        SilverSneakerBillingActivityResult SilverSneakerBillingActivity(int changeSourceId);
    }
}
