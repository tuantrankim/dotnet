using Sample.Hello.BusinessFacade.SilverSneakerPartner.Healthways.Models;
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
    class HealthWayAdapter: SilverSneakerPartnerBase
    {
        public HealthWayAdapter(SilverSneakerType silverSneakerType)
            : base(silverSneakerType, "Tivity Health", "Silver Sneakers/Prime") { }
        public override SilverSneakerMembership VerifyFromPartner(int changeSourceId, Common.Data.SilverSneakerRequest silverSneakerRequest, out SilverSneakerResult silverSneakerResult)
        {
            SilverSneakerMembership ssm = null;
            string errorMessage = "";


            string defaultErrorMessage = Company + " was unable to verify eligibility.\r\n"
                                                                           + "Guest needs to call the " + Company + "\r\n"
                                                                           + "customer service number \r\n"
                                                                           + "located on the back of their ID card.";
            Healthway_Service.HwayMembershipClient healthwayService = new Healthway_Service.HwayMembershipClient();

            //use old end point if before the switching daytime
            //if (DateTime.Now < new DateTime(2017, 3, 25))
            //{
            //    string endPoint = healthwayService.Endpoint.ListenUri.ToString();
            //    endPoint = endPoint.Replace("tivityhealth.com", "healthways.com");
            //    healthwayService = new Healthway_Service.HwayMembershipClient("WSHttpBinding_IHwayMembership",endPoint);
            //}

            string healthwayCompanyID = Settings.Default.Healthway_Company_ID;
            string healthwayRegKey = Settings.Default.Healthway_RegKey;
            bool corporateIDInUse = false;
            //string emergencycontact = string.Empty; string emergencyphone = string.Empty;

            string comments = string.Empty;
            int statusID = 0;
            Healthway_Service.VerificationResult responseMember = null;

            // Access Data from Healthway
            
            //LogHelper.LogError("Sample.Hello.Exception", null, false, employeeID.ToString(), "Information", null, healthwayIdentificationNumber, employeeID, clubID);
            responseMember = healthwayService.VerifySingle(healthwayCompanyID, healthwayRegKey, silverSneakerRequest.MembershipID, Healthway_Service.Product.All);

            // Load existing data from SilverSneaker table
            corporateIDInUse = DataAccess.SilverSneaker.CorporateIDExists(ref silverSneakerRequest, ref comments, ref statusID);
            bool statusEligibleToReinstate = (statusID == (int)CustomerStatuses.Cancelled && !string.IsNullOrEmpty(comments));

            //  Build Data Object
            ssm = new SilverSneakerMembership();

            //ssm.HealthwayActiveLivingMemberID = responseMember.ActiveLivingMemberID;
            ssm.CorporateID = responseMember.HwayCardNumber;
            ssm.FirstName = responseMember.FirstName;
            ssm.LastName = responseMember.LastName;
            ssm.Email = responseMember.Email;
            ssm.EffectiveDate = responseMember.EffDate;
            ssm.TermDate = responseMember.TermDate;
            ssm.Status = responseMember.Status.ToString();
            ssm.Product = responseMember.Product.ToString();
            ssm.EmergencyFirstName = string.Empty;
            ssm.EmergencyLastName = string.Empty;
            ssm.HomePhone = string.Empty;
            ssm.CellPhone = string.Empty;
            ssm.WorkPhone = string.Empty;
            ssm.MembershipTypeID = silverSneakerRequest.MembershipTypeID;

            // load data when member already exists
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

            //  End date validation
            if (ssm.TermDate.Date < DateTime.Today)
            {
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = defaultErrorMessage,
                    ErrorCode = ErrorCodes.TermedMember,
                };
                return ssm;
            }

            //  Validate Healthway Product in defined in Configuration table
            IEnumerable<Configuration> validProductCollection = HelloCache.Instance.Configurations.Where(c => c.TypeID.Equals((int)ConfigurationTypes.Healthway_Product_SilverSneakers) ||
                                                                                                                                        c.TypeID.Equals((int)ConfigurationTypes.Healthway_Product_Prime) ||
                                                                                                                                        c.TypeID.Equals((int)ConfigurationTypes.Healthway_Product_PrimePrivateBrand));
            
            if (!string.IsNullOrEmpty(ssm.Product)  
                && validProductCollection.Where(c => c.ConfigValue.Equals(ssm.Product, StringComparison.CurrentCultureIgnoreCase)).Count() == 0)
            {
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = defaultErrorMessage,
                    ErrorCode = ErrorCodes.IncorectProgram
                };
                return ssm;
            }

            //Invalid data or not eligible member
            if (responseMember.ActiveLivingMemberID == 0)
            {
                string json = JsonConvert.SerializeObject(responseMember, Formatting.Indented);
                string message = "VerifySilverSneakerMember. (ChangeSourceId = " + changeSourceId + "). "
                + "Ineligible when verifying a Tivity Health member from partner service. JSON = " + json;
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


            // Existing Member that is eligible
            if (corporateIDInUse && responseMember.Status != Healthway_Service.MemberStatus.Ineligible && statusID != (int)CustomerStatuses.Cancelled)
            {
                errorMessage = "Member is already in our system.\r\nPlease use LAF keytag or mobile barcode.";
                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = errorMessage,
                    ErrorCode = ErrorCodes.ExistingMember
                };
                return ssm; 
            }

            // Exists in our system in Cancelled status, returns valid customer ID and if Tivity Health Service returns status as not eligible, set customerID = 0 and return
            if (corporateIDInUse &&
                    (
                        (statusEligibleToReinstate && responseMember.Status == Healthway_Service.MemberStatus.Ineligible) ||
                        (statusID == (int)CustomerStatuses.Cancelled && string.IsNullOrEmpty(comments))
                    )
               )
            {
                errorMessage = Product + " ID is not eligible to reinstate.\r\n"
                                       + "Guest needs to call the " + Company + "\r\n"
                                       + "customer service number \r\n"
                                       + "located on the back of their ID card.";


                silverSneakerResult = new SilverSneakerResult
                {
                    ErrorType = SilverSneakerErrorTypes.ErrorFromHelloSystem,
                    MessageFromLAF = errorMessage,
                    ErrorCode = ErrorCodes.ExistingMember
                };
                return ssm;
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

            //  Return Data
            silverSneakerResult = new SilverSneakerResult
            {
                ErrorType = SilverSneakerErrorTypes.Success
            };
            return ssm;
        }

        public override void EnrollFromPartner(int changeSourceId, ref Common.Data.SilverSneakerMembership ssm, out SilverSneakerResult silverSneakerResult)
        {
            silverSneakerResult = new SilverSneakerResult{ErrorType = SilverSneakerErrorTypes.Success};
        }

        public override SilverSneakerCancelResult CancelSilverSneakerMembers(int changeSourceId)
        {
            SilverSneakerCancelResult result = new SilverSneakerCancelResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };
            if (this.SSType == SilverSneakerType.Healthways)
            {

                Guid processID;
                List<Common.Data.SilverSneakerCancelProcessStaging> cancellingMembers;
                List<Common.Data.SilverSneakerCancelProcessStaging> reinstatingMembers;
                PrepareCancelProcessStaging(changeSourceId, out processID, out cancellingMembers, out reinstatingMembers);

                // Do cancellation
                result = DoCancel(changeSourceId, processID, cancellingMembers);

                // Do reinstation
                var reinstateResult = DoReinstate(changeSourceId, processID, reinstatingMembers);
            }
            else
            {
                result.ErrorMessage = "Tivity Health service does not support silver sneaker type " + SSType.ToString();
            }

            return result;
        }

        private void PrepareCancelProcessStaging(int changeSourceId, out Guid processID,
            out List<Common.Data.SilverSneakerCancelProcessStaging> cancellingMembers,
            out List<Common.Data.SilverSneakerCancelProcessStaging> reinstatingMembers)
        {
            //  STEP 01: Copy Data to Staging table and set process ID
            DataAccess.SilverSneaker.Copy_SilverSneakerMembers_To_StagingTable(out processID, this.SSType);

            int top = 300;
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
                    //Verify from HEALTHWAY
                    //  Build Request Collection
                    List<Healthway_Service.VerificationRequest> requestCollection = BuildRequestCollection(subset);

                    //  validate against API
                    List<Healthway_Service.VerificationResult> resultsCollection = ValidateViaHealthwaysAPI(requestCollection, processID, threadID);

                    
                    
                    //var results = responseMember.result.ToDictionary(r=> r.member_id, r => r.eligible);
                    //Tolookup allows duplicated lookup key in case of multiple brand
                    var responseResults = resultsCollection.ToLookup(r => r.HwayCardNumber.ToUpper(), r => r);

                    //Update subset 
                    foreach (var ssm in subset)
                    {
                        var responseItem = responseResults[ssm.CorporateID.ToUpper()].FirstOrDefault();
                        if (responseItem == null) ssm.NewStatusID = null;
                        else
                        {
                            bool isEligible = (responseItem.Status == Healthway_Service.MemberStatus.Eligible);
                            ssm.ProductName = responseItem.Product.ToString();
                            ssm.NewHwayCardNumber = responseItem.HwayCardNumber;
                            ssm.HwayStatus = isEligible ? "Eligible" : "Ineligible";
                            ssm.EffectiveDate = responseItem.EffDate;
                            ssm.TerminationDate = responseItem.TermDate;
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

        private List<Healthway_Service.VerificationRequest> BuildRequestCollection(List<Common.Data.SilverSneakerCancelProcessStaging> members)
        {
            List<Healthway_Service.VerificationRequest> requestCollection = new List<Healthway_Service.VerificationRequest>();

            foreach (Common.Data.SilverSneakerCancelProcessStaging mem in members)
            {
                requestCollection.Add(new Healthway_Service.VerificationRequest()
                {
                    HwayCardNumber = mem.CorporateID,
                    Product = Healthway_Service.Product.None
                }
                );
            }

            return requestCollection;
        }

        private List<Healthway_Service.VerificationResult> ValidateViaHealthwaysAPI(List<Healthway_Service.VerificationRequest> requests, Guid? processID, int threadID)
        {
            List<Healthway_Service.VerificationResult> resultCollection = new List<Healthway_Service.VerificationResult>();

            ///  Do not need to divide up members in groups of 300.
            ///  Assume members count is 300.

            Healthway_Service.HwayMembershipClient healthwayService = new Healthway_Service.HwayMembershipClient();

            //use old end point if before the switching daytime
            //if (DateTime.Now < new DateTime(2017, 3, 25))
            //{
            //    string endPoint = healthwayService.Endpoint.ListenUri.ToString();
            //    endPoint = endPoint.Replace("tivityhealth.com", "healthways.com");
            //    healthwayService = new Healthway_Service.HwayMembershipClient("WSHttpBinding_IHwayMembership", endPoint);
            //}

            string healthwayCompanyID = Settings.Default.Healthway_Company_ID;
            string healthwayRegKey = Settings.Default.Healthway_RegKey;


            Healthway_Service.VerificationResult[] results = null;

            //  Step 03:    Validate via Tivity Health API
            try
            {
                results = healthwayService.VerifyArray(healthwayCompanyID, healthwayRegKey, requests.ToArray());
            }
            catch (Exception ex)
            {
                try
                {
                    LogHelper.LogError("HealthwaysCancellation", ex, false, System.Reflection.MethodInfo.GetCurrentMethod(), processID, threadID);
                }
                catch { }
            }

            if (results != null && results.Count() > 0)
                return results.ToList();
            else return null;
        }

        private SilverSneakerCancelResult DoCancel(int changeSourceId, Guid processID, List<Common.Data.SilverSneakerCancelProcessStaging> cancellingMembers)
        {
            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, SSType.ToString() + " member cancellation",
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
                        string comments = string.Format("Cancel from {0} Membership Cancellation on {1:G} from Corporate.", SSType.ToString(), DateTime.Now);
                        bool isSuccess = HelloSystem.Instance.ProcessSimpleMembershipCancel(customer, comments, (int)cancelReason, employeeID,
                            aphelionClubID, statusReason, DateTime.Now.Date, DateTime.Now.Date, operation, (int)changeSource);
                        if (isSuccess)
                        {
                            result.SuccessCount++;
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, SSType.ToString() + "member cancellation",
                                string.Format("CorporateID: {0}, CustomerID: {1} - success", member.CorporateID, member.LAFCustomerID.ToString()));

                        }
                        else
                        {
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, SSType.ToString() + "member cancellation",
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
                    DataAccess.SilverSneaker.CancelAppLog_Insert(processID, SSType.ToString() + " member cancellation",
                        string.Format("CorporateID: {0}, CustomerID: {1} - failed. EventLogID = {2}", member.CorporateID, member.LAFCustomerID, eventLogID));
                }
            }
            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, SSType.ToString() + " member cancellation", string.Format("End. {0} received from {1}. {2} are succeed. {3} are failed. {4} are skipped.",
                cancellingMembers.Count, SSType.ToString(), result.SuccessCount, result.ErrorCount, result.SkipCount));

            return result;
        }


        private SilverSneakerCancelResult DoReinstate(int changeSourceId, Guid processID, List<Common.Data.SilverSneakerCancelProcessStaging> reinstatingMembers)
        {
            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, SSType.ToString() + " member reinstating",
                string.Format("Start.{0} members are going to be reinstated", reinstatingMembers.Count));

            SilverSneakerCancelResult result = new SilverSneakerCancelResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };

            int employeeID = Settings.Default.AutomatedEmployeeID;
            string statusReason = this.SSType.ToString();
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
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, SSType.ToString() + "member reinstating",
                                string.Format("CorporateID: {0}, CustomerID: {1} - success", member.CorporateID, member.LAFCustomerID.ToString()));

                        }
                        else
                        {
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, SSType.ToString() + "member reinstating",
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
                    DataAccess.SilverSneaker.CancelAppLog_Insert(processID, SSType.ToString() + " member reinstating",
                        string.Format("CorporateID: {0}, CustomerID: {1} - failed. EventLogID = {2}", member.CorporateID, member.LAFCustomerID, eventLogID));
                }
            }
            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, SSType.ToString() + " member reinstating", string.Format("End. {0} received from {1}. {2} are succeed. {3} are failed. {4} are skipped.",
                reinstatingMembers.Count, SSType.ToString(), result.SuccessCount, result.ErrorCount, result.SkipCount));

            return result;
        }

        //public override SilverSneakerBillingActivityResult SilverSneakerBillingActivity(int changeSourceId)
        //{
        //    SilverSneakerBillingActivityResult result = new SilverSneakerBillingActivityResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };

        //    try
        //    {
        //        Healthways.Service HWAuthService = new Healthways.Service(Settings.Default.HealthwaysAuth_BaseUrl);
        //        Healthways.Models.OAuthRequest tokenRequest = new Healthways.Models.OAuthRequest()
        //        {
        //            client_id = "Sample.activityfeed",
        //            client_secret = Settings.Default.HealthwaysClientSecret,
        //            grant_type = "client_credentials",
        //            scope = "insert_single insert_batch get_ineligible",
        //        };

        //        OAuthResponse tokenResponse = HWAuthService.GetToken(changeSourceId, tokenRequest).GetAwaiter().GetResult();


        //        if (tokenResponse == null|| tokenResponse.access_token == "")
        //        {
        //             result = new SilverSneakerBillingActivityResult
        //            {
        //                ErrorMessage = "Authentication failed from Healthway service."
        //            };
        //        }
        //        else //Processing billing activity // 50k limited batch size
        //        {
        //            String accessToken = tokenResponse.access_token;
        //            /*
        //            [
        //             {
        //            "FirstName": "Peter",
        //            "LastName": "Parker",
        //            "DateOfBirth": null,
        //            "LocationCardNumber": "33779999",
        //            "TivityHealthCardNumber":"2300999999999999",
        //            "ActivityDateTime": "11/15/2017 10:08:07 AM",
        //            "TivityHealthLocationID": null,
        //            "YourClubID":"065",
        //            "Email":"peterparker@test.com",
        //            "HomePhoneNumber": "999-999-9999",
        //            "MobilePhoneNumber": "999-999-9999"
        //            },
        //            {
        //            "FirstName": "Bruce",
        //            "LastName": "Wayne",
        //            "DateOfBirth": null,
        //            "LocationCardNumber": "33776777",
        //            "TivityHealthCardNumber":"2300999999999999",
        //            "ActivityDateTime": "11/15/2017 11:08:07 AM",
        //            "TivityHealthLocationID": null,
        //            "YourClubID":"065",
        //            "Email":"brucewayne@test.com",
        //            "HomePhoneNumber": "999-999-9999",
        //            "MobilePhoneNumber": "999-999-9999"
        //            }
        //            ]
        //             */
        //            //Step 1: SubmitBatchActivity to upload batch of members checkin today
        //            List<Activity> batchActivityRequest = new List<Activity>();
        //            batchActivityRequest.Add(
        //            new Activity()
        //                {
        //                    FirstName = "Peter",
        //                    LastName = "Parker",
        //                    DateOfBirth = null,
        //                    LocationCardNumber = null,
        //                    TivityHealthCardNumber = "2300999999999999",
        //                    ActivityDateTime = System.Convert.ToDateTime("11/15/2017 10:08:07 AM"),
        //                    TivityHealthLocationID = null,
        //                    YourClubID = "065",
        //                    Email = "peterparker@test.com",
        //                    HomePhoneNumber = "999-999-9999",
        //                    MobilePhoneNumber = "999-999-9999"
        //                }
        //                );
        //            batchActivityRequest.Add(
        //                new Activity()
        //                {
        //                    FirstName = "Bruce",
        //                    LastName = "Wayne",
        //                    DateOfBirth = null,
        //                    LocationCardNumber = null,
        //                    TivityHealthCardNumber = "2300999999999999",
        //                    ActivityDateTime = System.Convert.ToDateTime("11/15/2017 11:08:07 AM"),
        //                    TivityHealthLocationID = null,
        //                    YourClubID = "065",
        //                    Email = "brucewayne@test.com",
        //                    HomePhoneNumber = "999-999-9999",
        //                    MobilePhoneNumber = "999-999-9999"
        //                }
        //                );
        //            Healthways.Service HWService = new Healthways.Service(Settings.Default.Healthways_BaseUrl, BaseService.Authentication.OAuth, accessToken);

        //            BatchActivityResponse batchActivityResponse = HWService.SubmitBatchActivity(changeSourceId, batchActivityRequest).GetAwaiter().GetResult();

        //            string batchGUID = "";
        //            if (batchActivityResponse == null || batchActivityResponse.ResponseStatus != "Success")
        //            {
        //                result = new SilverSneakerBillingActivityResult
        //                {
        //                    ErrorMessage = "Submit batch activity failed"
        //                };
        //            }
        //            else
        //            {
        //                batchGUID = batchActivityResponse.BatchGUID;
        //            }

        //            //Step 2: GetIneligibleMembers to retrieve the failed ones from the previous run
        //            if(!string.IsNullOrEmpty(batchGUID))
        //            {
        //                GetIneligibleMembersRequest getIneligibleMemberRequest = new GetIneligibleMembersRequest{BatchGUID = batchGUID};
        //                List<IneligibleMember> getIneligibleMemberResponse = HWService.GetIneligibleMembers(changeSourceId, getIneligibleMemberRequest).GetAwaiter().GetResult();

        //                if (getIneligibleMemberResponse == null)
        //                {
        //                    result = new SilverSneakerBillingActivityResult
        //                    {
        //                        ErrorMessage = "Get Ineligible Members failed"
        //                    };
        //                }
        //                else
        //                {
        //                    //GetIneligibleMembers success
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)//suppress the exception because enrollment won't block the process
        //    {
        //        result = new SilverSneakerBillingActivityResult
        //        {
        //            ErrorMessage = "Error when processing Healthways billing activity"
        //        };
        //        LogHelper.Log("Sample.Hello.Exception", ex.Message);
        //    }

        //    return result;
        //}
        public override SilverSneakerBillingActivityResult SilverSneakerBillingActivity(int changeSourceId)
        {
            SilverSneakerBillingActivityResult result = new SilverSneakerBillingActivityResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };

            try
            {
                Guid processID = DataAccess.SilverSneaker.CancelProcess_History_Insert(SilverSneakerType.Healthways, SilverSneakersProcessType.BillingActivity);
                Healthways.Service HWAuthService = new Healthways.Service(Settings.Default.HealthwaysAuth_BaseUrl);
                Healthways.Models.OAuthRequest tokenRequest = new Healthways.Models.OAuthRequest()
                {
                    client_id = "Sample.activityfeed",
                    client_secret = Settings.Default.HealthwaysClientSecret,
                    grant_type = "client_credentials",
                    scope = "insert_single insert_batch get_ineligible get_processed_activities",
                };

                OAuthResponse tokenResponse = HWAuthService.GetToken(changeSourceId, tokenRequest).GetAwaiter().GetResult();
                

                if (tokenResponse == null|| tokenResponse.access_token == "")
                {
                     result = new SilverSneakerBillingActivityResult
                    {
                        ErrorMessage = "Authentication failed from Tivity Health service."
                    };
                }
                else //Processing billing activity // 50k limited batch size
                {
                    String accessToken = tokenResponse.access_token;
                    Healthways.Service HWService = new Healthways.Service(Settings.Default.Healthways_BaseUrl, BaseService.Authentication.OAuth, accessToken);
                    SubmitBatchActivity(HWService, changeSourceId, processID);
                    result = GetIneligibleBillingActivity(HWService, changeSourceId, processID);
                    SubmitBatchActivity_Retry(HWService, changeSourceId, processID);
                }
            }
            catch (Exception ex)
            {
                result = new SilverSneakerBillingActivityResult
                {
                    ErrorMessage = "Error when processing Tivity Health billing activity"
                };
                LogHelper.LogError("Sample.Hello.Exception", ex, false, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            
            return result;
        }

        private SilverSneakerBillingActivityResult SubmitBatchActivity(Healthways.Service HWService, int changeSourceId, Guid processID)
        {
            SilverSneakerBillingActivityResult result = new SilverSneakerBillingActivityResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };
            try
            {
                //  STEP 01: Copy Data to Staging table and set process ID
                DataAccess.SilverSneaker.Copy_SilverSneakerActivity_To_StagingTable(processID, this.SSType);

                int top = 3000;
                int threadID = 1;
                List<SilverSneakerBillingActivityStaging> subset = null;

                do
                {
                    subset = DataAccess.SilverSneaker.SilverSneakers_BillingActivity_Staging_Select_Top(processID, threadID, top, false);
                    if (subset == null || subset.Count() == 0) break;
                    SubmitBatchActivity_Subset(HWService, changeSourceId, processID, result, threadID, subset);

                    threadID++;

                } while (subset != null && subset.Count() > 0);

                int submittedCount = threadID - 1;
                DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways SubmitBatchActivity",
                    string.Format("End.{0} batchs. {1} activities. {2} success. {3} fail", submittedCount, result.ErrorCount + result.SuccessCount, result.SuccessCount, result.ErrorCount));
            }
            catch (Exception e)
            {
                int eventLogID = LogHelper.LogError("Sample.Hello.Exception", e, false, System.Reflection.MethodInfo.GetCurrentMethod());
                string errMsg = string.Format("Healthways SubmitBatchActivity. EventLogID = {0}", eventLogID);
                DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways SubmitBatchActivity", errMsg);
                result.ErrorMessage = errMsg;
                result.ErrorCount = -1;
            }
            return result;

        }

        private SilverSneakerBillingActivityResult SubmitBatchActivity_Retry(Healthways.Service HWService, int changeSourceId, Guid processID)
        {
            SilverSneakerBillingActivityResult result = new SilverSneakerBillingActivityResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };
            try
            {

                int top = 3000;
                int threadID = -1;
                List<SilverSneakerBillingActivityStaging> subset = null;

                //Resubmit previous date activities batch if they are failed
                SilverSneakerBillingActivityResult result2 = new SilverSneakerBillingActivityResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };
                DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways SubmitBatchActivity", "Begin retry");
                do
                {
                    subset = DataAccess.SilverSneaker.SilverSneakers_BillingActivity_Staging_Select_Top(processID, threadID, top, true);
                    if (subset == null || subset.Count() == 0) break;
                    SubmitBatchActivity_Subset(HWService, changeSourceId, processID, result2, threadID, subset);

                    threadID--;

                } while (subset != null && subset.Count() > 0);

                int submittedCount = -(threadID + 1);
                DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways SubmitBatchActivity",
                    string.Format("End. {0} batchs. {1} activities. {2} success. {3} fail", submittedCount, result2.ErrorCount + result2.SuccessCount, result2.SuccessCount, result2.ErrorCount));

            }
            catch (Exception e)
            {
                int eventLogID = LogHelper.LogError("Sample.Hello.Exception", e, false, System.Reflection.MethodInfo.GetCurrentMethod());
                string errMsg = string.Format("Healthways SubmitBatchActivity. EventLogID = {0}", eventLogID);
                DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways SubmitBatchActivity", errMsg);
                result.ErrorMessage = errMsg;
                result.ErrorCount = -1;
            }
            return result;

        }
        private static void SubmitBatchActivity_Subset(Healthways.Service HWService, int changeSourceId, Guid processID, SilverSneakerBillingActivityResult result, int threadID, List<SilverSneakerBillingActivityStaging> subset)
        {
            try
            {
                //Step 1: SubmitBatchActivity to upload batch of members checkin today
                //  Build Request Collection
                //  Do not send Email, Phone and DateOfBirth to TivityHealth

                List<Activity> batchActivityRequest = subset.Select(e => new Activity
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DateOfBirth = "",
                    Email = "",
                    HomePhoneNumber = "",
                    MobilePhoneNumber = "",
                    TivityHealthCardNumber = e.CorporateID,
                    LocationCardNumber = e.TivityHealthLocationID,
                    YourClubID = e.ClubDepartment,
                    ActivityDateTime = e.CheckInTime,
                    TivityHealthLocationID = e.TivityHealthLocationID,
                    ReferenceID = e.LAFCustomerID.ToString(),
                }).ToList<Activity>();

                BatchActivityResponse batchActivityResponse = HWService.SubmitBatchActivity(changeSourceId, batchActivityRequest).GetAwaiter().GetResult();

                string batchGUID = "";
                if (batchActivityResponse == null || batchActivityResponse.APIResponseStatus != "Success")
                {
                    //result = new SilverSneakerBillingActivityResult
                    //{
                    //    ErrorMessage = "Submit batch activity failed"
                    //};

                    DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways SubmitBatchActivity", "SubmitBatchActivity - failed");
                    result.ErrorCount += subset.Count();
                }
                else
                {
                    batchGUID = batchActivityResponse.BatchGUID;
                    subset.ForEach(s =>
                    {
                        s.BatchGUID = batchGUID;
                        s.ThreadID = threadID;
                    });
                    //Update staging table
                    var dtChangedSubset = Converter.ToSilverSneakerBillingActivityStaging_DataTable(subset);
                    DataAccess.SilverSneaker.SilverSneakers_BillingActivity_Staging_Update(dtChangedSubset);
                    DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways SubmitBatchActivity BatchGUID", batchGUID);
                    result.SuccessCount += subset.Count();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    int eventLogID = LogHelper.LogError("Sample.Hello.Exception", ex, false, System.Reflection.MethodInfo.GetCurrentMethod(), processID);
                    DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways SubmitBatchActivity", string.Format("SubmitBatchActivity - failed. EventLogID = {0}", eventLogID));
                }
                catch { }
            }
        }

        private SilverSneakerBillingActivityResult GetIneligibleBillingActivity(Healthways.Service HWService, int changeSourceId, Guid processID)
        {
            SilverSneakerBillingActivityResult result = new SilverSneakerBillingActivityResult() { ErrorCount = 0, SkipCount = 0, SuccessCount = 0, TotalCount = 0, ErrorMessage = null };
            try
            {
                //  STEP 01: Copy Data to Staging table and set process ID
                List<String> activityBatchs = DataAccess.SilverSneaker.SilverSneakers_Healthways_BillingActivityBatchGuid_Select();
                DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways GetIneligibleBillingActivity",
                string.Format("Start.{0} batchs", activityBatchs.Count));
                foreach (String batchGUID in activityBatchs)
                {
                    //get our batch
                    var activityStagingList = DataAccess.SilverSneaker.SilverSneakers_BillingActivity_Staging_Select(batchGUID);
                    //only handle ones that IneligibleForBilling is null
                    activityStagingList.RemoveAll(x => x.IneligibleForBilling != null);

                    try
                    {
                        //get their batch
                        GetProcessedActivitiesRequest getProcessedActivitiesRequest = new GetProcessedActivitiesRequest { BatchGUID = batchGUID };
                        GetProcessedActivitiesResponse getProcessedActivitiesResponse = HWService.GetProcessedActivities(changeSourceId, getProcessedActivitiesRequest).GetAwaiter().GetResult();


                        if (getProcessedActivitiesResponse == null 
                            || getProcessedActivitiesResponse.APIStatus != "Success" 
                            || getProcessedActivitiesResponse.Response  == null
                            //|| getProcessedActivitiesResponse.Response.BatchStatus != "Processed"
                            )
                        {

                            //result = new SilverSneakerBillingActivityResult
                            //{
                            //    ErrorMessage = "Get Ineligible Members failed"
                            //};

                            string msg = string.Format("Failed. BatchGUID={0}. ", batchGUID);
                            if (getProcessedActivitiesResponse == null) msg += "Tivity getProcessedActivitiesResponse is null. ";
                            else if (getProcessedActivitiesResponse.APIStatus != "Success") msg += string.Format("APIStatus={0}. ", getProcessedActivitiesResponse.APIStatus);
                            else if (getProcessedActivitiesResponse.Response == null) msg += "Tivity getProcessedActivitiesResponse.Response is null. ";
                            else if (getProcessedActivitiesResponse.Response.BatchStatus != "Processed") msg += string.Format("BatchStatus={0}. ", getProcessedActivitiesResponse.Response.BatchStatus);
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways GetIneligibleBillingActivity", msg);
                            result.ErrorCount += activityStagingList.Count();
                        }
                        else
                        {
                            var responseResults = getProcessedActivitiesResponse.Response.MemberActivities
                                .ToLookup(r => r.MemberDetails.TivityHealthCardNumber + r.MemberDetails.ActivityDateTime.ToString("yyyyMMddHHmmss"), r => r);

                            //Success - let's update staging table
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways GetIneligibleBillingActivity", string.Format("Success. BatchGUID={0}", batchGUID));
                            // getIneligibleMemberResponse.

                            foreach (var activity in activityStagingList)
                            {
                                string lookupKey = activity.CorporateID + activity.CheckInTime.ToString("yyyyMMddHHmmss");
                                ActivityStatus responseItem = responseResults[lookupKey].FirstOrDefault();
                                if (responseItem != null)
                                {
                                    if (responseItem.EligibilityStatus == "Ineligible") activity.IneligibleForBilling = true;
                                    else if (responseItem.EligibilityStatus == "Eligible") activity.IneligibleForBilling = false;
                                    else activity.IneligibleForBilling = null;
                                    
                                    activity.Reason = responseItem.Reason;
                                    activity.TivityHealthLocationID = responseItem.MemberDetails.TivityHealthLocationID;
                                    activity.LocationCardNumber = responseItem.MemberDetails.LocationCardNumber;
                                    activity.ReferenceID = responseItem.MemberDetails.ReferenceID;
                                }
                                else
                                {
                                    activity.IneligibleForBilling = null;
                                }
                            }

                            //Update staging table
                            var dtChangedSubset = Converter.ToSilverSneakerBillingActivityStaging_DataTable(activityStagingList);
                            DataAccess.SilverSneaker.SilverSneakers_BillingActivity_Staging_Update(dtChangedSubset);
                            result.SuccessCount += activityStagingList.Count();

                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            int eventLogID = LogHelper.LogError("Sample.Hello.Exception", ex, false, System.Reflection.MethodInfo.GetCurrentMethod(), batchGUID);
                            DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways GetIneligibleBillingActivity", string.Format("Failed. BatchGUID={0}, EventLogID = {1}", batchGUID, eventLogID));
                            result.ErrorCount += activityStagingList.Count();
                        }
                        catch { }
                    }
                }

                DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways GetIneligibleBillingActivity",
                string.Format("End. {0} batchs. {1} activities. {2} success. {3} fail", activityBatchs.Count, result.ErrorCount + result.SuccessCount, result.SuccessCount, result.ErrorCount));
            }
            catch (Exception e)
            {
                int eventLogID = LogHelper.LogError("Sample.Hello.Exception", e, false, System.Reflection.MethodInfo.GetCurrentMethod());
                string errMsg = string.Format("Failed to get BatchGUID list. EventLogID = {0}", eventLogID);
                DataAccess.SilverSneaker.CancelAppLog_Insert(processID, "Healthways GetIneligibleBillingActivity", errMsg);
                result.ErrorMessage = errMsg;
                result.ErrorCount = -1;
            }
            return result;
        }
    }
}
