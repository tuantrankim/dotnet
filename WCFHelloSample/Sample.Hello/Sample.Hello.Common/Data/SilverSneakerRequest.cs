using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Sample.Hello.Common.Data;
using Sample.Hello.Common.Enumeration;
using Sample.Hello.Common.Interface;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.Common.Data
{
    [DataContract]
    public class SilverSneakerRequest
    {

        #region Properties
        [DataMember]
        public string MembershipID { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string DateOfBirth { get; set; }

        [DataMember]
        public int CustomerID { get; set; }

        [DataMember]
        public string EmergencyContact { get; set; }

        [DataMember]
        public string EmergencyPhone { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string HomePhone { get; set; }

        [DataMember]
        public string CellPhone { get; set; }

        [DataMember]
        public string WorkPhone { get; set; }

        [DataMember]
        public int MembershipTypeID { get; set; }

        [DataMember]
        public int? BrandID { get; set; }

        [DataMember]
        public string Program { get; set; }

        public string Provider {
            get {
                string provider = "";
                switch (MembershipTypeID)
                {
                    case 1:
                        provider = "Tivity Health";
                        break;
                    case 2:
                        provider = "ASH";
                        break;
                    case 3:
                        provider = "Optum";
                        break;
                }

                return provider;
            }
        }
      

        [DataMember]
        public string FacilityID { get; set; }

        
        #endregion
    }
}
