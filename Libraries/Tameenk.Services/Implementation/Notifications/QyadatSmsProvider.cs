using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Tameenk.Common.Utilities;
using Tameenk.Core.Domain.Enums;
using Tameenk.Integration.Dto;
using Tameenk.Loggin.DAL;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Core.Notifications;
using Tameenk.Services.Repository;

namespace Tameenk.Services.Implementation.Notifications
{
    public class QyadatSmsProvider : ISmsProvider
    {
        private readonly IHttpClient _httpClient;

        public QyadatSmsProvider(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task SendSmsAsync(string phoneNumber, string message, string method)
        {
            //SMSLog log = new SMSLog();
            //log.Method = method;
            //try
            //{
            //    phoneNumber = validatePhoneNumber(phoneNumber);            //    log.MobileNumber = phoneNumber;            //    log.SMSMessage = message;
            //    var content = new StringContent("{  \"userName\": \"" + RepositoryConstants.STCSmsAccountUsername + "\",  \"numbers\": \"" + phoneNumber + "\",  \"userSender\": \"" + RepositoryConstants.STCSmsAccountSender + "\",  \"apiKey\": \"" + RepositoryConstants.STCSmsApiKey + "\",  \"msg\": \"" + message + "\"}", System.Text.Encoding.UTF8, "application/json");
            //    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            //    var response = await _httpClient.PostAsync(RepositoryConstants.STCSmsServiceUrl, content);
            //    if (response == null || response.Content == null)            //    {            //        log.ErrorCode = 13;            //        log.ErrorDescription = "response from STCSmsProvider is null";            //    }            //    else            //    {            //        var responseString = await response.Content?.ReadAsStringAsync();            //        STCSmsResponse stcResponse = JsonConvert.DeserializeObject<STCSmsResponse>(responseString);
            //        switch (stcResponse.Code)            //        {            //            case "1":            //            case "M0000": //Success
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.Success;            //                    break;            //                }            //            case "M0001":            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.VariablesMissing;            //                    break;            //                }            //            case "1010"://Variables missing
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.VariablesMissing;            //                    break;            //                }            //            case "M0002":            //            case "1020"://Invalid login info
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.InvalidLoginInfo;            //                    break;            //                }            //            case "M0022": //Exceed number of senders allowed
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.ExceedNumberOfSendersAllowed;            //                    break;            //                }            //            case "M0023": //Exceed number of senders allowed
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.SenderNameIsActiveOrUnderActivationOrRefused;            //                    break;            //                }            //            case "M0024": //Sender Name should be in English or number
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.SenderNameShouldBeInEnglishOrNumber;            //                    break;            //                }            //            case "M0025": //Invalid Sender Name Length
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.InvalidSenderNameLength;            //                    break;            //                }            //            case "M0026": //Sender Name is already activated or not found
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.SenderNameIsAlreadyActivatedOrNotFound;            //                    break;            //                }            //            case "M0027": //Activation Code is not Correct
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.ActivationCodeIsNotCorrect;            //                    break;            //                }            //            case "M0028": //You reach maximum number of attempts. Sender name is locked
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.YouReachMaximumNumberOfAttemptsSenderNameIsLocked;            //                    break;            //                }            //            case "M0050": //MSG body is empty
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.MsgBodyIsEmpty;            //                    break;            //                }            //            case "M0060": //Balance is not enough
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.BalanceIsNotEnough;            //                    break;            //                }            //            case "M0061": //MSG duplicated
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.MsgDuplicated;            //                    break;            //                }            //            case "1110": //Sender name is missing or incorrect
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.SenderNameIsMissingOrIncorrect;            //                    break;            //                }            //            case "1120": //Mobile numbers is not correct
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.MobileNumbersIsNotCorrect;            //                    break;            //                }            //            case "1140": //MSG length is too long
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.MsgLengthIsTooLong;            //                    break;            //                }            //            case "M0029": //Invalid Sender Name
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.InvalidSenderName;            //                    break;            //                }            //            case "M0030": //Sender Name should ended with AD
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.SenderNameShouldEndedWithAD;            //                    break;            //                }            //            case "M0031": //Maximum allowed size of uploaded file is 5 MB
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.MaximumAllowedSizeOfUploadedFileIs5MB;            //                    break;            //                }            //            case "M0032": //Only pdf,png,jpg and jpeg files are allowed!
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.FileExtensionNotAllowed;            //                    break;            //                }            //            case "M0033": //Sender Type should be normal or whitelist only
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.SenderTypeShouldBeNormalOrWhitelistOnly;            //                    break;            //                }            //            case "M0034": //Please Use POST Method
            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.PleaseUsePOSTMethod;            //                    break;            //                }            //            default:            //                {            //                    log.ErrorCode = (int)ESTCSmsResponse.GenericError;            //                    break;            //                }            //        }            //        log.ErrorDescription = stcResponse.Message;            //    }
            //    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);            //}
            //catch (Exception exp)
            //{
            //    log.ErrorCode = 12;
            //    log.ErrorDescription = exp.ToString();
            //    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
            //}
        }

        

        public bool SendSmsSTC(string phoneNumber, string message, string method, out string exception)        {            exception = string.Empty;            SMSLog log = new SMSLog();            log.Method = method;            try            {                phoneNumber = validatePhoneNumber(phoneNumber);                log.MobileNumber = phoneNumber;                log.SMSMessage = message;
                var content = new StringContent("{  \"userName\": \"" + RepositoryConstants.STCSmsAccountUsername + "\",  \"numbers\": \"" + phoneNumber + "\",  \"userSender\": \"" + RepositoryConstants.STCSmsAccountSender + "\",  \"apiKey\": \"" + RepositoryConstants.STCSmsApiKey + "\",  \"msg\": \"" + message + "\"}", System.Text.Encoding.UTF8, "application/json");
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                var response = _httpClient.Post(RepositoryConstants.STCSmsServiceUrl, content);
                if (response == null || response.Content == null)                {                    log.ErrorCode = 13;                    log.ErrorDescription = "response from STCSmsProvider is null";                }                else                {                    var responseString = response.Content?.ReadAsStringAsync().Result;                    STCSmsResponse stcResponse = JsonConvert.DeserializeObject<STCSmsResponse>(responseString);
                    switch (stcResponse.Code)                    {                        case "1":                        case "M0000": //Success
                            {                                log.ErrorCode = (int)ESTCSmsResponse.Success;                                break;                            }                        case "M0001":                            {                                log.ErrorCode = (int)ESTCSmsResponse.VariablesMissing;                                break;                            }                        case "1010"://Variables missing
                            {                                log.ErrorCode = (int)ESTCSmsResponse.VariablesMissing;                                break;                            }                        case "M0002":                        case "1020"://Invalid login info
                            {                                log.ErrorCode = (int)ESTCSmsResponse.InvalidLoginInfo;                                break;                            }                        case "M0022": //Exceed number of senders allowed
                            {                                log.ErrorCode = (int)ESTCSmsResponse.ExceedNumberOfSendersAllowed;                                break;                            }                        case "M0023": //Exceed number of senders allowed
                            {                                log.ErrorCode = (int)ESTCSmsResponse.SenderNameIsActiveOrUnderActivationOrRefused;                                break;                            }                        case "M0024": //Sender Name should be in English or number
                            {                                log.ErrorCode = (int)ESTCSmsResponse.SenderNameShouldBeInEnglishOrNumber;                                break;                            }                        case "M0025": //Invalid Sender Name Length
                            {                                log.ErrorCode = (int)ESTCSmsResponse.InvalidSenderNameLength;                                break;                            }                        case "M0026": //Sender Name is already activated or not found
                            {                                log.ErrorCode = (int)ESTCSmsResponse.SenderNameIsAlreadyActivatedOrNotFound;                                break;                            }                        case "M0027": //Activation Code is not Correct
                            {                                log.ErrorCode = (int)ESTCSmsResponse.ActivationCodeIsNotCorrect;                                break;                            }                        case "M0028": //You reach maximum number of attempts. Sender name is locked
                            {                                log.ErrorCode = (int)ESTCSmsResponse.YouReachMaximumNumberOfAttemptsSenderNameIsLocked;                                break;                            }                        case "M0050": //MSG body is empty
                            {                                log.ErrorCode = (int)ESTCSmsResponse.MsgBodyIsEmpty;                                break;                            }                        case "M0060": //Balance is not enough
                            {                                log.ErrorCode = (int)ESTCSmsResponse.BalanceIsNotEnough;                                break;                            }                        case "M0061": //MSG duplicated
                            {                                log.ErrorCode = (int)ESTCSmsResponse.MsgDuplicated;                                break;                            }                        case "1110": //Sender name is missing or incorrect
                            {                                log.ErrorCode = (int)ESTCSmsResponse.SenderNameIsMissingOrIncorrect;                                break;                            }                        case "1120": //Mobile numbers is not correct
                            {                                log.ErrorCode = (int)ESTCSmsResponse.MobileNumbersIsNotCorrect;                                break;                            }                        case "1140": //MSG length is too long
                            {                                log.ErrorCode = (int)ESTCSmsResponse.MsgLengthIsTooLong;                                break;                            }                        case "M0029": //Invalid Sender Name
                            {                                log.ErrorCode = (int)ESTCSmsResponse.InvalidSenderName;                                break;                            }                        case "M0030": //Sender Name should ended with AD
                            {                                log.ErrorCode = (int)ESTCSmsResponse.SenderNameShouldEndedWithAD;                                break;                            }                        case "M0031": //Maximum allowed size of uploaded file is 5 MB
                            {                                log.ErrorCode = (int)ESTCSmsResponse.MaximumAllowedSizeOfUploadedFileIs5MB;                                break;                            }                        case "M0032": //Only pdf,png,jpg and jpeg files are allowed!
                            {                                log.ErrorCode = (int)ESTCSmsResponse.FileExtensionNotAllowed;                                break;                            }                        case "M0033": //Sender Type should be normal or whitelist only
                            {                                log.ErrorCode = (int)ESTCSmsResponse.SenderTypeShouldBeNormalOrWhitelistOnly;                                break;                            }                        case "M0034": //Please Use POST Method
                            {                                log.ErrorCode = (int)ESTCSmsResponse.PleaseUsePOSTMethod;                                break;                            }                        default:                            {                                log.ErrorCode = (int)ESTCSmsResponse.GenericError;                                break;                            }                    }                    log.ErrorDescription = stcResponse.Message;                }
                SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                return true;            }            catch (Exception exp)            {                exception = exp.ToString();                log.ErrorCode = 12;                log.ErrorDescription = exp.ToString();                SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                return false;            }        }
        private string validatePhoneNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith(RepositoryConstants.InternationalPhoneCode))
                phoneNumber = phoneNumber.Substring(RepositoryConstants.InternationalPhoneCode.Length);
            else if (phoneNumber.StartsWith(RepositoryConstants.InternationalPhoneSymbol))
                phoneNumber = phoneNumber.Substring(RepositoryConstants.InternationalPhoneSymbol.Length);

