using Sample.Hello.BusinessFacade.SilverSneakerPartner.ASH.Models;
using Sample.Hello.Common.Data;
using Sample.Hello.Common.Enumeration;
using Sample.Hello.Properties;
using Sample.Helper;
using Sample.MemberManagement.Common.Enumeration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade.SilverSneakerPartner
{
    class ASHAdapter : SilverSneakerPartnerBase
    {
        public ASHAdapter(SilverSneakerType silverSneakerType)
            : base(silverSneakerType, "American Specialty Health", "Silver & Fit") { }
        public override SilverSneakerMembership VerifyFromPartner(int changeSourceId, Common.Data.SilverSneakerRequest silverSneakerRequest, out SilverSneakerResult silverSneakerResult)
        {
            SilverSneakerMembership ssm = null;
            string errorMessage = "";
            bool corporateIDInUse = false;
            string comments = string.Empty;
            int statusID = 0;
            string defaultErrorMessage =
                "Per American Specialty Health, guest cannot enroll\r\n"
                + "at this time and needs to call \r\n"
                + "American Specialty Health customer service.";

            //  Step 01:    Access Data from ASH
            ASH.Service ASHService = new ASH.Service(Settings.Default.ASH_BaseUrl, Settings.Default.ASH_UserName, Settings.Default.ASH_Password);
            ASH.Models.Member requestMember = null;

            int memberId = 0;
            int.TryParse(silverSneakerRequest.MembershipID, out memberId);
            if (memberId > 0)
            {
                requestMember = new ASH.Models.Member() { MemberId = memberId };
            }
            else
            {
                requestMember = new ASH.Models.Member()
                {
                    FirstName = silverSneakerRequest.FirstName,
                    LastName = silverSneakerRequest.LastName,
                    DateOfBirth = silverSneakerRequest.DateOfBirth,
                    ZipCode = silverSneakerRequest.ZipCode,
                    FacilityID = silverSneakerRequest.FacilityID
                };
            }
            ASH.Models.Member responseMember = ASHService.VerifyMemberAsync(changeSourceId, requestMember).GetAwaiter().GetResult();
            //  Invalid data or not eligible member               

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

            silverSneakerRequest.MembershipID = responseMember.MemberId.ToString();

            corporateIDInUse = DataAccess.SilverSneaker.CorporateIDExists(ref silverSneakerRequest, ref comments, ref statusID);
            bool statusEligibleToReinstate = (statusID == (int)CustomerStatuses.Cancelled && !string.IsNullOrEmpty(comments));

            //  Build Data Object
            ssm = new SilverSneakerMembership();

            ssm.CorporateID = responseMember.MemberId.ToString();
            ssm.FirstName = responseMember.FirstName;
            ssm.LastName = responseMember.LastName;
            ssm.Email = responseMember.EmailAddress;

            DateTime dtmEffective = DateTime.MinValue;
            if (!DateTime.TryParse(responseMember.EffectiveDate, out dtmEffective)) dtmEffective = DateTime.MinValue;
            ssm.EffectiveDate = dtmEffective;

            DateTime dtmTermination = DateTime.MaxValue;
            if (!DateTime.TryParse(responseMember.TerminationDate, out dtmTermination)) dtmTermination = DateTime.MaxValue;
            ssm.TermDate = dtmTermination;

            //ssm.Status = responseMember.ResponseStatus.Message;
            ssm.Product = responseMember.ProductName;
            ssm.Zipcode = responseMember.ZipCode;
            ssm.EmergencyFirstName = string.Empty;
            ssm.EmergencyLastName = string.Empty;
            ssm.HomePhone = string.Empty;
            ssm.CellPhone = string.Empty;
            ssm.WorkPhone = string.Empty;
            ssm.MembershipTypeID = silverSneakerRequest.MembershipTypeID;

            ssm.FacilityID = responseMember.FacilityID;
            ssm.Product = responseMember.ProductName;


            var facility = responseMember.GetFacility();
            if (facility != null)
            {
                ssm.DocID = facility.DocId;
                ssm.ClinicID = facility.ClinicId;
            }

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
                    ErrorCode = ErrorCodes.TermedMember
                };
                return ssm;
            }

            //  Step 06:    Validate ASH Product in defined in Configuration table
            //if (responseMember.VerifyResultCode == 0) // old API
            {
                if (!string.IsNullOrEmpty(responseMember.ProductName) && (responseMember.ProductName.ToLower() != @"silver&fit" && responseMember.ProductName.ToLower() != @"active&fit"))
                {
                    silverSneakerResult = new SilverSneakerResult
                    {
                        ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                        MessageFromLAF = defaultErrorMessage,
                        ErrorCode = ErrorCodes.IncorectProgram
                    };
                    return ssm;
                }
            }

            if (!responseMember.IsEligible())
            {
                string json = JsonConvert.SerializeObject(responseMember, Formatting.Indented);
                string message = "VerifySilverSneakerMember. (ChangeSourceId = " + changeSourceId + "). "
                    + "Ineligible when verifying an ASH member from partner service. JSON = " + json;
                LogHelper.Log("Sample.Hello.Exception", message);

                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromPartner,
                    MessageFromLAF = defaultErrorMessage,
                    MessageFromPartner = message,
                    ErrorCode = ErrorCodes.Ineligible
                };
                return ssm;
            }


            if (corporateIDInUse && statusID != (int)CustomerStatuses.Cancelled)
            {
                errorMessage = "Member is already in our system.\r\nPlease use LAF keytag or mobile barcode.";
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromPartner,
                    MessageFromLAF = errorMessage,
                    ErrorCode = ErrorCodes.ExistingMember
                };
                return ssm;
            }

            // Exists in our system in Cancelled status, returns valid customer ID and if Healthways Service returns status as not eligible, set customerID = 0 and return
            if (corporateIDInUse &&
                        (statusID == (int)CustomerStatuses.Cancelled && string.IsNullOrEmpty(comments))
               )
            {
                errorMessage = Product + " ID is not eligible to reinstate.\r\n"
                                   + "Guest needs to call the " + Company + "\r\n"
                                   + "customer service.";

                errorMessage = "Member is already in our system.\r\nPlease use LAF keytag or mobile barcode.";
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromPartner,
                    MessageFromLAF = errorMessage,
                    ErrorCode = ErrorCodes.ExistingMember
                };
                return ssm;
            }


           
            /// By Commenting out errorMessage and  return null,
            /// it will allow a membership to be created with future date
            /// 
            //  Step 04:    Start date validation
            //if (ssm.HealthwayMembershipStartDate > DateTime.Now)
            //{
            //errorMessage = "Cannot use club yet. Membership not active.";
            //return null;
            //}

           

            // Success

            //info logging when success
            string jsonMsg = JsonConvert.SerializeObject(responseMember, Formatting.Indented);
            string msg = "VerifySilverSneakerMember succeeded. (ChangeSourceId = " + changeSourceId + "). JSON = " + jsonMsg;
            LogHelper.Log("Sample.Hello.Exception", msg);

            // Enroll from partner then return successful
            if (responseMember.VerifyResultCode == 200) // eligible and need to enroll
            {
                ssm.EnrollmentRequestId = responseMember.EnrollmentRequestId;
                SilverSneakerResult ssResult = new SilverSneakerResult();
                EnrollFromPartner(changeSourceId, ref ssm, out ssResult);
            }

            // load data when eligible to reinstate
            if (statusEligibleToReinstate)
            {
                errorMessage = "Member is eligible to be reinstated";
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = errorMessage,
                    ErrorCode = ErrorCodes.ExistingMemberCanReinstate
                };
                return ssm;
            }

            silverSneakerResult = new SilverSneakerResult
            {
                ErrorType = SilverSneakerErrorTypes.Success
            };
            return ssm;
        }

        public override void EnrollFromPartner(int changeSourceId, ref Common.Data.SilverSneakerMembership ssm, out SilverSneakerResult silverSneakerResult)
        {
            try
            {
                //exeption/ faultException : log/unlog from caller
                if (string.IsNullOrEmpty(ssm.EnrollmentRequestId))
                {
                    silverSneakerResult = new SilverSneakerResult
                    {
                        ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                        MessageFromLAF = "Invalid EnrollmentRequestId",
                    };
                    return;
                }
                ASH.Service ASHService = new ASH.Service(Settings.Default.ASH_BaseUrl, Settings.Default.ASH_UserName, Settings.Default.ASH_Password);
                ASH.Models.EnrollMember request = new ASH.Models.EnrollMember()
                {
                    EnrollmentRequestId = ssm.EnrollmentRequestId,
                };

                ASH.Models.EnrollMember response = ASHService.EnrollMemberAsync(changeSourceId, request).GetAwaiter().GetResult();

                if (response == null)
                {
                    silverSneakerResult = new SilverSneakerResult
                    {
                        ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                        MessageFromLAF = "Error when enrolling the member"
                    };
                    return;
                }
                else if (!response.IsSuccess())
                {
                    string json = JsonConvert.SerializeObject(response, Formatting.Indented);
                    string message = "EnrollFromPartner succeeded. (ChangeSourceId = " + changeSourceId + "). JSON = " + json;
                    LogHelper.Log("Sample.Hello.Exception", message);

                    silverSneakerResult = new SilverSneakerResult
                    {
                        ErrorType = SilverSneakerErrorTypes.ErrorFromPartner,
                        MessageFromLAF = "Error when enrolling the member",
                        MessageFromPartner = message
                    };
                    return;
                }
                else
                {
                    //SUCCESS
                    string date = response.ProgramEffectiveDate;
                    DateTime? dt = string.IsNullOrEmpty(date) ? (DateTime?)null : DateTime.Parse(date);
                    ssm.EnrollProgramEffectiveDate = dt;
                    //info logging when success
                    string json = JsonConvert.SerializeObject(response, Formatting.Indented);
                    string message = "EnrollFromPartner. (ChangeSourceId = " + changeSourceId + "). "
                        + "Enroll success. JSON = " + json;
                    LogHelper.Log("Sample.Hello.Exception", message);

                    silverSneakerResult = new SilverSneakerResult
                    {
                        ErrorType = SilverSneakerErrorTypes.Success,
                    };
                    return;
                }
            }
            catch (Exception ex)//suppress the exception because enrollment won't block the process
            {
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = "Error when enrolling the member"
                };
                LogHelper.Log("Sample.Hello.Exception", ex.Message);
            }
        }

        public override SilverSneakerCancelResult CancelSilverSneakerMembers(int changeSourceId)
        {
            SilverSneakerCancelResult result = new SilverSneakerCancelResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };
            SilverSneakerPartner.ASH.Service ASHService = new SilverSneakerPartner.ASH.Service(Settings.Default.ASH_BaseUrl, Settings.Default.ASH_UserName, Settings.Default.ASH_Password);
            List<TermedMember> responseMemberList = ASHService.TermedMemberAsync((int)ChangeSource.Hello).GetAwaiter().GetResult();
            Guid processID = DataAccess.SilverSneaker.CancelProcess_History_Insert(SilverSneakerType.ASH, SilverSneakersProcessType.Cancellation);
            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "ASH member cancellation", string.Format("Start.{0} members received from ASH service", responseMemberList.Count));
            DataTable dtTermedMember = Converter.ToASH_TermedMember_DataTable(processID, responseMemberList);
            Sample.Hello.DataAccess.SilverSneaker.ASH_TermedMember_Insert(dtTermedMember);
            foreach (TermedMember termedMember in responseMemberList)
            {
                try
                {
                    List<int> customerIDs = DataAccess.SilverSneaker.GetCustomerIDFromCorporateID(termedMember.FitnessId.ToString(), SilverSneakerType.ASH);
                    if (customerIDs == null || customerIDs.Count <= 0)
                    {
                        result.SkipCount++;
                        continue;
                    }

                    foreach (var customerID in customerIDs)
                    {
                        Customer cust = HelloSystem.Instance.GetCustomer(customerID);

                        if (termedMember.TerminationDate != null
                            && termedMember.TerminationDate != DateTime.MinValue
                            && termedMember.TerminationDate.Value.Date <= DateTime.Today
                            && cust != null
                            && !cust.Status.CurrentStatus.IsCancelledOrThree_SevenDayCancelled
                            && !cust.Status.CurrentStatus.IsRevokedDenyEntryOnly)
                        {
                            int employeeID = Settings.Default.AutomatedEmployeeID;
                            DateTime currentDate = DateTime.Now.Date;
                            string statusReason = "Silver Fit";
                            CancelReasons cancelReason = CancelReasons.NonUsage;  // from CancelReason table
                            int aphelionClubID = Settings.Default.CorporateClubAphelionID;
                            string operation = SSType.ToString() + " automated cancellation";
                            ChangeSource changeSource = ChangeSource.HealthwaysCancellation; //application name
                            string comments = string.Format("Cancel from ASH Membership Cancellation on {0:G} from Corporate.", DateTime.Now);
                            bool isSuccess = HelloSystem.Instance.ProcessSimpleMembershipCancel(cust, comments, (int)cancelReason, employeeID,
                                aphelionClubID, statusReason, DateTime.Now.Date, DateTime.Now.Date, operation, (int)changeSource);
                            if (isSuccess)
                            {
                                result.SuccessCount++;
                                DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "ASH member cancellation", string.Format("CorporateID: {0} - success", termedMember.FitnessId));

                            }
                            else
                            {
                                DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "ASH member cancellation", string.Format("CorporateID: {0} - failed", termedMember.FitnessId));
                                result.ErrorCount++;
                            }
                        }
                        else
                        {
                            result.SkipCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    int eventLogID = HelloSystem.Instance.Log(ex, "TermedMember.FitnessID " + termedMember.FitnessId.ToString());
                    DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "ASH member cancellation", string.Format("CorporateID: {0} - failed. EventLogID = {1}", termedMember.FitnessId, eventLogID));
                }
            }
            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "ASH member cancellation", string.Format("End. {0} received from ASH. {1} are succeed. {2} are failed. {3} are skipped.",
                responseMemberList.Count, result.SuccessCount, result.ErrorCount, result.SkipCount));

            return result;
        }

        public override SilverSneakerBillingActivityResult SilverSneakerBillingActivity(int changeSourceId)
        {
            SilverSneakerBillingActivityResult result = new SilverSneakerBillingActivityResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };
            return result;
        }
    }
}
