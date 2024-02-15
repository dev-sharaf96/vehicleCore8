using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Transactions;
using Tameenk.Common.Utilities;
using Tameenk.Loggin.DAL.Entities.ServiceRequestLogs;

namespace Tameenk.Loggin.DAL
{
    public class ServiceRequestLogDataAccess
    {
        //public static bool AddtoServiceRequestLogs(ServiceRequestLog toSaveLog)
        //{
        //    try
        //    {
        //        using (TameenkLog context = new TameenkLog())
        //        {
        //            context.Database.CommandTimeout = 30;
        //            toSaveLog.CreatedDate = DateTime.Now;
        //            toSaveLog.CreatedOn = DateTime.Now.Date;

        //            context.ServiceRequestLogs.Add(toSaveLog);
        //            context.SaveChanges();
        //            return true;
        //        }
        //    }
        //    catch (DbEntityValidationException dbEx)
        //    {
        //        foreach (var validationErrors in dbEx.EntityValidationErrors)
        //        {
        //            foreach (var validationError in validationErrors.ValidationErrors)
        //            {
        //                ErrorLogger.LogDebug("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
        //            }
        //        }
        //        return false;
        //    }
        //    catch (Exception exp)
        //    {
        //        ErrorLogger.LogError(exp.Message, exp, false);
        //        return false;

        //    }
        //}

        public static bool AddtoServiceRequestLogs(ServiceRequestLog toSaveLog)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    context.Database.CommandTimeout = 60;
                    toSaveLog.CreatedDate = DateTime.Now;
                    toSaveLog.CreatedOn = DateTime.Now.Date;
                    if (toSaveLog.Method.ToLower() == "saudipost")
                    {
                        var log = MapLog<SaudiPostServiceRequestLog>(toSaveLog);
                        context.SaudiPostServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("yakeen"))
                    {
                        var log = MapLog<YakeenServiceRequestLog>(toSaveLog);
                        context.YakeenServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("najm") || toSaveLog.Method.ToLower().Contains("GetDriverCaseDetailV2".ToLower()))
                    {
                        var log = MapLog<NajmServiceRequestLog>(toSaveLog);
                        context.NajmServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("sadadactive") || toSaveLog.Method.ToLower().Contains("sadaddeactive"))
                    {
                        var log = MapLog<PaymentServiceRequestLog>(toSaveLog);
                        context.PaymentServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("getesaltoken")
                        || toSaveLog.Method.ToLower().Contains("saveesalsettlement")
                        || toSaveLog.Method.ToLower().Contains("uploadesalinvoice")
                        || toSaveLog.Method.ToLower().Contains("cancelesalinvoice".ToLower())
                        || toSaveLog.Method.ToLower().Contains("HayperPayUpdateOrder".ToLower())
                        || toSaveLog.Method.ToLower().Contains("SplitLogin".ToLower())
                        || toSaveLog.Method.ToLower().Contains("RequestHyperpayUrlWithSplitOption".ToLower())
                        || toSaveLog.Method.ToLower().Contains("HyperpaySearch".ToLower())
                        || toSaveLog.Method.ToLower().Contains("ApplePayPayment".ToLower())
                        || toSaveLog.Method.ToLower().Contains("StartApplePaySession".ToLower())
                        || toSaveLog.Method.ToLower().Contains("HyperpayToValidateResponse".ToLower())
                        || toSaveLog.Method.ToLower().Contains("Tabby".ToLower()))
                    {
                        var log = MapLog<PaymentServiceRequestLog>(toSaveLog);
                        context.PaymentServiceRequestLogs.Add(log);
                    }

                    else if (toSaveLog.Method.ToLower().Contains("edaat") || toSaveLog.Method.ToLower().Contains("HayperPayUpdateOrder"))
                    {
                        var log = MapLog<PaymentServiceRequestLog>(toSaveLog);
                        context.PaymentServiceRequestLogs.Add(log);
                    }