            if (!phoneNumber.StartsWith(RepositoryConstants.SaudiInternationalPhoneCode))
            {
                if (phoneNumber.StartsWith(RepositoryConstants.Zero))
                    phoneNumber = phoneNumber.Substring(RepositoryConstants.Zero.Length);

                phoneNumber = RepositoryConstants.SaudiInternationalPhoneCode + phoneNumber;
            }

            return phoneNumber;
        }

        public async Task SendWhatsAppMessageAsync(string phoneNumber, string message, string method, string referenceId,string langCode)
        {
            WhatsAppLog log = new WhatsAppLog();
            log.Method = method;
            log.ReferenceId = referenceId;
            try
            {
                phoneNumber = validatePhoneNumber(phoneNumber);                log.MobileNumber = "00" + phoneNumber;
                log.WhatsAppMessage = message;
                MsgContent msg = new MsgContent();
                To toNumber = new To();
                RichContent richContent = new RichContent();
                richContent.conversation[0] = new Conversation();
                WhatsAppDto whatsApp = new WhatsAppDto();
                richContent.conversation[0].Template = new Template();
                richContent.conversation[0].Template.Whatsapp = new Whatsapp();
                richContent.conversation[0].Template.Whatsapp.Components = new Components[1];
                richContent.conversation[0].Template.Whatsapp.Components[0] = new Components();
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters = new Parameters[1];
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0] = new Parameters();
                //assign values to properties
                msg.AllowedChannels = new string[1] { "WhatsApp" };
                msg.body.Type = "Auto";
                msg.From = "00" + validatePhoneNumber(RepositoryConstants.WhatsAppSender);
                msg.body.Content = message;
                richContent.conversation[0].Template.Whatsapp.Namespace = RepositoryConstants.WhatsAppNamespace;
                richContent.conversation[0].Template.Whatsapp.ElementName = RepositoryConstants.WhatsAppElementName;
                richContent.conversation[0].Template.Whatsapp.Language.Code = langCode;
                richContent.conversation[0].Template.Whatsapp.Language.Policy = "deterministic";
                richContent.conversation[0].Template.Whatsapp.Components[0].Type = "header";
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Type = "document";
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Media = new Media();
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Media.MediaName = "Policy";
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Media.MediaUri = "https://bcare.com.sa/Identityapi/api/u/p?r=" + referenceId;
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Media.MimeType = "application/pdf";
                toNumber.Number = log.MobileNumber;
                msg.to[0] = toNumber;
                msg.richContent = richContent;
                whatsApp.message.authentication.ProductToken = RepositoryConstants.WhatsAppProductToken;
                whatsApp.message.Msg = new MsgContent[] { msg };

                log.ServiceRequest = JsonConvert.SerializeObject(whatsApp);
                DateTime dtBeforeCalling = DateTime.Now;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var response = _httpClient.PostAsync(RepositoryConstants.WhatsAppServiceUrl, whatsApp).Result;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null || response.Content == null)                {                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ResponseIsNull;                    log.ErrorDescription = "response from WhatsApp is null";                }                else                {                    var responseString = await response.Content?.ReadAsStringAsync();                    log.ServiceResponse = responseString;
                    WhatsAppResponse stcResponse = JsonConvert.DeserializeObject<WhatsAppResponse>(responseString);
                    if (stcResponse.message.Length != 0)
                    {
                        switch (stcResponse.message[0].MessageErrorCode)
                        {
                            case "0":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.Success;
                                    break;
                                }
                            case "999":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.UnknownError;
                                    break;
                                }
                            case "101":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.AuthenticationFailed;
                                    break;
                                }
                            case "102":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheAccountUsingThisAuthenticationHasInsufficientBalance;
                                    break;
                                }
                            case "103":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheProductTokenIsIncorrect;
                                    break;
                                }
                            case "201":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheRequestMSGArrayIsIncorrect;
                                    break;
                                }
                            case "202":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisRequestIsMalformed;
                                    break;
                                }
                            case "203":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheRequestMSGArrayIsIncorrect;
                                    break;
                                }
                            case "301":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidFromField;
                                    break;
                                }
                            case "302":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidToField;
                                    break;
                                }
                            case "303":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidPhoneNumberInTheToField;
                                    break;
                                }
                            case "304":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidBodyField;
                                    break;
                                }
                            case "305":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidField;
                                    break;
                                }
                            case "401":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.MessageHasBeenSpamFiltered;
                                    break;
                                }
                            case "402":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.MessageHasBeenBlacklisted;
                                    break;
                                }
                            case "403":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.MessageHasBeenRejected;
                                    break;
                                }
                            case "500":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.AnInternalErrorHasOccurred;
                                    break;
                                }
                            default:
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                                    break;
                                }
                        }
                        //log.ErrorCode = (int)WhatsAppResponseErrorCodes.FailedToDeserialize;
                        log.ErrorDescription = string.IsNullOrEmpty(stcResponse.message[0].MessageDetails) ? stcResponse.Details : stcResponse.message[0].MessageDetails;

                    }
                    else
                    {
                        log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                        log.ErrorDescription = stcResponse.Details;
                    }                }                WhatsAppLogsDataAccess.AddToWhatsAppLogsDataAccess(log);            }
            catch (Exception exp)
            {
                log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                log.ErrorDescription = exp.ToString();
                WhatsAppLogsDataAccess.AddToWhatsAppLogsDataAccess(log);
            }
        }
        public async Task SendWhatsAppMessageForPolicyRenewalAsync(string phoneNumber, string message,string make,string model,string plateText,string url, string method, string referenceId, string langCode, string expiryDate)
        {
            WhatsAppLog log = new WhatsAppLog();
            log.Method = method;
            try
            {
                phoneNumber = validatePhoneNumber(phoneNumber);                log.MobileNumber = "00" + phoneNumber;
                log.WhatsAppMessage = message;
                log.ReferenceId = referenceId;
                MsgContent msg = new MsgContent();
                To toNumber = new To();
                RichContent richContent = new RichContent();
                richContent.conversation[0] = new Conversation();
                WhatsAppDto whatsApp = new WhatsAppDto();
                richContent.conversation[0].Template = new Template();
                richContent.conversation[0].Template.Whatsapp = new Whatsapp();
                //assign values to properties
                msg.AllowedChannels = new string[1] { "WhatsApp" };
                msg.body.Type = "Auto";
                msg.From = "00" + validatePhoneNumber(RepositoryConstants.WhatsAppSender);
                msg.body.Content = message;
                richContent.conversation[0].Template.Whatsapp.Namespace = RepositoryConstants.WhatsAppNamespace;
                richContent.conversation[0].Template.Whatsapp.ElementName = RepositoryConstants.WhatsAppElementNameForPolicyRenewal;
                richContent.conversation[0].Template.Whatsapp.Language.Code = langCode;
                richContent.conversation[0].Template.Whatsapp.Language.Policy = "deterministic";
                richContent.conversation[0].Template.Whatsapp.Components = new Components[1];
                richContent.conversation[0].Template.Whatsapp.Components[0] = new Components();
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters = new Parameters[5];
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0] = new Parameters();
                richContent.conversation[0].Template.Whatsapp.Components[0].Type = "body";

                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0] = new Parameters();
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Type = "text";
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Text = make;

                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[1] = new Parameters();
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[1].Type = "text";
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[1].Text = model;

                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[2] = new Parameters();
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[2].Type = "text";
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[2].Text = plateText;

                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[3] = new Parameters();
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[3].Type = "text";
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[3].Text = expiryDate;

                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[4] = new Parameters();
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[4].Type = "text";
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[4].Text = url;

                toNumber.Number = log.MobileNumber;
                msg.to[0] = toNumber;
                msg.richContent = richContent;
                whatsApp.message.authentication.ProductToken = RepositoryConstants.WhatsAppProductToken;
                whatsApp.message.Msg = new MsgContent[] { msg };

                log.ServiceRequest = JsonConvert.SerializeObject(whatsApp);
                DateTime dtBeforeCalling = DateTime.Now;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var response = _httpClient.PostAsync(RepositoryConstants.WhatsAppServiceUrl, whatsApp).Result;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null || response.Content == null)                {                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ResponseIsNull;                    log.ErrorDescription = "response from WhatsApp is null";                }                else                {                    var responseString = await response.Content?.ReadAsStringAsync();                    log.ServiceResponse = responseString;
                    WhatsAppResponse stcResponse = JsonConvert.DeserializeObject<WhatsAppResponse>(responseString);
                    if (stcResponse.message.Length != 0)
                    {
                        switch (stcResponse.message[0].MessageErrorCode)
                        {
                            case "0":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.Success;
                                    break;
                                }
                            case "999":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.UnknownError;
                                    break;
                                }
                            case "101":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.AuthenticationFailed;
                                    break;
                                }
                            case "102":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheAccountUsingThisAuthenticationHasInsufficientBalance;
                                    break;
                                }
                            case "103":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheProductTokenIsIncorrect;
                                    break;
                                }
                            case "201":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheRequestMSGArrayIsIncorrect;
                                    break;
                                }
                            case "202":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisRequestIsMalformed;
                                    break;
                                }
                            case "203":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheRequestMSGArrayIsIncorrect;
                                    break;
                                }
                            case "301":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidFromField;
                                    break;
                                }
                            case "302":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidToField;
                                    break;
                                }
                            case "303":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidPhoneNumberInTheToField;
                                    break;
                                }
                            case "304":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidBodyField;
                                    break;
                                }
                            case "305":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidField;
                                    break;
                                }
                            case "401":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.MessageHasBeenSpamFiltered;
                                    break;
                                }
                            case "402":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.MessageHasBeenBlacklisted;
                                    break;
                                }
                            case "403":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.MessageHasBeenRejected;
                                    break;
                                }
                            case "500":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.AnInternalErrorHasOccurred;
                                    break;
                                }
                            default:
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                                    break;
                                }
                        }
                        //log.ErrorCode = (int)WhatsAppResponseErrorCodes.FailedToDeserialize;
                        log.ErrorDescription = string.IsNullOrEmpty(stcResponse.message[0].MessageDetails) ? stcResponse.Details : stcResponse.message[0].MessageDetails;

                    }
                    else
                    {
                        log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                        log.ErrorDescription = stcResponse.Details;
                    }                }                WhatsAppLogsDataAccess.AddToWhatsAppLogsDataAccess(log);            }
            catch (Exception exp)
            {
                log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                log.ErrorDescription = exp.ToString();
                WhatsAppLogsDataAccess.AddToWhatsAppLogsDataAccess(log);
            }
        }

        public bool SendSmsMobiShastra(string phoneNumber, string message, string method, out string exception)
        {
            exception = string.Empty;            SMSLog log = new SMSLog();            log.Method = method;            try            {                phoneNumber = validatePhoneNumber(phoneNumber);                log.MobileNumber = phoneNumber;                log.SMSMessage = message;
                string jsonRequest = new JavaScriptSerializer().Serialize(new[]{new
                {
                    user = RepositoryConstants.MobiShastraSmsProfileId,
                    pwd = RepositoryConstants.MobiShastraSmsPassword,
                    number = phoneNumber,
                    msg = message,
                    sender = RepositoryConstants.MobiShastraSmsSender,
                    countrycode = "ALL"
                } });
                var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                var response = _httpClient.Post(RepositoryConstants.MobiShastraSmsServiceUrl, content);
                if (response == null || response.Content == null)                {                    log.ErrorCode = 13;                    log.ErrorDescription = "response from MobiShastraSmsProvider is null";
                    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                    return false;                }                else                {                    var responseString = response.Content?.ReadAsStringAsync().Result;                    List<MobiShastraSmsResponse> mobiShastraResponseList = JsonConvert.DeserializeObject<List<MobiShastraSmsResponse>>(responseString);
                    if (mobiShastraResponseList == null || !mobiShastraResponseList.Any())
                    {
                        log.ErrorCode = 99; // Empty Response List
                        log.ErrorDescription = $"MobiShastra-Empty Response List";                        SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
                        return false;
                    }
                    var mobiShastraResponse = mobiShastraResponseList.FirstOrDefault();
                    if (mobiShastraResponse  == null)
                    {
                        log.ErrorCode = 99; // Empty Response
                        log.ErrorDescription = $"MobiShastra-Empty Response";                        SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
                        return false;
                    }
                    if (mobiShastraResponse.ResponseString != "Send Successful")
                    {
                        log.ErrorCode = 99; // Faild to Send
                        log.ErrorDescription = $"MobiShastra-{mobiShastraResponse.ResponseString}";                        SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
                        return false;                    }

                    log.ErrorCode = (int)ESTCSmsResponse.Success;
                    log.ErrorDescription = "MobiShastra-Success";
                    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                    return true;                }
            }            catch (Exception exp)            {                exception = exp.ToString();                log.ErrorCode = 12;                log.ErrorDescription = $"MobiShastra-{exp.ToString()}";                SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                return false;            }
        }
        public async Task SendSmsMobiShastraAsync(string phoneNumber, string message, string method)
        {
            SMSLog log = new SMSLog();            log.Method = method;            try            {                phoneNumber = validatePhoneNumber(phoneNumber);                log.MobileNumber = phoneNumber;                log.SMSMessage = message;
                string jsonRequest = new JavaScriptSerializer().Serialize(new[]{new
                {
                    user = RepositoryConstants.MobiShastraSmsProfileId,
                    pwd = RepositoryConstants.MobiShastraSmsPassword,
                    number = phoneNumber,
                    msg = message,
                    sender = RepositoryConstants.MobiShastraSmsSender,
                    countrycode = "ALL"
                } });
                var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                var response = _httpClient.Post(RepositoryConstants.MobiShastraSmsServiceUrl, content);
                if (response == null || response.Content == null)                {                    log.ErrorCode = 13;                    log.ErrorDescription = "response from MobiShastraSmsProvider is null";
                    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                    return;                }                else                {                    var responseString = response.Content?.ReadAsStringAsync().Result;                    List<MobiShastraSmsResponse> mobiShastraResponseList = JsonConvert.DeserializeObject<List<MobiShastraSmsResponse>>(responseString);
                    if (mobiShastraResponseList == null || !mobiShastraResponseList.Any())
                    {
                        log.ErrorCode = 99; // Empty Response List
                        log.ErrorDescription = $"MobiShastra-Empty Response List";                        SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
                        return;
                    }
                    var mobiShastraResponse = mobiShastraResponseList.FirstOrDefault();
                    if (mobiShastraResponse == null)
                    {
                        log.ErrorCode = 99; // Empty Response
                        log.ErrorDescription = $"MobiShastra-Empty Response";                        SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
                        return;
                    }
                    if (mobiShastraResponse.ResponseString != "Send Successful")
                    {
                        log.ErrorCode = 99; // Faild to Send
                        log.ErrorDescription = $"MobiShastra-{mobiShastraResponse.ResponseString}";                        SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
                        return;                    }

                    log.ErrorCode = (int)ESTCSmsResponse.Success;
                    log.ErrorDescription = "MobiShastra-Success";
                    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                    return;                }
            }            catch (Exception exp)            {                log.ErrorCode = 12;                log.ErrorDescription = $"MobiShastra-{exp.ToString()}";                SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                return;            }
        }
        public bool SendWhatsAppMessageForShareQuoteAsync(string phoneNumber, string url,string externalId,string lang, out string exception)
        {
            WhatsAppLog log = new WhatsAppLog();
            log.Method = "ShareQuote";
            exception = string.Empty;
            try
            {
                phoneNumber = validatePhoneNumber(phoneNumber);                log.MobileNumber = "00" + phoneNumber;
                log.WhatsAppMessage = url;
                log.ReferenceId = externalId;
                MsgContent msg = new MsgContent();
                To toNumber = new To();
                RichContent richContent = new RichContent();
                richContent.conversation[0] = new Conversation();
                WhatsAppDto whatsApp = new WhatsAppDto();
                richContent.conversation[0].Template = new Template();
                richContent.conversation[0].Template.Whatsapp = new Whatsapp();

                //assign values to properties
                msg.AllowedChannels = new string[1] { "WhatsApp" };
                msg.body.Type = "Auto";
                msg.From = "00" + validatePhoneNumber(RepositoryConstants.WhatsAppSender);
                msg.body.Content = lang.ToLower() == "en"? "Your quote is ready from Bcare": "تسعيرتك جاهزة من بي كير";
                richContent.conversation[0].Template.Whatsapp.Namespace = RepositoryConstants.WhatsAppNamespace;
                richContent.conversation[0].Template.Whatsapp.ElementName = RepositoryConstants.WhatsAppElementNameForShareQuote;
                richContent.conversation[0].Template.Whatsapp.Language.Code =lang;
                richContent.conversation[0].Template.Whatsapp.Language.Policy = "deterministic";
                richContent.conversation[0].Template.Whatsapp.Components = new Components[1];
                richContent.conversation[0].Template.Whatsapp.Components[0] = new Components();
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters = new Parameters[1];
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0] = new Parameters();
                richContent.conversation[0].Template.Whatsapp.Components[0].Type = "body";

                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0] = new Parameters();
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Type = "text";
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Text = url;

                toNumber.Number = log.MobileNumber;
                msg.to[0] = toNumber;
                msg.richContent = richContent;
                whatsApp.message.authentication.ProductToken = RepositoryConstants.WhatsAppProductToken;
                whatsApp.message.Msg = new MsgContent[] { msg };

                log.ServiceRequest = JsonConvert.SerializeObject(whatsApp);
                DateTime dtBeforeCalling = DateTime.Now;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var response = _httpClient.Post(RepositoryConstants.WhatsAppServiceUrl, whatsApp);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null || response.Content == null)                {                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ResponseIsNull;                    log.ErrorDescription = "response from WhatsApp is null";                    exception = log.ErrorDescription;                }                else                {                    var responseString = response.Content?.ReadAsStringAsync().Result;                    log.ServiceResponse = responseString;
                    WhatsAppResponse stcResponse = JsonConvert.DeserializeObject<WhatsAppResponse>(responseString);
                    if (stcResponse.message.Length != 0)
                    {
                        switch (stcResponse.message[0].MessageErrorCode)
                        {
                            case "0":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.Success;
                                    break;
                                }
                            case "999":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.UnknownError;
                                    break;
                                }
                            case "101":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.AuthenticationFailed;
                                    break;
                                }
                            case "102":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheAccountUsingThisAuthenticationHasInsufficientBalance;
                                    break;
                                }
                            case "103":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheProductTokenIsIncorrect;
                                    break;
                                }
                            case "201":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheRequestMSGArrayIsIncorrect;
                                    break;
                                }
                            case "202":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisRequestIsMalformed;
                                    break;
                                }
                            case "203":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheRequestMSGArrayIsIncorrect;
                                    break;
                                }
                            case "301":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidFromField;
                                    break;
                                }
                            case "302":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidToField;
                                    break;
                                }
                            case "303":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidPhoneNumberInTheToField;
                                    break;
                                }
                            case "304":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidBodyField;
                                    break;
                                }
                            case "305":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidField;
                                    break;
                                }
                            case "401":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.MessageHasBeenSpamFiltered;
                                    break;
                                }
                            case "402":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.MessageHasBeenBlacklisted;
                                    break;
                                }
                            case "403":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.MessageHasBeenRejected;
                                    break;
                                }
                            case "500":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.AnInternalErrorHasOccurred;
                                    break;
                                }
                            default:
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                                    break;
                                }
                        }
                        log.ErrorDescription = string.IsNullOrEmpty(stcResponse.message[0].MessageDetails) ? stcResponse.Details : stcResponse.message[0].MessageDetails;

                    }
                    else
                    {
                        log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                        log.ErrorDescription = stcResponse.Details;
                    }                }                WhatsAppLogsDataAccess.AddToWhatsAppLogsDataAccess(log);                if (log.ErrorCode == (int)WhatsAppResponseErrorCodes.Success)                    return true;                else
                {
                    exception = log.ErrorDescription;
                    return false;
                }            }
            catch (Exception exp)
            {
                log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                log.ErrorDescription = exp.ToString();
                exception = log.ErrorDescription;
                WhatsAppLogsDataAccess.AddToWhatsAppLogsDataAccess(log);
                return false;
            }
        }

        public SMSOutput SendSmsByMobiShastra(SMSModel model)
        {
            SMSLog log = new SMSLog();            SMSOutput output = new SMSOutput();            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();            log.SMSProvider = "MobiShastra";            log.ServiceURL = RepositoryConstants.MobiShastraSmsServiceUrl;            log.Method = model.Method;            log.Module = model.Module;            log.Channel = model.Channel;            log.ReferenceId = model.ReferenceId;            try            {                log.MobileNumber = model.PhoneNumber;                log.SMSMessage = model.MessageBody;
                string jsonRequest = new JavaScriptSerializer().Serialize(new[]{new
                {
                    user = RepositoryConstants.MobiShastraSmsProfileId,
                    pwd = RepositoryConstants.MobiShastraSmsPassword,
                    number = model.PhoneNumber,
                    msg = model.MessageBody,
                    sender = RepositoryConstants.MobiShastraSmsSender,
                    countrycode = "ALL"
                } });
                log.ServiceRequest = jsonRequest;
                var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                DateTime dtBeforeCalling = DateTime.Now;
                var response = _httpClient.Post(RepositoryConstants.MobiShastraSmsServiceUrl, content);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;

                if (response == null || response.Content == null)                {                    log.ErrorCode = 13;                    log.ErrorDescription = "response from MobiShastraSmsProvider is null";
                    output.ErrorCode = log.ErrorCode.Value;
                    output.ErrorDescription = log.ErrorDescription;
                    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                    return output;                }                else                {                    var responseString = response.Content?.ReadAsStringAsync().Result;                    log.ServiceResponse = responseString;                    List<MobiShastraSmsResponse> mobiShastraResponseList = JsonConvert.DeserializeObject<List<MobiShastraSmsResponse>>(responseString);
                    if (mobiShastraResponseList == null || !mobiShastraResponseList.Any())
                    {
                        log.ErrorCode = 99; // Empty Response List
                        log.ErrorDescription = $"MobiShastra-Empty Response List";                        output.ErrorCode = log.ErrorCode.Value;
                        output.ErrorDescription = log.ErrorDescription;                        SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
                        return output;
                    }
                    var mobiShastraResponse = mobiShastraResponseList.FirstOrDefault();
                    if (mobiShastraResponse == null)
                    {
                        log.ErrorCode = 99; // Empty Response
                        log.ErrorDescription = $"MobiShastra-Empty Response";                        output.ErrorCode = log.ErrorCode.Value;
                        output.ErrorDescription = log.ErrorDescription;                        SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
                        return output;
                    }
                    if (mobiShastraResponse.ResponseString != "Send Successful")
                    {
                        log.ErrorCode = 99; // Faild to Send
                        log.ErrorDescription = $"MobiShastra-{mobiShastraResponse.ResponseString}";                        output.ErrorCode = log.ErrorCode.Value;
                        output.ErrorDescription = log.ErrorDescription;                        SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
                        return output;                    }

                    log.ErrorCode = (int)ESTCSmsResponse.Success;
                    log.ErrorDescription = "Success";
                    output.ErrorCode = log.ErrorCode.Value;
                    output.ErrorDescription = log.ErrorDescription;
                    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                    return output;                }
            }            catch (Exception exp)            {                log.ErrorCode = 12;                log.ErrorDescription = exp.ToString();                output.ErrorCode = log.ErrorCode.Value;
                output.ErrorDescription = log.ErrorDescription;                SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                return output;            }
        }
        public SMSOutput SendSmsBySTC(SMSModel model)        {            SMSLog log = new SMSLog();            SMSOutput output = new SMSOutput();            log.UserIP = Utilities.GetUserIPAddress();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();            log.SMSProvider = "STC";            log.ServiceURL = RepositoryConstants.STCSmsServiceUrl;            log.Method = model.Method;            log.Module = model.Module;            log.Channel = model.Channel;            log.ReferenceId = model.ReferenceId;            try            {                log.MobileNumber = model.PhoneNumber;                log.SMSMessage = model.MessageBody;
                string request = "{  \"userName\": \"" + RepositoryConstants.STCSmsAccountUsername + "\",  \"numbers\": \"" + model.PhoneNumber + "\",  \"userSender\": \"" + RepositoryConstants.STCSmsAccountSender + "\",  \"apiKey\": \"" + RepositoryConstants.STCSmsApiKey + "\",  \"msg\": \"" + model.MessageBody + "\"}";
                var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                log.ServiceRequest = request;
                DateTime dtBeforeCalling = DateTime.Now;
                var response = _httpClient.Post(RepositoryConstants.STCSmsServiceUrl, content);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null || response.Content == null)                {                    log.ErrorCode = 13;                    log.ErrorDescription = "response from STCSmsProvider is null";                }                else                {                    var responseString = response.Content?.ReadAsStringAsync().Result;                    log.ServiceResponse = responseString;                    STCSmsResponse stcResponse = JsonConvert.DeserializeObject<STCSmsResponse>(responseString);
                    switch (stcResponse.Code)                    {                        case "1":                        case "M0000": //Success
                            {                                log.ErrorCode = (int)ESTCSmsResponse.Success;                                break;                            }                        case "M0001":                            {                                log.ErrorCode = (int)ESTCSmsResponse.VariablesMissing;                                break;                            }                        case "1010"://Variables missing
                            {                                log.ErrorCode = (int)ESTCSmsResponse.VariablesMissing;                                break;                            }                        case "M0002":                        case "1020"://Invalid login info
                            {                                log.ErrorCode = (int)ESTCSmsResponse.InvalidLoginInfo;                                break;                            }                        case "M0022": //Exceed number of senders allowed
                            {                                log.ErrorCode = (int)ESTCSmsResponse.ExceedNumberOfSendersAllowed;                                break;                            }                        case "M0023": //Exceed number of senders allowed
                            {                                log.ErrorCode = (int)ESTCSmsResponse.SenderNameIsActiveOrUnderActivationOrRefused;                                break;                            }                        case "M0024": //Sender Name should be in English or number
                            {                                log.ErrorCode = (int)ESTCSmsResponse.SenderNameShouldBeInEnglishOrNumber;                                break;                            }                        case "M0025": //Invalid Sender Name Length
                            {                                log.ErrorCode = (int)ESTCSmsResponse.InvalidSenderNameLength;                                break;                            }                        case "M0026": //Sender Name is already activated or not found
                            {                                log.ErrorCode = (int)ESTCSmsResponse.SenderNameIsAlreadyActivatedOrNotFound;                                break;                            }                        case "M0027": //Activation Code is not Correct
                            {                                log.ErrorCode = (int)ESTCSmsResponse.ActivationCodeIsNotCorrect;                                break;                            }                        case "M0028": //You reach maximum number of attempts. Sender name is locked
                            {                                log.ErrorCode = (int)ESTCSmsResponse.YouReachMaximumNumberOfAttemptsSenderNameIsLocked;                                break;                            }                        case "M0050": //MSG body is empty
                            {                                log.ErrorCode = (int)ESTCSmsResponse.MsgBodyIsEmpty;                                break;                            }                        case "M0060": //Balance is not enough
                            {                                log.ErrorCode = (int)ESTCSmsResponse.BalanceIsNotEnough;                                break;                            }                        case "M0061": //MSG duplicated
                            {                                log.ErrorCode = (int)ESTCSmsResponse.MsgDuplicated;                                break;                            }                        case "1110": //Sender name is missing or incorrect
                            {                                log.ErrorCode = (int)ESTCSmsResponse.SenderNameIsMissingOrIncorrect;                                break;                            }                        case "1120": //Mobile numbers is not correct
                            {                                log.ErrorCode = (int)ESTCSmsResponse.MobileNumbersIsNotCorrect;                                break;                            }                        case "1140": //MSG length is too long
                            {                                log.ErrorCode = (int)ESTCSmsResponse.MsgLengthIsTooLong;                                break;                            }                        case "M0029": //Invalid Sender Name
                            {                                log.ErrorCode = (int)ESTCSmsResponse.InvalidSenderName;                                break;                            }                        case "M0030": //Sender Name should ended with AD
                            {                                log.ErrorCode = (int)ESTCSmsResponse.SenderNameShouldEndedWithAD;                                break;                            }                        case "M0031": //Maximum allowed size of uploaded file is 5 MB
                            {                                log.ErrorCode = (int)ESTCSmsResponse.MaximumAllowedSizeOfUploadedFileIs5MB;                                break;                            }                        case "M0032": //Only pdf,png,jpg and jpeg files are allowed!
                            {                                log.ErrorCode = (int)ESTCSmsResponse.FileExtensionNotAllowed;                                break;                            }                        case "M0033": //Sender Type should be normal or whitelist only
                            {                                log.ErrorCode = (int)ESTCSmsResponse.SenderTypeShouldBeNormalOrWhitelistOnly;                                break;                            }                        case "M0034": //Please Use POST Method
                            {                                log.ErrorCode = (int)ESTCSmsResponse.PleaseUsePOSTMethod;                                break;                            }                        default:                            {                                log.ErrorCode = (int)ESTCSmsResponse.GenericError;                                break;                            }                    }                    log.ErrorDescription = stcResponse.Message;                }
                output.ErrorCode = log.ErrorCode.Value;
                output.ErrorDescription = log.ErrorDescription;                SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                return output;            }            catch (Exception exp)            {                log.ErrorCode = 12;                log.ErrorDescription = exp.ToString();                output.ErrorCode = log.ErrorCode.Value;
                output.ErrorDescription = log.ErrorDescription;                SMSLogsDataAccess.AddToSMSLogsDataAccess(log);                return output;            }        }
        public async Task SendWhatsAppMessageUpdateCustomCardAsync(string phoneNumber, string message, string method, string referenceId, string langCode,string make,string model,string plateText)
        {
            WhatsAppLog log = new WhatsAppLog();
            log.Method = method;
            try
            {
                phoneNumber = validatePhoneNumber(phoneNumber);                log.MobileNumber = "00" + phoneNumber;
                log.WhatsAppMessage = message;
                MsgContent msg = new MsgContent();
                To toNumber = new To();
                RichContent richContent = new RichContent();
                richContent.conversation[0] = new Conversation();
                WhatsAppDto whatsApp = new WhatsAppDto();
                richContent.conversation[0].Template = new Template();
                richContent.conversation[0].Template.Whatsapp = new Whatsapp();
                richContent.conversation[0].Template.Whatsapp.Components = new Components[2];
                richContent.conversation[0].Template.Whatsapp.Components[0] = new Components();
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters = new Parameters[1];
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0] = new Parameters();
                //assign values to properties
                msg.AllowedChannels = new string[1] { "WhatsApp" };
                msg.body.Type = "Auto";
                msg.From = "00" + validatePhoneNumber(RepositoryConstants.WhatsAppSender);
                msg.body.Content = message;
                richContent.conversation[0].Template.Whatsapp.Namespace = RepositoryConstants.WhatsAppNamespace;
                richContent.conversation[0].Template.Whatsapp.ElementName = RepositoryConstants.WhatsAppElementNameUpdateCustomToSequance;
                richContent.conversation[0].Template.Whatsapp.Language.Code = langCode;
                richContent.conversation[0].Template.Whatsapp.Language.Policy = "deterministic";
                richContent.conversation[0].Template.Whatsapp.Components[0].Type = "header";
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Type = "document";
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Media = new Media();
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Media.MediaName = "Policy";
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Media.MediaUri = "https://bcare.com.sa/Identityapi/api/u/p?r=" + referenceId;
                richContent.conversation[0].Template.Whatsapp.Components[0].Parameters[0].Media.MimeType = "application/pdf";

                richContent.conversation[0].Template.Whatsapp.Components[1] = new Components();
                richContent.conversation[0].Template.Whatsapp.Components[1].Parameters = new Parameters[3];
                richContent.conversation[0].Template.Whatsapp.Components[1].Type = "body";

                richContent.conversation[0].Template.Whatsapp.Components[1].Parameters[0] = new Parameters();
                richContent.conversation[0].Template.Whatsapp.Components[1].Parameters[0].Type = "text";
                richContent.conversation[0].Template.Whatsapp.Components[1].Parameters[0].Text = make;

                richContent.conversation[0].Template.Whatsapp.Components[1].Parameters[1] = new Parameters();
                richContent.conversation[0].Template.Whatsapp.Components[1].Parameters[1].Type = "text";
                richContent.conversation[0].Template.Whatsapp.Components[1].Parameters[1].Text = model;

                richContent.conversation[0].Template.Whatsapp.Components[1].Parameters[2] = new Parameters();
                richContent.conversation[0].Template.Whatsapp.Components[1].Parameters[2].Type = "text";
                richContent.conversation[0].Template.Whatsapp.Components[1].Parameters[2].Text = plateText;


                toNumber.Number = log.MobileNumber;
                msg.to[0] = toNumber;
                msg.richContent = richContent;
                whatsApp.message.authentication.ProductToken = RepositoryConstants.WhatsAppProductToken;
                whatsApp.message.Msg = new MsgContent[] { msg };

                log.ServiceRequest = JsonConvert.SerializeObject(whatsApp);
                DateTime dtBeforeCalling = DateTime.Now;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var response = _httpClient.PostAsync(RepositoryConstants.WhatsAppServiceUrl, whatsApp).Result;
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null || response.Content == null)                {                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ResponseIsNull;                    log.ErrorDescription = "response from WhatsApp is null";                }                else                {                    var responseString = await response.Content?.ReadAsStringAsync();                    log.ServiceResponse = responseString;
                    WhatsAppResponse stcResponse = JsonConvert.DeserializeObject<WhatsAppResponse>(responseString);
                    if (stcResponse.message.Length != 0)
                    {
                        switch (stcResponse.message[0].MessageErrorCode)
                        {
                            case "0":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.Success;
                                    break;
                                }
                            case "999":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.UnknownError;
                                    break;
                                }
                            case "101":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.AuthenticationFailed;
                                    break;
                                }
                            case "102":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheAccountUsingThisAuthenticationHasInsufficientBalance;
                                    break;
                                }
                            case "103":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheProductTokenIsIncorrect;
                                    break;
                                }
                            case "201":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheRequestMSGArrayIsIncorrect;
                                    break;
                                }
                            case "202":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisRequestIsMalformed;
                                    break;
                                }
                            case "203":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.TheRequestMSGArrayIsIncorrect;
                                    break;
                                }
                            case "301":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidFromField;
                                    break;
                                }
                            case "302":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidToField;
                                    break;
                                }
                            case "303":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidPhoneNumberInTheToField;
                                    break;
                                }
                            case "304":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidBodyField;
                                    break;
                                }
                            case "305":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.ThisMSGHasAnInvalidField;
                                    break;
                                }
                            case "401":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.MessageHasBeenSpamFiltered;
                                    break;
                                }
                            case "402":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.MessageHasBeenBlacklisted;
                                    break;
                                }
                            case "403":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.MessageHasBeenRejected;
                                    break;
                                }
                            case "500":
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.AnInternalErrorHasOccurred;
                                    break;
                                }
                            default:
                                {
                                    log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                                    break;
                                }
                        }
                        //log.ErrorCode = (int)WhatsAppResponseErrorCodes.FailedToDeserialize;
                        log.ErrorDescription = string.IsNullOrEmpty(stcResponse.message[0].MessageDetails) ? stcResponse.Details : stcResponse.message[0].MessageDetails;

                    }
                    else
                    {
                        log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                        log.ErrorDescription = stcResponse.Details;
                    }                }                WhatsAppLogsDataAccess.AddToWhatsAppLogsDataAccess(log);            }
            catch (Exception exp)
            {
                log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                log.ErrorDescription = exp.ToString();
                WhatsAppLogsDataAccess.AddToWhatsAppLogsDataAccess(log);
            }
        }
    }
}
