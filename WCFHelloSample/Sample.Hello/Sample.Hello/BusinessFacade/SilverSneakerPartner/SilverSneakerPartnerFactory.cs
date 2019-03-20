using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sample.Hello.Common.Enumeration;
using Sample.Hello.Common.Data;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner
{
    class SilverSneakerPartnerFactory
    {
        public static ISilverSneakerPartner GetSilverSneakerPartner(SilverSneakerType silverSneakerType)
        {
            if (silverSneakerType == SilverSneakerType.Healthways) return new HealthWayAdapter(silverSneakerType);
            else if (silverSneakerType == SilverSneakerType.ASH) return new ASHAdapter(silverSneakerType);
            else if (silverSneakerType == SilverSneakerType.FitnessAdvantage) return new OptumAdapter(silverSneakerType);
            else if (silverSneakerType == SilverSneakerType.Bewell) return new OptumAdapter(silverSneakerType);
            else return null;
        }
    }
}
