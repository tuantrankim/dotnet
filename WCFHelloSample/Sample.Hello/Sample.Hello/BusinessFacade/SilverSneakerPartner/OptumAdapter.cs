using Sample.Hello.BusinessFacade.SilverSneakerPartner.Optum.Models;
using Sample.Hello.Common.Data;
using Sample.Hello.Common.Enumeration;
using Sample.Hello.Properties;
using Sample.Helper;
using Sample.MemberManagement.Common.Enumeration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner
{
    class OptumAdapter:SilverSneakerPartnerBase
    {
        public string PartnerBaseURL { get; private set; }
        public OptumAdapter(SilverSneakerType silverSneakerType) 
            : base(silverSneakerType, "Optum", "") {
                if (silverSneakerType == SilverSneakerType.FitnessAdvantage)
                {
                    Product = "Fitness Advantage";
                    PartnerBaseURL = Settings.Default.Optum_FitnessAdvantageMA_BaseUrl;
                }
                else if (silverSneakerType == SilverSneakerType.Bewell)
                {
                    Product = "Be Well";
                    PartnerBaseURL = Settings.Default.Optum_PWRFitnessMS_BaseUrl;
                }
                else throw new Exception(string.Format("Invalid Optum silver sneaker type [{0}]", silverSneakerType.ToString()));
        }
        public override SilverSneakerMembership VerifyFromPartner(int changeSourceId, Common.Data.SilverSneakerRequest silverSneakerRequest, out SilverSneakerResult silverSneakerResult)
        {
            SilverSneakerMembership ssm = null;
            string errorMessage = "";
            bool corporateIDInUse = false;
            string comments = string.Empty;
            int statusID = 0;
            string defaultErrorMessage =
                "Per Optum, guest cannot enroll\r\n"
                + "at this time and needs to call \r\n"
                + "Optum customer service.";

            //  Step 01:    Access Data from Optum
            // validating

            if (
                (SSType == SilverSneakerType.Bewell && !silverSneakerRequest.MembershipID.ToUpper().StartsWith("S"))
                || (SSType == SilverSneakerType.FitnessAdvantage && !silverSneakerRequest.MembershipID.ToUpper().StartsWith("A"))
                )
            {
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = "Invalid Optum Membership ID",
                    ErrorCode = ErrorCodes.InvalidMembershipID
                };
                return null;
            }

            DateTime newAPI_EffectiveDate = DateTime.MaxValue;

            foreach (Configuration c in HelloCache.Instance.Configurations)
            {
                if (SSType == SilverSneakerType.Bewell && c.TypeID == (int)Common.Enumeration.ConfigurationTypes.Optum_Medicare_Supplement_Program_Effective_Date)
                {
                    DateTime value = DateTime.MaxValue;
                    if (DateTime.TryParse(c.ConfigValue, out value)) newAPI_EffectiveDate = value;
                }
                else if (SSType == SilverSneakerType.FitnessAdvantage && c.TypeID == (int)Common.Enumeration.ConfigurationTypes.Optum_Medicare_Advantage_Program_Effective_Date)
                {
                    DateTime value = DateTime.MaxValue;
                    if (DateTime.TryParse(c.ConfigValue, out value)) newAPI_EffectiveDate = value;
                }
            }

            if (DateTime.Now < newAPI_EffectiveDate)
            {
                return OldAPI_VerifyFromPartner(changeSourceId, silverSneakerRequest, out silverSneakerResult);
            }

            //////////////////////////////////////
            /////////    New Optum API    ////////
            //////////////////////////////////////


            SilverSneakerBillingActivityResult result = new SilverSneakerBillingActivityResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };


            Optum.Service OptumAuthService = new Optum.Service(Settings.Default.OptumAuth_BaseUrl);
            Optum.Models.OAuthRequest tokenRequest = new Optum.Models.OAuthRequest()
            {
                client_id = Settings.Default.OptumClientId,
                client_secret = Settings.Default.OptumClientSecret,
                grant_type = "client_credentials",
            };

            OAuthResponse tokenResponse = OptumAuthService.GetToken(changeSourceId, tokenRequest).GetAwaiter().GetResult();


            if (tokenResponse == null || tokenResponse.access_token == "")
            {
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = "Authentication failed from Optum service.",
                    ErrorCode = ErrorCodes.Exception
                };
                LogHelper.Log("Sample.Hello.Exception", "Optum GetToken fail.");
                return null;
            }

            String accessToken = tokenResponse.access_token;
            Optum.Service optumService = new Optum.Service(Settings.Default.Optum_BaseUrl, BaseService.Authentication.OAuth, accessToken);

            Optum.Models.MemberApiRequest requestMember = null;
            requestMember = new Optum.Models.MemberApiRequest()
            {
                confirmationId = silverSneakerRequest.MembershipID
            };

            string requestJson = JsonConvert.SerializeObject(requestMember, Formatting.Indented);
            LogHelper.Log("Sample.Hello.OptumAdapter", "Optum GetMembersAsync Request: " + requestJson);

            Optum.Models.MemberApiResponse responseMember = optumService.GetMembersAsync(changeSourceId, requestMember).GetAwaiter().GetResult();
            //  Step 01:    Invalid data or not eligible member               
            string logJson = JsonConvert.SerializeObject(responseMember, Formatting.Indented);
            LogHelper.Log("Sample.Hello.OptumAdapter", "Optum GetMembersAsync Response: " + logJson);

            string logMessage = "VerifyFromPartner. (ChangeSourceId = " + changeSourceId + "). "
                + "{MESSAGE}. JSON = " + logJson;

            if (responseMember == null)
            {
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = defaultErrorMessage,
                    ErrorCode = ErrorCodes.Exception
                };
                return null;
            }

            if (responseMember.error)
            {
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromPartner,
                    MessageFromLAF = defaultErrorMessage,
                    MessageFromPartner = "",
                    ErrorCode = ErrorCodes.Exception
                };
                LogMessage(logMessage, "Error from Optum service");
                return null;
            }

            var optumMember = responseMember;

            silverSneakerRequest.MembershipID = responseMember.confirmationId;

            corporateIDInUse = DataAccess.SilverSneaker.CorporateIDExists(ref silverSneakerRequest, ref comments, ref statusID);
            bool statusEligibleToReinstate = (statusID == (int)CustomerStatuses.Cancelled && !string.IsNullOrEmpty(comments));

            //  Step 03:    Build Data Object
            ssm = new SilverSneakerMembership();

            ssm.CorporateID = responseMember.confirmationId;
            ssm.FirstName = responseMember.firstName;
            ssm.LastName = responseMember.lastName;

            DateTime dtmEffective = DateTime.MinValue;
            ssm.EffectiveDate = dtmEffective;

            DateTime dtmTermination = DateTime.MaxValue;
            ssm.TermDate = dtmTermination;

            ssm.Status = string.Empty;
            ssm.Product = Product;
            ssm.Zipcode = responseMember.zipcode;
            ssm.EmergencyFirstName = string.Empty;
            ssm.EmergencyLastName = string.Empty;
            ssm.HomePhone = string.Empty;
            ssm.CellPhone = string.Empty;
            ssm.WorkPhone = string.Empty;
            ssm.MembershipTypeID = silverSneakerRequest.MembershipTypeID;

            // load data when eligible to reinstate
            if (corporateIDInUse)
            {
                ssm.CustomerID = silverSneakerRequest.CustomerID;
                ssm.EmergencyFirstName = silverSneakerRequest.EmergencyContactFirstName;
                ssm.EmergencyLastName = silverSneakerRequest.EmergencyContactLastName;
                ssm.EmergencyPhone = silverSneakerRequest.EmergencyPhone;
                ssm.Email = silverSneakerRequest.Email;
                ssm.Zipcode = silverSneakerRequest.ZipCode;
                ssm.HomePhone = silverSneakerRequest.HomePhone;
                ssm.CellPhone = silverSneakerRequest.CellPhone;
                ssm.WorkPhone = silverSneakerRequest.WorkPhone;
            }

            //  Step 05:    End date validation
            if (ssm.TermDate.Date < DateTime.Today)
            {
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = defaultErrorMessage,
                    ErrorCode = ErrorCodes.TermedMember,
                };
                return null;
            }

            //Validate response result
            //if (
            //    (this.SSType == SilverSneakerType.FitnessAdvantage && !(responseMember.site == "A" && responseMember.programIdentifier == "MA"))
            //    || (this.SSType == SilverSneakerType.Bewell && !(responseMember.site == "S" && responseMember.programIdentifier == "MS"))
            //    )
            //{
            //    string json = JsonConvert.SerializeObject(responseMember, Formatting.Indented);
            //    string message = "VerifySilverSneakerMember. (ChangeSourceId = " + changeSourceId + "). "
            //        + "Site or Program mismatch. JSON = " + json;
                
            //    LogMessage(logMessage, "Site or Program mismatch");

            //    silverSneakerResult = new SilverSneakerResult
            //    {
            //        ErrorType = SilverSneakerErrorTypes.IneligibleFromPartner,
            //        MessageFromLAF = defaultErrorMessage,
            //        MessageFromPartner = message,
            //        ErrorCode = ErrorCodes.IncorectProgram
            //    };
            //    return ssm;
            //}

            if (!responseMember.eligible)
            {
                string json = JsonConvert.SerializeObject(responseMember, Formatting.Indented);
                string message = "VerifySilverSneakerMember. (ChangeSourceId = " + changeSourceId + ", SilversneakerType = " + SSType.ToString() + "). "
                    + "Ineligible when verifying an Optum member from partner service. JSON = " + json;
                LogMessage(logMessage, "Ineligible when verifying an Optum member from partner service");

                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.IneligibleFromPartner,
                    MessageFromLAF = defaultErrorMessage,
                    MessageFromPartner = message,
                    ErrorCode = ErrorCodes.Ineligible
                };
                return ssm;
            }

            if (corporateIDInUse && statusID != (int)CustomerStatuses.Cancelled)
            {
                errorMessage = "Member is already in our system.\r\nPlease use LAF keytag or mobile barcode.";
                LogMessage(logMessage, "Member is already in our system");
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromPartner,
                    MessageFromLAF = errorMessage,
                    ErrorCode = ErrorCodes.ExistingMember
                };
                return ssm;
            }

            // Exists in our system in Cancelled status, returns valid customer ID and if Optum returns status as not eligible, set customerID = 0 and return
            if (corporateIDInUse && statusID == (int)CustomerStatuses.Cancelled && string.IsNullOrEmpty(comments))
            {
                errorMessage = Product + " ID is not eligible to reinstate.\r\n"
                                   + "Guest needs to call the " + Company + "\r\n"
                                   + "customer service.";

                errorMessage = "Member is already in our system.\r\nPlease use LAF keytag or mobile barcode.";
                LogMessage(logMessage, "Member with cancelled status is already in our system");
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromPartner,
                    MessageFromLAF = errorMessage,
                    ErrorCode = ErrorCodes.ExistingMember
                };
                return ssm;
            }

            // load data when eligible to reinstate
            if (statusEligibleToReinstate)
            {
                errorMessage = "Member is eligible to be reinstated";
                LogMessage(logMessage, "Member is eligible to be reinstated");
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = errorMessage,
                    ErrorCode = ErrorCodes.ExistingMemberCanReinstate
                };
                return ssm;
            }

            LogMessage(logMessage, "Verify successfully");
            silverSneakerResult = new SilverSneakerResult
            {
                ErrorType = SilverSneakerErrorTypes.Success
            };

            if (ssm.Zipcode.Length > 5) ssm.Zipcode = ssm.Zipcode.Substring(0, 5);
            return ssm;
        }
        
        public SilverSneakerMembership OldAPI_VerifyFromPartner(int changeSourceId, Common.Data.SilverSneakerRequest silverSneakerRequest, out SilverSneakerResult silverSneakerResult)
        {
            SilverSneakerMembership ssm = null;
            string errorMessage = "";
            bool corporateIDInUse = false;
            string comments = string.Empty;
            int statusID = 0;
            string defaultErrorMessage =
                "Per Optum, guest cannot enroll\r\n"
                + "at this time and needs to call \r\n"
                + "Optum customer service.";

            //  Step 01:    Access Data from Optum
            // validating
            
            if (
                (SSType == SilverSneakerType.Bewell &&  !silverSneakerRequest.MembershipID.ToUpper().StartsWith("S"))
                || (SSType == SilverSneakerType.FitnessAdvantage && !silverSneakerRequest.MembershipID.ToUpper().StartsWith("A"))
                )
            {
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = "Invalid Optum Membership ID",
                    ErrorCode = ErrorCodes.InvalidMembershipID
                };
                return null;
            }

            Optum.Service OptumService = new Optum.Service(PartnerBaseURL);
            Optum.Models.Request requestMember = null;

            requestMember = new Optum.Models.Request()
            {
                member_id = new List<string>() { silverSneakerRequest.MembershipID }
            };

            Optum.Models.MemberResult responseMember = OptumService.VerifyMemberAsync(changeSourceId, Settings.Default.Optum_API_Key, requestMember).GetAwaiter().GetResult();
            //  Step 01:    Invalid data or not eligible member               

            if (responseMember == null)
            {
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = defaultErrorMessage,
                    ErrorCode = ErrorCodes.Exception
                };
                return null;
            }

            string logJson = JsonConvert.SerializeObject(responseMember, Formatting.Indented);

            string logMessage = "OldAPI_VerifyFromPartner. (ChangeSourceId = " + changeSourceId + "). "
                + "{MESSAGE}. JSON = " + logJson;

            if (responseMember.error)
            {
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromPartner,
                    MessageFromLAF = defaultErrorMessage,
                    MessageFromPartner = responseMember.error_message,
                    ErrorCode = ErrorCodes.Exception
                };
                LogMessage(logMessage, "Error from Optum service");
                return null;
            }
            var optumMember = responseMember.result.First();

            silverSneakerRequest.MembershipID = responseMember.result.First().member_id;

            corporateIDInUse = DataAccess.SilverSneaker.CorporateIDExists(ref silverSneakerRequest, ref comments, ref statusID);
            bool statusEligibleToReinstate = (statusID == (int)CustomerStatuses.Cancelled && !string.IsNullOrEmpty(comments));

            //  Step 03:    Build Data Object
            ssm = new SilverSneakerMembership();

            ssm.CorporateID = responseMember.result.First().member_id;
            ssm.FirstName = responseMember.result.First().first_name;
            ssm.LastName = responseMember.result.First().last_name;

            DateTime dtmEffective = DateTime.MinValue;
            ssm.EffectiveDate = dtmEffective;

            DateTime dtmTermination = DateTime.MaxValue;
            ssm.TermDate = dtmTermination;

            ssm.Status = string.Empty;
            ssm.Product = Product;
            ssm.Zipcode = responseMember.result.First().zipcode;
            ssm.EmergencyFirstName = string.Empty;
            ssm.EmergencyLastName = string.Empty;
            ssm.HomePhone = string.Empty;
            ssm.CellPhone = string.Empty;
            ssm.WorkPhone = string.Empty;
            ssm.MembershipTypeID = silverSneakerRequest.MembershipTypeID;

            // load data when eligible to reinstate
            if (corporateIDInUse)
            {
                ssm.CustomerID = silverSneakerRequest.CustomerID;
                ssm.EmergencyFirstName = silverSneakerRequest.EmergencyContactFirstName;
                ssm.EmergencyLastName = silverSneakerRequest.EmergencyContactLastName;
                ssm.EmergencyPhone = silverSneakerRequest.EmergencyPhone;
                ssm.Email = silverSneakerRequest.Email;
                ssm.Zipcode = silverSneakerRequest.ZipCode;
                ssm.HomePhone = silverSneakerRequest.HomePhone;
                ssm.CellPhone = silverSneakerRequest.CellPhone;
                ssm.WorkPhone = silverSneakerRequest.WorkPhone;
            }

            //  Step 05:    End date validation
            if (ssm.TermDate.Date < DateTime.Today)
            {
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = defaultErrorMessage,
                    ErrorCode = ErrorCodes.TermedMember,
                };
                return null;
            }

            //Validate response result
            //if (
            //    (this.SSType == SilverSneakerType.FitnessAdvantage && !(optumMember.site == "A" && optumMember.program_identifier == "MA"))
            //    || (this.SSType == SilverSneakerType.Bewell && !(optumMember.site == "S" && optumMember.program_identifier == "MS"))
            //    )
            //{
            //    string json = JsonConvert.SerializeObject(responseMember, Formatting.Indented);
            //    string message = "VerifySilverSneakerMember. (ChangeSourceId = " + changeSourceId + "). "
            //        + "Site or Program mismatch. JSON = " + json;

            //    LogMessage(logMessage,"Site or Program mismatch");

            //    silverSneakerResult = new SilverSneakerResult
            //    {
            //        ErrorType = SilverSneakerErrorTypes.IneligibleFromPartner,
            //        MessageFromLAF = defaultErrorMessage,
            //        MessageFromPartner = message,
            //        ErrorCode = ErrorCodes.IncorectProgram
            //    };
            //    return ssm;
            //}

            if (!optumMember.eligible              
                )
            {                
                string json = JsonConvert.SerializeObject(responseMember, Formatting.Indented);
                string message = "VerifySilverSneakerMember. (ChangeSourceId = " + changeSourceId + ", SilversneakerType = " + SSType.ToString() + "). "
                    + "Ineligible when verifying an Optum member from partner service. JSON = " + json;
                LogMessage(logMessage, "Ineligible when verifying an Optum member from partner service");

                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.IneligibleFromPartner,
                    MessageFromLAF = defaultErrorMessage,
                    MessageFromPartner = message,
                    ErrorCode = ErrorCodes.Ineligible
                };
                return ssm;
            }

            if (corporateIDInUse && statusID != (int)CustomerStatuses.Cancelled)
            {
                errorMessage = "Member is already in our system.\r\nPlease use LAF keytag or mobile barcode.";
                LogMessage(logMessage, "Member is already in our system");
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromPartner,
                    MessageFromLAF = errorMessage,
                    ErrorCode = ErrorCodes.ExistingMember
                };
                return ssm;
            }

            // Exists in our system in Cancelled status, returns valid customer ID and if Optum returns status as not eligible, set customerID = 0 and return
            if (corporateIDInUse && statusID == (int)CustomerStatuses.Cancelled && string.IsNullOrEmpty(comments))
            {
                LogMessage(logMessage, "Member with cancelled status is already in our system");

                errorMessage = "Member is already in our system.\r\nPlease use LAF keytag or mobile barcode.";
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromPartner,
                    MessageFromLAF = errorMessage,
                    ErrorCode = ErrorCodes.ExistingMember
                };
                return ssm;
            }

            // load data when eligible to reinstate
            if (statusEligibleToReinstate)
            {
                errorMessage = "Member is eligible to be reinstated";
                LogMessage(logMessage, "Member is eligible to be reinstated");
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = errorMessage,
                    ErrorCode = ErrorCodes.ExistingMemberCanReinstate
                };
                return ssm;
            }

            LogMessage(logMessage, "Verify successfully.");
            silverSneakerResult = new SilverSneakerResult
            {
                ErrorType = SilverSneakerErrorTypes.Success
            };            
            return ssm;
        }
        private string LogMessage(string format, string message)
        {
            string result = format.Replace("{MESSAGE}", message);
            LogHelper.Log("Sample.Hello.Exception", result);
            return result;
        }
        public override void EnrollFromPartner(int changeSourceId, ref Common.Data.SilverSneakerMembership ssm, out SilverSneakerResult silverSneakerResult)
        {
            //SUCCESS
            silverSneakerResult = new SilverSneakerResult
            {
                ErrorType = SilverSneakerErrorTypes.Success,
            };
        }
        public override SilverSneakerCancelResult CancelSilverSneakerMembers(int changeSourceId)
        {
            SilverSneakerCancelResult result = new SilverSneakerCancelResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };
            if (this.SSType == SilverSneakerType.FitnessAdvantage)
            {

                Guid processID;
                List<Common.Data.SilverSneakerCancelProcessStaging> cancellingMembers;
                List<Common.Data.SilverSneakerCancelProcessStaging> reinstatingMembers;
                PrepareCancelProcessStaging(changeSourceId, out processID, out cancellingMembers, out reinstatingMembers);

                // Do cancellation
                result = CancelFitnessAdvantage(changeSourceId, processID, cancellingMembers);

                // Do reinstation
                var reinstateResult = ReinstateFitnessAdvantage(changeSourceId, processID, reinstatingMembers);
            }
            else if (this.SSType == SilverSneakerType.Bewell)
            {
                Guid processID;
                List<Common.Data.SilverSneakerCancelProcessStaging> cancellingMembers;
                List<Common.Data.SilverSneakerCancelProcessStaging> reinstatingMembers;
                PrepareCancelProcessStaging(changeSourceId, out processID, out cancellingMembers, out reinstatingMembers);

                // Do cancellation
                result = CancelBewell(changeSourceId, processID, cancellingMembers);

                // No reinstate for bewell
            }

            else
            {
                result.ErrorMessage = "Optum service does not support silver sneaker type " + SSType.ToString();
            }

            return result;
        }

        private void PrepareCancelProcessStaging(int changeSourceId, out Guid processID,
            out List<Common.Data.SilverSneakerCancelProcessStaging> cancellingMembers,
            out List<Common.Data.SilverSneakerCancelProcessStaging> reinstatingMembers)
        {
            //  STEP 01: Copy Data to Staging table and set process ID
            DataAccess.SilverSneaker.Copy_SilverSneakerMembers_To_StagingTable(out processID, this.SSType);

            int top = 50;
            int threadID = 1;
            List<Common.Data.SilverSneakerCancelProcessStaging> subset = null;
            cancellingMembers = new List<SilverSneakerCancelProcessStaging>();
            reinstatingMembers = new List<SilverSneakerCancelProcessStaging>();

            do
            {
                subset = DataAccess.SilverSneaker.SilverSneakers_CancelProcess_Staging_Select_Top(processID, threadID, top);
                if (subset == null || subset.Count() == 0) break;
                try
                {
                    //Verify from OPTUM
                    Optum.Service OptumService = new Optum.Service(PartnerBaseURL);
                    Optum.Models.Request requestMember = null;

                    var memberIDs = subset.Select(i => i.CorporateID).ToList<string>();
                    requestMember = new Optum.Models.Request()
                    {
                        member_id = memberIDs
                    };

                    Optum.Models.MemberResult responseMember = OptumService.VerifyMemberAsync(changeSourceId, Settings.Default.Optum_API_Key, requestMember).GetAwaiter().GetResult();

                    if (responseMember == null) break;
                    if (responseMember.error) break;
                    //var results = responseMember.result.ToDictionary(r=> r.member_id, r => r.eligible);
                    //Tolookup allows duplicated lookup key in case of multiple brand
                    var responseResults = responseMember.result.ToLookup(r => r.member_id.ToUpper(), r => r);

                    //Update subset 
                    foreach (var ssm in subset)
                    {
                        var responseItem = responseResults[ssm.CorporateID.ToUpper()].FirstOrDefault();
                        //bool isEligible = responseResults[ssm.CorporateID.ToUpper()].First();
                        if (responseItem == null) ssm.NewStatusID = null;
                        else
                        {
                            bool isEligible = responseItem.eligible;
                            ssm.ProductName = SSType.ToString();
                            ssm.NewHwayCardNumber = responseItem.member_id;
                            ssm.HwayStatus = isEligible ? "Eligible" : "Ineligible";
                            //ssm.EffectiveDate =
                            //ssm.TerminationDate =
                            ssm.ModifiedDate = DateTime.Now;
                            bool isCancelled = (ssm.CurrentStatusID == 4 || ssm.CurrentStatusID == 19 || ssm.CurrentStatusID == 10);
                            if (!isCancelled && !isEligible)
                            {
                                ssm.NewStatusID = 4;
                                cancellingMembers.Add(ssm);
                            }
                            else if (ssm.CurrentStatusID == 4 && isEligible)
                            {
                                ssm.NewStatusID = 1;
                                reinstatingMembers.Add(ssm);
                            }
                            else ssm.NewStatusID = null;
                        }
                    }
                    //Only update the change
                    //var changedSubset = subset.Where(s => s.NewStatusID != null).ToList();

                    //Update staging table
                    var dtChangedSubset = Converter.ToSilverSneakerCancelProcessStaging_DataTable(subset);
                    DataAccess.SilverSneaker.SilverSneakers_CancelProcess_Staging_Update(dtChangedSubset);

                }
                catch (Exception ex)
                {
                    try { LogHelper.LogError("Sample.Hello.Exception", ex, false, System.Reflection.MethodInfo.GetCurrentMethod(), processID); }
                    catch { }
                }

                threadID++;

            } while (subset != null && subset.Count() > 0);
        }

        private SilverSneakerCancelResult CancelFitnessAdvantage(int changeSourceId, Guid processID, List<Common.Data.SilverSneakerCancelProcessStaging> cancellingMembers)
        {
            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + " member cancellation",
                string.Format("Start.{0} members are going to be cancelled", cancellingMembers.Count));

            SilverSneakerCancelResult result = new SilverSneakerCancelResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };
            foreach (var member in cancellingMembers)
            {
                try
                {
                    int customerID = member.LAFCustomerID;
                    if (customerID <= 0)
                    {
                        result.SkipCount++;
                        continue;
                    }

                    Customer customer = HelloSystem.Instance.GetCustomer(customerID);
                    customer.SilverSneakerMembershipTypeID = (int)this.SSType;

                    if (customer != null
                        && !customer.Status.CurrentStatus.IsCancelledOrThree_SevenDayCancelled
                        && !customer.Status.CurrentStatus.IsRevokedDenyEntryOnly
                        && customer.Status.FutureCancelStatuses.Count == 0
                        )
                    {
                        int employeeID = Settings.Default.AutomatedEmployeeID;
                        DateTime currentDate = DateTime.Now.Date;
                        string statusReason = this.Company;
                        CancelReasons cancelReason = CancelReasons.NonUsage;  // from CancelReason table
                        int aphelionClubID = Settings.Default.CorporateClubAphelionID;
                        string operation = SSType.ToString() + " automated cancellation";
                        ChangeSource changeSource = (ChangeSource)changeSourceId; //application name
                        string comments = string.Format("Cancel from {0} Membership Cancellation on {1:G} from Corporate.", Product, DateTime.Now);
                        bool isSuccess = HelloSystem.Instance.ProcessSimpleMembershipCancel(customer, comments, (int)cancelReason, employeeID,
                            aphelionClubID, statusReason, DateTime.Now.Date, DateTime.Now.Date, operation, (int)changeSource);
                        if (isSuccess)
                        {
                            result.SuccessCount++;
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + "member cancellation",
                                string.Format("CorporateID: {0}, CustomerID: {1} - success", member.CorporateID, member.LAFCustomerID.ToString()));

                        }
                        else
                        {
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + "member cancellation",
                                string.Format("CorporateID: {0}, CustomerID: {1} - failed", member.CorporateID, member.LAFCustomerID.ToString()));
                            result.ErrorCount++;
                        }
                    }
                    else
                    {
                        result.SkipCount++;
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    int eventLogID = HelloSystem.Instance.Log(ex, string.Format("CorporateID: {0}, CustomerID: {1} - failed.", member.CorporateID, member.LAFCustomerID));
                    DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + " member cancellation",
                        string.Format("CorporateID: {0}, CustomerID: {1} - failed. EventLogID = {2}", member.CorporateID, member.LAFCustomerID, eventLogID));
                }
            }
            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + " member cancellation", string.Format("End. {0} received from {1}. {2} are succeed. {3} are failed. {4} are skipped.",
                cancellingMembers.Count, Product, result.SuccessCount, result.ErrorCount, result.SkipCount));
            
            return result;
        }


        private SilverSneakerCancelResult ReinstateFitnessAdvantage(int changeSourceId, Guid processID, List<Common.Data.SilverSneakerCancelProcessStaging> reinstatingMembers)
        {
            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + " member reinstating",
                string.Format("Start.{0} members are going to be reinstated", reinstatingMembers.Count));

            SilverSneakerCancelResult result = new SilverSneakerCancelResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };

            int employeeID = Settings.Default.AutomatedEmployeeID;
            string statusReason = this.Product;
            int aphelionClubID = Settings.Default.CorporateClubAphelionID;
            Club club = new Club(aphelionClubID, true);

            foreach (var member in reinstatingMembers)
            {
                try
                {
                    int customerID = member.LAFCustomerID;
                    if (customerID <= 0)
                    {
                        result.SkipCount++;
                        continue;
                    }

                    Customer cust = HelloSystem.Instance.GetCustomer(customerID);

                    if (cust != null 
                        && cust.Status.CurrentStatus.StatusID == CustomerStatuses.Cancelled 
                        && !string.IsNullOrEmpty(cust.Status.CurrentStatus.Comments)) 
                    {
                        DateTime currentDate = DateTime.Now.Date;
                        ChangeSource changeSource = (ChangeSource)changeSourceId; //application name
                        cust.SilverSneakerMembershipTypeID = (int)SSType; // should pass into ProcessUnCancelForHealthwaysMember as a parameter
                        Customer customerResult = HelloSystem.Instance.ProcessUnCancelForHealthwaysMember(cust, club, employeeID, string.Empty, string.Empty, string.Empty, string.Empty, changeSourceId, false);

                         bool isSuccess = customerResult != null;
                        if (isSuccess)
                        {
                            result.SuccessCount++;
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + "member reinstating",
                                string.Format("CorporateID: {0}, CustomerID: {1} - success", member.CorporateID, member.LAFCustomerID.ToString()));

                        }
                        else
                        {
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + "member reinstating",
                                string.Format("CorporateID: {0}, CustomerID: {1} - failed", member.CorporateID, member.LAFCustomerID.ToString()));
                            result.ErrorCount++;
                        }
                    }
                    else
                    {
                        result.SkipCount++;
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    int eventLogID = HelloSystem.Instance.Log(ex, string.Format("CorporateID: {0}, CustomerID: {1} - failed.", member.CorporateID, member.LAFCustomerID));
                    DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + " member reinstating",
                        string.Format("CorporateID: {0}, CustomerID: {1} - failed. EventLogID = {2}", member.CorporateID, member.LAFCustomerID, eventLogID));
                }
            }
            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + " member reinstating", string.Format("End. {0} received from {1}. {2} are succeed. {3} are failed. {4} are skipped.",
                reinstatingMembers.Count, Product, result.SuccessCount, result.ErrorCount, result.SkipCount));
            
            return result;
        }

        private SilverSneakerCancelResult CancelBewell(int changeSourceId, Guid processID, List<Common.Data.SilverSneakerCancelProcessStaging> cancellingMembers)
        {
            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + " member cancellation",
                string.Format("Start.{0} members are going to be cancelled", cancellingMembers.Count));

            SilverSneakerCancelResult result = new SilverSneakerCancelResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };
            DateTime currentDate = DateTime.Now.Date;
            string statusReason = this.Product;
            //CancelReasons cancelReason = CancelReasons.NonUsage;  // from CancelReason table
            ChangeSource changeSource = (ChangeSource)changeSourceId; //application name
            Applications correspondenceApp = Applications.Hello;
            CommunicationTypes communicationType = CommunicationTypes.HelloOther;
            string comments = string.Format("Cancel from {0} Membership Cancellation on {1:G} from Corporate.", Product, DateTime.Now);

            foreach (var member in cancellingMembers)
            {
                try
                {
                    int customerID = member.LAFCustomerID;
                    if (customerID <= 0)
                    {
                        result.SkipCount++;
                        continue;
                    }

                    Customer customer = HelloSystem.Instance.GetCustomer(customerID);
                    customer.SilverSneakerMembershipTypeID = (int)this.SSType;

                    if (customer != null 
                        && !customer.Status.CurrentStatus.IsCancelledOrThree_SevenDayCancelled
                        && !customer.Status.CurrentStatus.IsRevokedDenyEntryOnly
                        && customer.Status.FutureCancelStatuses.Count == 0
                        )
                    {
                        CancelMemberRequest request = new CancelMemberRequest()
                        {
                            Club = null,
                            Employee = null,
                            CancelReasonID = 0,
                            ChangeSourceId = changeSourceId,
                            ConfirmationType = ConfirmationTypes.Email,
                            CorrespondenceAppID = (int)correspondenceApp,
                            CorrespondenceCampaignAbbr = communicationType.ToString(),
                            Customer = customer,
                            PostMarkDate = DateTime.Today,
                            PostMarkDateType = PostMarkDateTypes.FormRequest,
                            StatusReason = this.Company,
                            StatusComment = comments,
                            EmailAddress = customer.Demographics.Email
                        };

                        bool isSuccess = HelloSystem.Instance.EFTQuickCancel(request);
                        if (isSuccess)
                        {
                            result.SuccessCount++;
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + "member cancellation",
                                string.Format("CorporateID: {0}, CustomerID: {1} - success", member.CorporateID, member.LAFCustomerID.ToString()));

                        }
                        else
                        {
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + "member cancellation",
                                string.Format("CorporateID: {0}, CustomerID: {1} - failed", member.CorporateID, member.LAFCustomerID.ToString()));
                            result.ErrorCount++;
                        }
                    }
                    else
                    {
                        result.SkipCount++;
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    int eventLogID = HelloSystem.Instance.Log(ex, string.Format("CorporateID: {0}, CustomerID: {1} - failed.", member.CorporateID, member.LAFCustomerID));
                    DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + " member cancellation",
                        string.Format("CorporateID: {0}, CustomerID: {1} - failed. EventLogID = {2}", member.CorporateID, member.LAFCustomerID, eventLogID));
                }
            }
             
            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, Product + " member cancellation", string.Format("End. {0} received from {1}. {2} are succeed. {3} are failed. {4} are skipped.",
                cancellingMembers.Count, Product, result.SuccessCount, result.ErrorCount, result.SkipCount));

            return result;
        }

        public override SilverSneakerBillingActivityResult SilverSneakerBillingActivity(int changeSourceId)
        {
            SilverSneakerBillingActivityResult result = new SilverSneakerBillingActivityResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };
            return result;
        }
        
    }
}