                    else if (toSaveLog.Method.ToLower().Contains("cancelpolicy") || toSaveLog.Method.ToLower() == "cancelvehiclepolicy")
                    {
                        var log = MapLog<CancellationServiceRequestLog>(toSaveLog);
                        context.CancellationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "ACIG")
                    {
                        var log = MapLog<ACIGPolicyServiceRequestLog>(toSaveLog);
                        context.ACIGPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "ACIG")
                    {
                        var log = MapLog<ACIGQuotationServiceRequestLog>(toSaveLog);
                        context.ACIGQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "Ahlia")
                    {
                        var log = MapLog<AhliaPolicyServiceRequestLog>(toSaveLog);
                        context.AhliaPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "Ahlia")
                    {
                        var log = MapLog<AhliaQuotationServiceRequestLog>(toSaveLog);
                        context.AhliaQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "AICC")
                    {
                        var log = MapLog<AICCPolicyServiceRequestLog>(toSaveLog);
                        context.AICCPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "AICC")
                    {
                        var log = MapLog<AICCQuotationServiceRequestLog>(toSaveLog);
                        context.AICCQuotationServiceRequestLogs.Add(log);
                    }


                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "Alalamiya")
                    {
                        var log = MapLog<AlalamiyaPolicyServiceRequestLog>(toSaveLog);
                        context.AlalamiyaPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "Alalamiya")
                    {
                        var log = MapLog<AlalamiyaQuotationServiceRequestLog>(toSaveLog);
                        context.AlalamiyaQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "AlRajhi")
                    {
                        var log = MapLog<AlRajhiPolicyServiceRequestLog>(toSaveLog);
                        context.AlRajhiPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "AlRajhi")
                    {
                        var log = MapLog<AlRajhiQuotationServiceRequestLog>(toSaveLog);
                        context.AlRajhiQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "ArabianShield")
                    {
                        var log = MapLog<ArabianShieldPolicyServiceRequestLog>(toSaveLog);
                        context.ArabianShieldPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "ArabianShield")
                    {
                        var log = MapLog<ArabianShieldQuotationServiceRequestLog>(toSaveLog);
                        context.ArabianShieldQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "BCARE")
                    {
                        var log = MapLog<ACIGQuotationServiceRequestLog>(toSaveLog);
                        context.ACIGQuotationServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "BCARE")
                    {
                        var log = MapLog<ACIGPolicyServiceRequestLog>(toSaveLog);
                        context.ACIGPolicyServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "GGI")
                    {
                        var log = MapLog<GGIPolicyServiceRequestLog>(toSaveLog);
                        context.GGIPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "GGI")
                    {
                        var log = MapLog<GGIQuotationServiceRequestLog>(toSaveLog);
                        context.GGIQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "GulfUnion")
                    {
                        var log = MapLog<GulfUnionPolicyServiceRequestLog>(toSaveLog);
                        context.GulfUnionPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "GulfUnion")
                    {
                        var log = MapLog<GulfUnionQuotationServiceRequestLog>(toSaveLog);
                        context.GulfUnionQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "Malath")
                    {
                        var log = MapLog<MalathPolicyServiceRequestLog>(toSaveLog);
                        context.MalathPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "Malath")
                    {
                        var log = MapLog<MalathQuotationServiceRequestLog>(toSaveLog);
                        context.MalathQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "MedGulf")
                    {
                        var log = MapLog<MedGulfPolicyServiceRequestLog>(toSaveLog);
                        context.MedGulfPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "MedGulf")
                    {
                        var log = MapLog<MedGulfQuotationServiceRequestLog>(toSaveLog);
                        context.MedGulfQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "UploadComprehansiveImages" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "SAICO")
                    {
                        var log = MapLog<SAICOPolicyServiceRequestLog>(toSaveLog);
                        context.SAICOPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "SAICO")
                    {
                        var log = MapLog<SAICOQuotationServiceRequestLog>(toSaveLog);
                        context.SAICOQuotationServiceRequestLog.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "Salama")
                    {
                        var log = MapLog<SalamaPolicyServiceRequestLog>(toSaveLog);
                        context.SalamaPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "Salama")
                    {
                        var log = MapLog<SalamaQuotationServiceRequestLog>(toSaveLog);
                        context.SalamaQuotationServiceRequestLogs.Add(log);
                    }
                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && (toSaveLog.CompanyName == "Saqr" || toSaveLog.CompanyName == "Sagr"))
                    {
                        var log = MapLog<SaqrPolicyServiceRequestLog>(toSaveLog);
                        context.SaqrPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && (toSaveLog.CompanyName == "Saqr" || toSaveLog.CompanyName == "Sagr"))
                    {
                        var log = MapLog<SaqrQuotationServiceRequestLog>(toSaveLog);
                        context.SaqrQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "Solidarity")
                    {
                        var log = MapLog<SolidarityPolicyServiceRequestLog>(toSaveLog);
                        context.SolidarityPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "Solidarity")
                    {
                        var log = MapLog<SolidarityQuotationServiceRequestLog>(toSaveLog);
                        context.SolidarityQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName.Contains("Tawuniya"))
                    {
                        var log = MapLog<TawuniyaPolicyServiceRequestLog>(toSaveLog);
                        context.TawuniyaPolicyServiceRequestLogs.Add(log);
                    }
                    else if ((toSaveLog.Method.ToLower().Contains("quotation") || toSaveLog.Method.ToLower().Contains("proposal")) && toSaveLog.CompanyName.Contains("Tawuniya"))
                    {
                        var log = MapLog<TawuniyaQuotationServiceRequestLog>(toSaveLog);
                        context.TawuniyaQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "TokioMarine")
                    {
                        var log = MapLog<TokioMarinePolicyServiceRequestLog>(toSaveLog);
                        context.TokioMarinePolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "TokioMarine")
                    {
                        var log = MapLog<TokioMarineQuotationServiceRequestLog>(toSaveLog);
                        context.TokioMarineQuotationServiceRequestLog.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "TUIC")
                    {
                        var log = MapLog<TUICPolicyServiceRequestLog>(toSaveLog);
                        context.TUICPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "TUIC")
                    {
                        var log = MapLog<TUICQuotationServiceRequestLog>(toSaveLog);
                        context.TUICQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "UCA")
                    {
                        var log = MapLog<UCAPolicyServiceRequestLog>(toSaveLog);
                        context.UCAPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "UCA")
                    {
                        var log = MapLog<UCAQuotationServiceRequestLog>(toSaveLog);
                        context.UCAQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "Wafa")
                    {
                        var log = MapLog<WafaPolicyServiceRequestLog>(toSaveLog);
                        context.WafaPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "Wafa")
                    {
                        var log = MapLog<WafaQuotationServiceRequestLog>(toSaveLog);
                        context.WafaQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && (toSaveLog.CompanyName == "Walaa" || toSaveLog.CompanyName == "Wala"))
                    {
                        var log = MapLog<WalaPolicyServiceRequestLog>(toSaveLog);
                        context.WalaPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "Walaa")
                    {
                        var log = MapLog<WalaQuotationServiceRequestLog>(toSaveLog);
                        context.WalaQuotationServiceRequestLogs.Add(log);
                    }

                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "Wataniya")
                    {
                        var log = MapLog<WataniyaPolicyServiceRequestLog>(toSaveLog);
                        context.WataniyaPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "Wataniya")
                    {
                        var log = MapLog<WataniyaQuotationServiceRequestLog>(toSaveLog);
                        context.WataniyaQuotationServiceRequestLogs.Add(log);
                    }
                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "Allianz")
                    {
                        var log = MapLog<AllianzPolicyServiceRequestLog>(toSaveLog);
                        context.AllianzPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "Allianz")
                    {
                        var log = MapLog<AllianzQuotationServiceRequestLog>(toSaveLog);
                        context.AllianzQuotationServiceRequestLogs.Add(log);
                    }
                    else if ((toSaveLog.Method.ToLower().Contains("policy") || toSaveLog.Method.ToLower() == "policyschedule" || toSaveLog.Method.ToLower() == "adddriver" || toSaveLog.Method.ToLower() == "purchasedriver" || toSaveLog.Method.ToLower() == "updatecustomcard" || toSaveLog.Method.ToLower() == "addbenefit" || toSaveLog.Method.ToLower() == "purchasebenefit" || toSaveLog.Method == "AutoleaseUpdateCustomCard" || toSaveLog.Method.ToLower() == "addvehiclebenefit" || toSaveLog.Method.ToLower() == "purchasevehiclebenefit" || toSaveLog.Method.ToLower() == "addvehicledriver" || toSaveLog.Method.ToLower() == "submitvehicleclaimregistrationrequest" || toSaveLog.Method.ToLower() == "submitvehicleclaimnotificationrequest") && toSaveLog.CompanyName == "AXA")
                    {
                        var log = MapLog<AXAPolicyServiceRequestLog>(toSaveLog);
                        context.AXAPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "AXA")
                    {
                        var log = MapLog<AXAQuotationServiceRequestLog>(toSaveLog);
                        context.AXAQuotationServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "Amana")
                    {
                        var log = MapLog<AmanaQuotationServiceRequestLog>(toSaveLog);
                        context.AmanaQuotationServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("policy") && toSaveLog.CompanyName == "Amana")
                    {
                        var log = MapLog<AmanaPolicyServiceRequestLog>(toSaveLog);
                        context.AmanaPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("quotation") && toSaveLog.CompanyName == "Buruj")
                    {
                        var log = MapLog<BurujQuotationServiceRequestLog>(toSaveLog);
                        context.BurujQuotationServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("policy") && toSaveLog.CompanyName == "Buruj")
                    {
                        var log = MapLog<BurujPolicyServiceRequestLog>(toSaveLog);
                        context.BurujPolicyServiceRequestLogs.Add(log);
                    }
                    else if (toSaveLog.Method.ToLower().Contains("tree"))
                    {
                        var log = MapLog<WathqServiceRequestLog>(toSaveLog);
                        context.WathqServiceRequestLogs.Add(log);
                    }
                    else
                    {
                        var log = MapLog<ServiceRequestLog>(toSaveLog);
                        context.ServiceRequestLogs.Add(log);
                    }
                    context.SaveChanges();
                    return true;
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ErrorLogger.LogDebug("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                return false;
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return false;

            }
        }

        public static List<ServiceRequestLog> GetQuotationListForCompany(int commandTimeout, string company)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    context.Database.CommandTimeout = commandTimeout;

                    DateTime startDate = DateTime.Now.Date.AddDays(-1).AddHours(0).AddMinutes(0).AddSeconds(0);// new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 0, 0, 0);
                    DateTime endDate = DateTime.Now.Date.Date.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59); //new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 23, 59, 59);

                    var quotations = (from d in context.ServiceRequestLogs
                                      where
                                       d.CreatedDate >= startDate
                                      && d.CreatedDate <= endDate && d.CompanyName == company &&
                                   d.Method == "Quotation"
                                      orderby d.CreatedDate
                                      select d);

                    List<ServiceRequestLog> quotationList = quotations.ToList<ServiceRequestLog>();

                    if (quotationList.Count > 0)
                        return quotationList;
                    else
                        return null;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return null;

            }
        }


        public static List<ServiceRequestLog> GetQuotationList(int commandTimeout)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    context.Database.CommandTimeout = commandTimeout;


                    DateTime startDate = DateTime.Now.Date.AddDays(-1).AddHours(0).AddMinutes(0).AddSeconds(0);// new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 0, 0, 0);
                    DateTime endDate = DateTime.Now.Date.Date.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59); //new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 23, 59, 59);

                    var quotations = (from d in context.ServiceRequestLogs
                                      where
                                       d.CreatedDate >= startDate
                                      && d.CreatedDate <= endDate &&
                                   d.Method == "Quotation"
                                      orderby d.CreatedDate
                                      select d);

                    List<ServiceRequestLog> quotationList = quotations.ToList<ServiceRequestLog>();

                    return quotationList;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return null;

            }
        }


        public static List<ServiceRequestLog> GetPolicyListForCompany(int commandTimeout, string company)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    context.Database.CommandTimeout = commandTimeout;


                    DateTime startDate = DateTime.Now.Date.AddDays(-1).AddHours(0).AddMinutes(0).AddSeconds(0);// new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 0, 0, 0);
                    DateTime endDate = DateTime.Now.Date.Date.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59); //new DateTime(DateTime.Now.Year, DateTime.Now.Month, startDay, 23, 59, 59);

                    var policies = (from d in context.ServiceRequestLogs
                                    where
                                     d.CreatedDate >= startDate
                                    && d.CreatedDate <= endDate &&
                                    d.Method == "policy" && d.CompanyName == company
                                    orderby d.CreatedDate
                                    select d);

                    List<ServiceRequestLog> policiesList = policies.ToList<ServiceRequestLog>();

                    if (policiesList.Count > 0)
                        return policiesList;
                    else
                        return null;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return null;

            }
        }


        public static ServiceRequestLog GetPolicyByRefernceId(string refernceId)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {

                    var policy = (from d in context.ServiceRequestLogs
                                  where d.ReferenceId == refernceId
                                  && d.Method == "Policy" && d.ErrorCode == 1
                                  orderby d.ID descending
                                  select d).FirstOrDefault();
                    return policy;
                }
            }
            catch (Exception exp)
            {
                ErrorLogger.LogError(exp.Message, exp, false);
                return null;

            }
        }

        public static List<string> GetAllMethodInServiceRequestLog()
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    var methods = (from d in context.ServiceRequestLogs
                                   select d.Method).Distinct().ToList();
                    return methods;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }


        public static List<ServiceRequestLog> GetAllServiceRequestLog()
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    var query = (from d in context.ServiceRequestLogs
                                 select d).Distinct();

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }

        public static List<ServiceRequestLog> GetAllServiceRequestLogBasedOnFilter(string connectionString, ServiceRequestFilter serviceRequestFilter, out int total, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "ID", bool? sortOrder = false)
        {
            try
            {
                TameenkLog context;//= new TameenkLog();

                if (string.IsNullOrEmpty(connectionString))
                    context = new TameenkLog();
                else
                    context = new TameenkLog(connectionString);

                using (context)
                {
                    context.Database.CommandTimeout = 240;
                    context.Configuration.AutoDetectChangesEnabled = false;
                    var query = (from d in context.ServiceRequestLogs select d);
                    //var x = context.ServiceRequestLogs.ToList();
                    if (serviceRequestFilter.EndDate.HasValue && serviceRequestFilter.StartDate.HasValue)
                    {
                        DateTime dtEnd = new DateTime(serviceRequestFilter.EndDate.Value.Year, serviceRequestFilter.EndDate.Value.Month, serviceRequestFilter.EndDate.Value.Day, 23, 59, 59);
                        DateTime dtStart = new DateTime(serviceRequestFilter.StartDate.Value.Year, serviceRequestFilter.StartDate.Value.Month, serviceRequestFilter.StartDate.Value.Day, 0, 0, 0);
                        query = query.Where(e => e.CreatedDate >= dtStart && e.CreatedDate <= dtEnd);
                    }
                    else if (serviceRequestFilter.EndDate.HasValue)
                    {
                        DateTime dtEnd = new DateTime(serviceRequestFilter.EndDate.Value.Year, serviceRequestFilter.EndDate.Value.Month, serviceRequestFilter.EndDate.Value.Day, 23, 59, 59);
                        query = query.Where(e => e.CreatedDate <= serviceRequestFilter.EndDate.Value.Date);

                    }
                    else if (serviceRequestFilter.StartDate.HasValue)
                    {
                        DateTime dtStart = new DateTime(serviceRequestFilter.StartDate.Value.Year, serviceRequestFilter.StartDate.Value.Month, serviceRequestFilter.StartDate.Value.Day, 0, 0, 0);
                        query = query.Where(e => e.CreatedDate >= dtStart);
                    }
                    if (!string.IsNullOrEmpty(serviceRequestFilter.Method))
                        query = query.Where(q => q.Method.Equals(serviceRequestFilter.Method));

                    if (!string.IsNullOrEmpty(serviceRequestFilter.NationalId))
                        query = query.Where(q => q.DriverNin.Equals(serviceRequestFilter.NationalId));

                    if (!string.IsNullOrEmpty(serviceRequestFilter.VehicleId))
                        query = query.Where(q => q.VehicleId.Equals(serviceRequestFilter.VehicleId));

                    if (!string.IsNullOrEmpty(serviceRequestFilter.ReferenceNo))
                        query = query.Where(q => q.ReferenceId.Equals(serviceRequestFilter.ReferenceNo));

                    if (serviceRequestFilter.StatusCode.HasValue)
                    {
                        if (serviceRequestFilter.StatusCode.Value == 1)
                            query = query.Where(q => q.ErrorCode == 1);
                        else
                            query = query.Where(q => q.ErrorCode != 1);
                    }
                    if (serviceRequestFilter.InsuranceCompanyId.HasValue)
                        query = query.Where(q => q.CompanyID == serviceRequestFilter.InsuranceCompanyId);

                    if (!string.IsNullOrEmpty(serviceRequestFilter.PolicyNo))
                        query = query.Where(q => q.PolicyNo.Equals(serviceRequestFilter.PolicyNo));

                    total = query.Count();
                    if (total == 0)
                        return null;
                    int TotalCount = total;
                    int TotalPages = total / pageSize;

                    if (total % pageSize > 0)
                        TotalPages++;

                    if (!string.IsNullOrEmpty(sortField))
                    {
                        if (sortOrder.HasValue)
                        {
                            if (sortOrder.GetValueOrDefault())
                            {
                                query = query.OrderBy("ID DESC");
                            }
                            else
                            {
                                query = query.OrderBy("ID");
                            }
                        }
                    }
                    query = query.Skip(pageIndex * pageSize).Take(pageSize);
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                total = 0;
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }


        public static List<ServiceRequestLog> GetFromServiceRequestLog(string connectionString, ServiceRequestFilter serviceRequestFilter, int pageNumber, int pageSize)
        {
            try
            {
                TameenkLog context;//= new TameenkLog();

                if (string.IsNullOrEmpty(connectionString))
                    context = new TameenkLog();
                else
                    context = new TameenkLog(connectionString);

                using (context)
                {
                    context.Database.CommandTimeout = 240;
                    context.Configuration.AutoDetectChangesEnabled = false;
                    DateTime? startdate = null;
                    DateTime? endDate = null;
                    string method = null;
                    string driverNin = null;
                    string vehicleId = null;
                    string referenceId = null;
                    int? errorCode;
                    int? companyID;
                    string policyNo = null;



                    if (serviceRequestFilter.EndDate.HasValue && serviceRequestFilter.StartDate.HasValue)
                    {
                        startdate = new DateTime(serviceRequestFilter.EndDate.Value.Year, serviceRequestFilter.EndDate.Value.Month, serviceRequestFilter.EndDate.Value.Day, 23, 59, 59);
                        endDate = new DateTime(serviceRequestFilter.StartDate.Value.Year, serviceRequestFilter.StartDate.Value.Month, serviceRequestFilter.StartDate.Value.Day, 0, 0, 0);
                    }
                    else if (serviceRequestFilter.EndDate.HasValue)
                    {
                        endDate = new DateTime(serviceRequestFilter.EndDate.Value.Year, serviceRequestFilter.EndDate.Value.Month, serviceRequestFilter.EndDate.Value.Day, 23, 59, 59);
                    }
                    else if (serviceRequestFilter.StartDate.HasValue)
                    {
                        startdate = new DateTime(serviceRequestFilter.StartDate.Value.Year, serviceRequestFilter.StartDate.Value.Month, serviceRequestFilter.StartDate.Value.Day, 0, 0, 0);
                    }
                    if (!string.IsNullOrEmpty(serviceRequestFilter.Method))
                        method = serviceRequestFilter.Method;

                    if (!string.IsNullOrEmpty(serviceRequestFilter.NationalId))
                        driverNin = serviceRequestFilter.NationalId;

                    if (!string.IsNullOrEmpty(serviceRequestFilter.VehicleId))
                        vehicleId = serviceRequestFilter.VehicleId;

                    if (!string.IsNullOrEmpty(serviceRequestFilter.ReferenceNo))
                        referenceId = serviceRequestFilter.ReferenceNo;

                    if (serviceRequestFilter.StatusCode.HasValue)
                    {
                        errorCode = serviceRequestFilter.StatusCode.Value;
                    }
                    else
                    {
                        errorCode = null;
                    }

                    if (serviceRequestFilter.InsuranceCompanyId.HasValue)
                        companyID = serviceRequestFilter.InsuranceCompanyId.Value;
                    else
                        companyID = null;

                    if (!string.IsNullOrEmpty(serviceRequestFilter.PolicyNo))
                        policyNo = serviceRequestFilter.PolicyNo;


                    var result = context.GetFromServiceRequestLog(startdate, endDate, method, driverNin, vehicleId, referenceId, errorCode, companyID, policyNo, pageNumber, pageSize);
                    if (result != null)
                        return result.ToList();
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }

        #region New Methods

        public static List<IServiceRequestLog> GetAllServiceRequestLogBasedOnFilterNew(string connectionString, string companyKey, ServiceRequestFilter serviceRequestFilter, out int total, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "ID", bool? sortOrder = false)
        {
            try
            {
                using (TameenkLog context = new TameenkLog(connectionString))
                {
                    context.Database.CommandTimeout = 240;
                    context.Configuration.AutoDetectChangesEnabled = false;
                    var query = GetQueryContext(serviceRequestFilter.Method, companyKey, context);
                    if (serviceRequestFilter.EndDate.HasValue && serviceRequestFilter.StartDate.HasValue)
                    {
                        //DateTime dtEnd = new DateTime(serviceRequestFilter.EndDate.Value.Year, serviceRequestFilter.EndDate.Value.Month, serviceRequestFilter.EndDate.Value.Day, 23, 59, 59);
                        //DateTime dtStart = new DateTime(serviceRequestFilter.StartDate.Value.Year, serviceRequestFilter.StartDate.Value.Month, serviceRequestFilter.StartDate.Value.Day, 0, 0, 0);

                        if (serviceRequestFilter.EndDate.Value == serviceRequestFilter.StartDate.Value)
                        {
                            query = query.Where(e => e.CreatedDate >= serviceRequestFilter.StartDate && e.CreatedDate <= serviceRequestFilter.EndDate);
                        }
                        else
                        {
                            query = query.Where(e => e.CreatedDate >= serviceRequestFilter.StartDate && e.CreatedDate <= serviceRequestFilter.EndDate);
                        }
                    }
                    else if (serviceRequestFilter.EndDate.HasValue)
                    {
                        //DateTime dtEnd = new DateTime(serviceRequestFilter.EndDate.Value.Year, serviceRequestFilter.EndDate.Value.Month, serviceRequestFilter.EndDate.Value.Day, 23, 59, 59);
                        query = query.Where(e => e.CreatedDate <= serviceRequestFilter.EndDate.Value.Date);

                    }
                    else if (serviceRequestFilter.StartDate.HasValue)
                    {
                        //DateTime dtStart = new DateTime(serviceRequestFilter.StartDate.Value.Year, serviceRequestFilter.StartDate.Value.Month, serviceRequestFilter.StartDate.Value.Day, 0, 0, 0);
                        query = query.Where(e => e.CreatedDate >= serviceRequestFilter.StartDate);
                    }
                    //if (!string.IsNullOrEmpty(serviceRequestFilter.Method) && serviceRequestFilter.Method.ToLower().Contains("quotation") && companyKey.Contains("Tawuniya"))
                    //    query = query.Where(q => q.Method.Equals(serviceRequestFilter.Method) || q.Method.Equals("Proposal"));

                    //else 

                    if (!string.IsNullOrEmpty(serviceRequestFilter.InsuranceTypeId))
                    {
                        int? typecode = Convert.ToInt32(serviceRequestFilter.InsuranceTypeId);
                        query = query.Where(q => q.InsuranceTypeCode == typecode);
                    }
                    if (!string.IsNullOrEmpty(serviceRequestFilter.Method))
                        query = query.Where(q => q.Method.Equals(serviceRequestFilter.Method));

                    if (!string.IsNullOrEmpty(serviceRequestFilter.NationalId))
                        query = query.Where(q => q.DriverNin.Equals(serviceRequestFilter.NationalId));

                    if (!string.IsNullOrEmpty(serviceRequestFilter.VehicleId))
                        query = query.Where(q => q.VehicleId.Equals(serviceRequestFilter.VehicleId));

                    if (!string.IsNullOrEmpty(serviceRequestFilter.ReferenceNo))
                        query = query.Where(q => q.ReferenceId.Equals(serviceRequestFilter.ReferenceNo));

                    if (serviceRequestFilter.StatusCode.HasValue)
                    {
                        if (serviceRequestFilter.StatusCode.Value == 1)
                            query = query.Where(q => q.ErrorCode == 1);
                        else
                            query = query.Where(q => q.ErrorCode != 1);
                    }
                    if (serviceRequestFilter.InsuranceCompanyId.HasValue)
                        query = query.Where(q => q.CompanyID == serviceRequestFilter.InsuranceCompanyId);

                    if (!string.IsNullOrEmpty(serviceRequestFilter.PolicyNo))
                        query = query.Where(q => q.PolicyNo.Equals(serviceRequestFilter.PolicyNo));

                    total = query.Count();
                    if (total == 0)
                        return null;
                    int TotalCount = total;
                    int TotalPages = total / pageSize;

                    if (total % pageSize > 0)
                        TotalPages++;

                    //if (!string.IsNullOrEmpty(sortField))
                    //{
                    //    if (sortOrder.HasValue)
                    //    {
                    //        if (sortOrder.GetValueOrDefault())
                    //        {
                    //            query = query.OrderBy("ID DESC");
                    //        }
                    //        else
                    //        {
                    //            query = query.OrderBy("ID");
                    //        }
                    //    }
                    //}
                    query = query.OrderByDescending(a => a.CreatedDate);
                    query = query.Skip(pageIndex * pageSize).Take(pageSize);
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                total = 0;
                ErrorLogger.LogError(ex.Message, ex, false);
                //System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\ScheduledTask\logs\ex_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", "error is " + ex.ToString());
                System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\GetAllServiceRequestLogBasedOnFilterNew_Exception" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", ex.ToString());
                return null;
            }
        }

        private static IQueryable<IServiceRequestLog> GetQueryContext(string Method, string CompanyKey, TameenkLog context)
        {
            // IQueryable<IServiceRequestLog> query = null;

            if (Method.ToLower().Contains("najm") || Method.ToLower().Contains("GetDriverCaseDetailV2".ToLower()))
            {
                return (from d in context.NajmServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("yakeen"))
            {
                return (from d in context.YakeenServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower() == "saudipost")
            {
                return (from d in context.SaudiPostServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("sadadactive") || Method.ToLower().Contains("sadaddeactive"))
            {
                return (from d in context.PaymentServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("GetEsalToken") || Method.ToLower().Contains("saveesalsettlement") || Method.ToLower().Contains("uploadesalinvoice") || Method.ToLower().Contains("HayperPayUpdateOrder") || Method.ToLower().Contains("SplitLogin") || Method.Contains("RequestHyperpayUrlWithSplitOption")
                || Method.ToLower().Contains("ApplePayPayment".ToLower()) || Method.ToLower().Contains("StartApplePaySession".ToLower())
                || Method.ToLower().Contains("HyperpayToValidateResponse".ToLower())
                || Method.ToLower().Contains("Tabby".ToLower()))
            {
                return (from d in context.PaymentServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower() == "cancelpolicy" || Method.ToLower() == "cancelvehiclepolicy")
            {
                return (from d in context.CancellationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "ACIG")
            {
                return (from d in context.ACIGPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "ACIG")
            {
                return (from d in context.ACIGQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "Ahlia")
            {
                return (from d in context.AhliaPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "Ahlia")
            {
                return (from d in context.AhliaQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "AICC")
            {
                return (from d in context.AICCPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "AICC")
            {
                return (from d in context.AICCQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "Alalamiya")
            {
                return (from d in context.AlalamiyaPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "Alalamiya")
            {
                return (from d in context.AlalamiyaQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "AlRajhi")
            {
                return (from d in context.AlRajhiPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "AlRajhi")
            {
                return (from d in context.AlRajhiQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "ArabianShield")
            {
                return (from d in context.ArabianShieldPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "ArabianShield")
            {
                return (from d in context.ArabianShieldQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "BCARE")
            {
                return (from d in context.ACIGQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "BCARE")
            {
                return (from d in context.ACIGPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "GGI")
            {
                return (from d in context.GGIPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "GGI")
            {
                return (from d in context.GGIQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "GulfUnion")
            {
                return (from d in context.GulfUnionPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "GulfUnion")
            {
                return (from d in context.GulfUnionQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "Malath")
            {
                return (from d in context.MalathPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "Malath")
            {
                return (from d in context.MalathQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "MedGulf")
            {
                return (from d in context.MedGulfPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "MedGulf")
            {
                return (from d in context.MedGulfQuotationServiceRequestLogs.AsNoTracking() select d);
            }

            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "UploadComprehansiveImages" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "SAICO")
            {
                return (from d in context.SAICOPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "SAICO")
            {
                return (from d in context.SAICOQuotationServiceRequestLog.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "Salama")
            {
                return (from d in context.SalamaPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "Salama")
            {
                return (from d in context.SalamaQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && (CompanyKey == "Saqr" || CompanyKey == "Sagr"))
            {
                return (from d in context.SaqrPolicyServiceRequestLogs select d);
            }
            else if (Method.ToLower().Contains("quotation") && (CompanyKey == "Saqr" || CompanyKey == "Sagr"))
            {
                return (from d in context.SaqrQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "Solidarity")
            {
                return (from d in context.SolidarityPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "Solidarity")
            {
                return (from d in context.SolidarityQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey.Contains("Tawuniya"))
            {
                return (from d in context.TawuniyaPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("quotation") || Method.ToLower().Contains("proposal")) && CompanyKey.Contains("Tawuniya"))
            {
                return (from d in context.TawuniyaQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "TokioMarine")
            {
                return (from d in context.TokioMarinePolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "TokioMarine")
            {
                return (from d in context.TokioMarineQuotationServiceRequestLog.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "TUIC")
            {
                return (from d in context.TUICPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "TUIC")
            {
                return (from d in context.TUICQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "UCA")
            {
                return (from d in context.UCAPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "UCA")
            {
                return (from d in context.UCAQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "Wafa")
            {
                return (from d in context.WafaPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "Wafa")
            {
                return (from d in context.WafaQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "Walaa")
            {
                return (from d in context.WalaPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "Walaa")
            {
                return (from d in context.WalaQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "Wataniya")
            {
                return (from d in context.WataniyaPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "Wataniya")
            {
                return (from d in context.WataniyaQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "Allianz")
            {
                return (from d in context.AllianzPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "Allianz")
            {
                return (from d in context.AllianzQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "AXA")
            {
                return (from d in context.AXAPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "AXA")
            {
                return (from d in context.AXAQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "Amana")
            {
                return (from d in context.AmanaPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "Amana")
            {
                return (from d in context.AmanaQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if ((Method.ToLower().Contains("policy") || Method.ToLower() == "policyschedule" || Method.ToLower() == "updatecustomcard" || Method.ToLower() == "addbenefit" || Method.ToLower() == "purchasebenefit" || Method.ToLower() == "adddriver" || Method.ToLower() == "purchasedriver" || Method == "AutoleaseUpdateCustomCard" || Method.ToLower() == "addvehiclebenefit" || Method.ToLower() == "purchasevehiclebenefit" || Method.ToLower() == "addvehicledriver" || Method.ToLower() == "submitvehicleclaimregistrationrequest" || Method.ToLower() == "submitvehicleclaimnotificationrequest") && CompanyKey == "Buruj")
            {
                return (from d in context.BurujPolicyServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("quotation") && CompanyKey == "Buruj")
            {
                return (from d in context.BurujQuotationServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("edaat"))
            {
                return (from d in context.PaymentServiceRequestLogs.AsNoTracking() select d);
            }
            else if (Method.ToLower().Contains("tree"))
            {
                return (from d in context.WathqServiceRequestLogs.AsNoTracking() select d);
            }
            else
            {
                return (from d in context.ServiceRequestLogs.AsNoTracking() select d);

            }
            // return query;
        }

        private static TLog MapLog<TLog>(IServiceRequestLog log) where TLog : IServiceRequestLog, new()
        {
            var saveLog = new TLog();
            saveLog.Channel = log.Channel;
            saveLog.CompanyID = log.CompanyID;
            saveLog.CompanyName = log.CompanyName;
            saveLog.CreatedDate = log.CreatedDate;
            saveLog.CreatedOn = log.CreatedOn;
            saveLog.DriverNin = log.DriverNin;
            saveLog.ErrorCode = log.ErrorCode;
            saveLog.ErrorDescription = log.ErrorDescription;
            saveLog.ID = log.ID;
            saveLog.InsuranceTypeCode = log.InsuranceTypeCode;
            saveLog.Method = log.Method;
            saveLog.PolicyNo = log.PolicyNo;
            saveLog.ReferenceId = log.ReferenceId;
            saveLog.RequestId = log.RequestId;
            saveLog.ServerIP = log.ServerIP;
            saveLog.ServiceErrorCode = log.ServiceErrorCode;
            saveLog.ServiceErrorDescription = log.ServiceErrorDescription;
            saveLog.ServiceRequest = log.ServiceRequest;
            saveLog.ServiceResponse = log.ServiceResponse;
            saveLog.ServiceResponseTimeInSeconds = log.ServiceResponseTimeInSeconds;
            saveLog.ServiceURL = log.ServiceURL;
            saveLog.UserID = log.UserID;
            saveLog.UserName = log.UserName;
            saveLog.VehicleId = log.VehicleId;
            saveLog.VehicleMaker = log.VehicleMaker;
            saveLog.VehicleMakerCode = log.VehicleMakerCode;
            saveLog.VehicleModel = log.VehicleModel;
            saveLog.VehicleModelCode = log.VehicleModelCode;
            saveLog.VehicleModelYear = log.VehicleModelYear;
            saveLog.ExternalId = log.ExternalId;
            saveLog.VehicleAgencyRepair = log.VehicleAgencyRepair;
            saveLog.City = log.City;
            saveLog.ChassisNumber = log.ChassisNumber;
            return saveLog;
        }

        public static List<string> GetAllMethodInServiceRequestLogNew()
        {
            try
            {
                var list = new List<string>();
                list.Add("CancelEsalInvoice");
                list.Add("CancelPolicy");
                list.Add("GetEsalToken");
                list.Add("Najm");
                list.Add("Policy");
                list.Add("Draft Policy");
                list.Add("AutoleasingDraftPolicy");
                list.Add("policyschedule");
                list.Add("AutoleasingPolicy");
                list.Add("Proposal");
                list.Add("AutoleasingProposal");
                list.Add("Quotation");
                list.Add("AutoleasingQuotation");
                list.Add("SadadActive");
                list.Add("SadadDeactive");
                list.Add("saudiPost");
                list.Add("SaveEsalSettlement");
                list.Add("UploadComprehansiveImages");
                list.Add("UploadEsalInvoice");
                list.Add("ValidateBeforeJoinProgram");
                // list.Add("YakeenAddress");
                list.Add("Yakeen-getAlienInfoByIqama");
                list.Add("Yakeen-getCarInfoByCustom");
                list.Add("Yakeen-getCarInfoBySequence");
                list.Add("Yakeen-getCitizenIDInfo");
                list.Add("Yakeen-GetVehiclePlateInfo");
                list.Add("Yakeen-GetAlienAddress");
                list.Add("Yakeen-getCitizenNatAddress");
                list.Add("Yakeen-getCarInfoByCustomTwo");
                list.Add("Morni");
                list.Add("GetEdaatToken");
                list.Add("GetEdaatSubBillerRegisterationNo");
                list.Add("SubmitEdaatRequest");
                list.Add("UpdateCustomCard");
                list.Add("AddBenefit");
                list.Add("PurchaseBenefit");
                list.Add("AddDriver");
                list.Add("PurchaseDriver");
                list.Add("HayperPayUpdateOrder");
                list.Add("SplitLogin");
                list.Add("RequestHyperpayUrlWithSplitOption");
                list.Add("StartApplePaySession");
                list.Add("ApplePayPayment");
                list.Add("HyperpayToValidateResponse");
                list.Add("AutoleaseUpdateCustomCard");

                list.Add("AddVehicleDriver");
                list.Add("PurchaseVehicleDriver");                list.Add("AddVehicleBenefit");
                list.Add("PurchaseVehicleBenefit");                list.Add("SubmitVehicleClaimRegistrationRequest");                list.Add("SubmitVehicleClaimNotificationRequest");                list.Add("CancelVehiclePolicy");
                list.Add("GetDriverCaseDetailV2");
                list.Add("SubmitTabbyRequest");
                list.Add("SubmitTabbyCaptureRequest");                list.Add("ValidateIBAN");                list.Add("TabbyPaymentWebHook");                list.Add("Yakeen-getYakeenMobileVerification");                list.Add("Tree");                return list;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }


        public static List<ServiceRequestLog> GetAllServiceRequestLogWithOnFilterNew(string connectionString, string companyKey, ServiceRequestFilter serviceRequestFilter, out int total, out string exception, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "ID", bool? sortOrder = false, bool export = false)
        {
            exception = string.Empty;
            try
            {
                using (TameenkLog context = new TameenkLog(connectionString))
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "SP_ServiceRequest";
                    command.CommandType = CommandType.StoredProcedure;
                    context.Database.CommandTimeout = 180;

                    SqlParameter referenceParameter = new SqlParameter() { ParameterName = "reference", Value = (!string.IsNullOrEmpty(serviceRequestFilter.ReferenceNo)) ? serviceRequestFilter.ReferenceNo : null };
                    command.Parameters.Add(referenceParameter);

                    SqlParameter companyKeyParameter = new SqlParameter() { ParameterName = "companyKey", Value = (!string.IsNullOrEmpty(companyKey)) ? companyKey : null };
                    command.Parameters.Add(companyKeyParameter);

                    SqlParameter methodParameter = new SqlParameter() { ParameterName = "method", Value = (!string.IsNullOrEmpty(serviceRequestFilter.Method)) ? serviceRequestFilter.Method : null };
                    command.Parameters.Add(methodParameter);

                    SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDateTime", Value = (serviceRequestFilter.StartDate.HasValue) ? serviceRequestFilter.StartDate.Value.ToString("yyyy-MM-dd 00:00:00") : null };
                    command.Parameters.Add(startDateParameter);

                    SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDateTime", Value = (serviceRequestFilter.EndDate.HasValue) ? serviceRequestFilter.EndDate.Value.ToString("yyyy-MM-dd 23:59:59") : null };
                    command.Parameters.Add(endDateParameter);

                    SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "nationalId", Value = (!string.IsNullOrWhiteSpace(serviceRequestFilter.NationalId)) ? serviceRequestFilter.NationalId : null };
                    command.Parameters.Add(NationalIdParameter);

                    SqlParameter vehicleIdParameter = new SqlParameter() { ParameterName = "vehicleId", Value = (!string.IsNullOrWhiteSpace(serviceRequestFilter.VehicleId)) ? serviceRequestFilter.VehicleId : null };
                    command.Parameters.Add(vehicleIdParameter);

                    SqlParameter PolicyNoParameter = new SqlParameter() { ParameterName = "policyNo", Value = (!string.IsNullOrWhiteSpace(serviceRequestFilter.PolicyNo)) ? serviceRequestFilter.PolicyNo : null };
                    command.Parameters.Add(PolicyNoParameter);

                    SqlParameter statusCodeParameter = new SqlParameter() { ParameterName = "statusCode", Value = serviceRequestFilter.StatusCode ?? null };
                    command.Parameters.Add(statusCodeParameter);

                    SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = (pageIndex > 0) ? pageIndex : 1 };
                    command.Parameters.Add(pageNumberParameter);

                    SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                    command.Parameters.Add(pageSizeParameter);

                    SqlParameter exportParameter = new SqlParameter() { ParameterName = "export", Value = (export) ? 1 : 0 };
                    command.Parameters.Add(exportParameter);

                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();

                    // get policy filteration data
                    List<ServiceRequestLog> filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<ServiceRequestLog>(reader).ToList();

                    reader.NextResult();
                    total = ((IObjectContextAdapter)context).ObjectContext.Translate<int>(reader).FirstOrDefault();

                    return filteredData;
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                total = 0;
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }

        public static List<IServiceRequestLog> GetServiceRequestLog(string companyKey, string method, int insuranceTypeCode, DateTime StartDate, DateTime endDate, bool isAutoLease, int errorCode, out string exception)
        {
            try
            {
                exception = string.Empty;
                using (new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                    }))
                {
                    using (TameenkLog context = new TameenkLog())
                    {
                        context.Database.CommandTimeout = 90;
                        context.Configuration.AutoDetectChangesEnabled = false;
                        var query = GetQueryContext(method, companyKey, context);
                        query = query.Where(q => q.Method == method);
                        query = query.Where(q => q.ErrorCode == errorCode);
                        query = query.Where(e => e.CreatedDate >= StartDate && e.CreatedDate <= endDate);
                        query = query.Where(q => q.InsuranceTypeCode == insuranceTypeCode);
                        if (isAutoLease)
                        {
                            query = query.Where(q => q.Channel == "autoleasing");
                        }
                        query = query.OrderByDescending(a => a.ID);
                        query = query.Skip(0).Take(1);
                        return query.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }

        public static List<IServiceRequestLog> GetServiceStatusFromServiceRequestLog(string method, int errorCode, DateTime StartDate, DateTime endDate, out string exception)
        {
            try
            {
                exception = string.Empty;
                using (new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                    }))
                {
                    using (TameenkLog context = new TameenkLog())
                    {
                        context.Database.CommandTimeout = 90;
                        context.Configuration.AutoDetectChangesEnabled = false;
                        var query = GetQueryContext(method, string.Empty, context);
                        query = query.Where(q => q.Method == method);
                        query = query.Where(q => q.ErrorCode == errorCode);
                        query = query.Where(e => e.CreatedDate >= StartDate && e.CreatedDate <= endDate);
                        query = query.OrderByDescending(a => a.ID);
                        query = query.Skip(0).Take(1);
                        return query.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }

        public static IServiceRequestLog GetFromPolicyRequestLog(string companyKey, string referenceId)
        {
            try
            {
                using (TameenkLog context = new TameenkLog())
                {
                    context.Database.CommandTimeout = 240;
                    context.Configuration.AutoDetectChangesEnabled = false;
                    var query = GetQueryContext("policy", companyKey, context);
                    return query.Where(a => a.ReferenceId == referenceId && a.ErrorCode == 1).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.Message, ex, false);
                return null;
            }
        }

        public static List<TabbyWebHookServiceRequestLogModel> GetAllServiceRequestLogBasedOnFilterForTabby(string connectionString, string companyKey, ServiceRequestFilter serviceRequestFilter, out int total, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            using (TameenkLog context = new TameenkLog(connectionString))
            {
                try
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "TabbyWebHookServiceRequestLog";
                    command.CommandType = CommandType.StoredProcedure;
                    context.Database.CommandTimeout = 180;

                    if (serviceRequestFilter.StartDate.HasValue)
                    {
                        DateTime startDate = new DateTime(serviceRequestFilter.StartDate.Value.Year, serviceRequestFilter.StartDate.Value.Month, serviceRequestFilter.StartDate.Value.Day, 0, 0, 0);
                        SqlParameter StartDateParameter = new SqlParameter() { ParameterName = "startDateTime", Value = startDate };
                        command.Parameters.Add(StartDateParameter);
                    }

                    if (serviceRequestFilter.EndDate.HasValue)
                    {
                        DateTime endDate = new DateTime(serviceRequestFilter.EndDate.Value.Year, serviceRequestFilter.EndDate.Value.Month, serviceRequestFilter.EndDate.Value.Day, 23, 59, 59);
                        SqlParameter EndDateParameter = new SqlParameter() { ParameterName = "endDateTime", Value = endDate };
                        command.Parameters.Add(EndDateParameter);
                    }

                    if (!string.IsNullOrEmpty(serviceRequestFilter.NationalId))
                    {
                        SqlParameter NationalIdParameter = new SqlParameter() { ParameterName = "nationalId", Value = serviceRequestFilter.NationalId };
                        command.Parameters.Add(NationalIdParameter);
                    }

                    if (!string.IsNullOrEmpty(serviceRequestFilter.VehicleId))
                    {
                        SqlParameter vehicleIdParameter = new SqlParameter() { ParameterName = "vehicleId", Value = serviceRequestFilter.VehicleId };
                        command.Parameters.Add(vehicleIdParameter);
                    }

                    if (!string.IsNullOrEmpty(serviceRequestFilter.ReferenceNo))
                    {
                        SqlParameter referenceParameter = new SqlParameter() { ParameterName = "reference", Value = serviceRequestFilter.ReferenceNo };
                        command.Parameters.Add(referenceParameter);
                    }

                    SqlParameter successParameter = new SqlParameter() { ParameterName = "isSuccess", Value = null };
                    SqlParameter errorParameter = new SqlParameter() { ParameterName = "isError", Value = null };
                    if (serviceRequestFilter.StatusCode.HasValue)
                    {
                        if (serviceRequestFilter.StatusCode.Value == 1)
                            successParameter.Value = 1;
                        else
                            errorParameter.Value = 1;
                    }

                    command.Parameters.Add(successParameter);
                    command.Parameters.Add(errorParameter);

                    if (serviceRequestFilter.InsuranceCompanyId.HasValue)
                    {
                        SqlParameter companyIdParameter = new SqlParameter() { ParameterName = "companyId", Value = serviceRequestFilter.InsuranceCompanyId };
                        command.Parameters.Add(companyIdParameter);
                    }

                    SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = (pageIndex > 0) ? pageIndex : 1 };
                    command.Parameters.Add(pageNumberParameter);

                    SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                    command.Parameters.Add(pageSizeParameter);

                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();

                    List<TabbyWebHookServiceRequestLogModel> filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<TabbyWebHookServiceRequestLogModel>(reader).ToList();

                    reader.NextResult();
                    total = ((IObjectContextAdapter)context).ObjectContext.Translate<int>(reader).FirstOrDefault();

                    return filteredData;
                }
                catch (Exception ex)
                {
                    total = 0;
                    ErrorLogger.LogError(ex.Message, ex, false);
                    System.IO.File.WriteAllText(@"C:\inetpub\WataniyaLog\GetAllServiceRequestLogBasedOnFilterForTabby_Exception" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_mms") + ".txt", ex.ToString());
                    return null;
                }
                finally
                {
                    if (context.Database.Connection.State == ConnectionState.Open)
                        context.Database.Connection.Close();
                }
            }
        }

        public static List<FirebaseNotificationLog> GetAllAppNotificationLogBasedOnFilter(string connectionString, AppNotificationsLogFilterModel filter, out int total, out string exception, int pageIndex = 0, int pageSize = int.MaxValue, string sortField = "Id", bool? sortOrder = false)        {
            total = 0;            exception = string.Empty;

            using (TameenkLog context = new TameenkLog(connectionString))
            {
                try
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "GetAllAppNotificationLogsFromDBWithFilter";
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 180;

                    if (filter.StartDate.HasValue)
                    {
                        DateTime startDate = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, filter.StartDate.Value.Day, 0, 0, 0);
                        SqlParameter startDateParameter = new SqlParameter() { ParameterName = "startDateTime", Value = startDate };
                        command.Parameters.Add(startDateParameter);
                    }

                    if (filter.EndDate.HasValue)
                    {
                        DateTime endDate = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, filter.EndDate.Value.Day, 23, 59, 59);
                        SqlParameter endDateParameter = new SqlParameter() { ParameterName = "endDateTime", Value = endDate };
                        command.Parameters.Add(endDateParameter);
                    }

                    if (!string.IsNullOrWhiteSpace(filter.ReferenceId))
                    {
                        SqlParameter MobileNoParameter = new SqlParameter() { ParameterName = "referenceId", Value = filter.ReferenceId };
                        command.Parameters.Add(MobileNoParameter);
                    }

                    if (filter.Channel.HasValue && filter.Channel.Value > 0)
                    {
                        string channel = filter.Channel != null ? Enum.GetName(typeof(Channel), filter.Channel) : null;
                        SqlParameter StatusCodeParameter = new SqlParameter() { ParameterName = "channel", Value = channel };
                        command.Parameters.Add(StatusCodeParameter);
                    }

                    SqlParameter successParameter = new SqlParameter() { ParameterName = "isSuccess", Value = null };
                    SqlParameter errorParameter = new SqlParameter() { ParameterName = "isError", Value = null };
                    if (filter.StatusCode.HasValue)
                    {
                        if (filter.StatusCode.Value == 1)
                            successParameter.Value = 1;
                        else
                            errorParameter.Value = 1;
                    }
                    command.Parameters.Add(successParameter);
                    command.Parameters.Add(errorParameter);

                    SqlParameter pageNumberParameter = new SqlParameter() { ParameterName = "pageNumber", Value = (pageIndex > 0) ? pageIndex : 1 };
                    command.Parameters.Add(pageNumberParameter);

                    SqlParameter pageSizeParameter = new SqlParameter() { ParameterName = "pageSize", Value = pageSize };
                    command.Parameters.Add(pageSizeParameter);

                    SqlParameter ExportParameter = new SqlParameter() { ParameterName = "export", Value = filter.Export };
                    command.Parameters.Add(ExportParameter);

                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();

                    List<FirebaseNotificationLog> filteredData = ((IObjectContextAdapter)context).ObjectContext.Translate<FirebaseNotificationLog>(reader).ToList();
                    if (!filter.Export)
                    {
                        reader.NextResult();
                        total = ((IObjectContextAdapter)context).ObjectContext.Translate<int>(reader).FirstOrDefault();
                    }

                    return filteredData;
                }
                catch (Exception ex)
                {
                    total = 0;
                    exception = ex.ToString();
                    return null;
                }
                finally
                {
                    if (context.Database.Connection.State == ConnectionState.Open)
                        context.Database.Connection.Close();
                }
            }        }

        #endregion

        public static ServiceRequestLog CheckForGetVehicleInfoByCustomTwoBefore(string connectionString, string vehicleId, bool export, out string exception)
        {
            exception = string.Empty;
            try
            {
                using (TameenkLog context = new TameenkLog(connectionString))
                {
                    var command = context.Database.Connection.CreateCommand();
                    command.CommandText = "CheckForGetVehicleInfoByCustomTwoBefore";
                    command.CommandType = CommandType.StoredProcedure;
                    context.Database.CommandTimeout = 180;

                    SqlParameter vehicleIdParameter = new SqlParameter() { ParameterName = "vehicleId", Value = vehicleId };
                    command.Parameters.Add(vehicleIdParameter);

                    context.Database.Connection.Open();
                    var reader = command.ExecuteReader();

                    ServiceRequestLog data = ((IObjectContextAdapter)context).ObjectContext.Translate<ServiceRequestLog>(reader).FirstOrDefault();
                    return data;
                }
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
                return null;
            }
        }
    }
}
