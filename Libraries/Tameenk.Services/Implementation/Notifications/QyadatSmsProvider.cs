﻿using Newtonsoft.Json;
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
            //    phoneNumber = validatePhoneNumber(phoneNumber);
            //    var content = new StringContent("{  \"userName\": \"" + RepositoryConstants.STCSmsAccountUsername + "\",  \"numbers\": \"" + phoneNumber + "\",  \"userSender\": \"" + RepositoryConstants.STCSmsAccountSender + "\",  \"apiKey\": \"" + RepositoryConstants.STCSmsApiKey + "\",  \"msg\": \"" + message + "\"}", System.Text.Encoding.UTF8, "application/json");
            //    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            //    var response = await _httpClient.PostAsync(RepositoryConstants.STCSmsServiceUrl, content);
            //    if (response == null || response.Content == null)
            //        switch (stcResponse.Code)
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {
            //                {

            //catch (Exception exp)
            //{
            //    log.ErrorCode = 12;
            //    log.ErrorDescription = exp.ToString();
            //    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
            //}
        }

        

        public bool SendSmsSTC(string phoneNumber, string message, string method, out string exception)
                var content = new StringContent("{  \"userName\": \"" + RepositoryConstants.STCSmsAccountUsername + "\",  \"numbers\": \"" + phoneNumber + "\",  \"userSender\": \"" + RepositoryConstants.STCSmsAccountSender + "\",  \"apiKey\": \"" + RepositoryConstants.STCSmsApiKey + "\",  \"msg\": \"" + message + "\"}", System.Text.Encoding.UTF8, "application/json");
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                var response = _httpClient.Post(RepositoryConstants.STCSmsServiceUrl, content);
                if (response == null || response.Content == null)
                    switch (stcResponse.Code)
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {

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
                phoneNumber = validatePhoneNumber(phoneNumber);
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
                if (response == null || response.Content == null)
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
                    }
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
                phoneNumber = validatePhoneNumber(phoneNumber);
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
                if (response == null || response.Content == null)
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
                    }
            catch (Exception exp)
            {
                log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                log.ErrorDescription = exp.ToString();
                WhatsAppLogsDataAccess.AddToWhatsAppLogsDataAccess(log);
            }
        }

        public bool SendSmsMobiShastra(string phoneNumber, string message, string method, out string exception)
        {
            exception = string.Empty;
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
                if (response == null || response.Content == null)
                    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
                    if (mobiShastraResponseList == null || !mobiShastraResponseList.Any())
                    {
                        log.ErrorCode = 99; // Empty Response List
                        log.ErrorDescription = $"MobiShastra-Empty Response List";
                        return false;
                    }
                    var mobiShastraResponse = mobiShastraResponseList.FirstOrDefault();
                    if (mobiShastraResponse  == null)
                    {
                        log.ErrorCode = 99; // Empty Response
                        log.ErrorDescription = $"MobiShastra-Empty Response";
                        return false;
                    }
                    if (mobiShastraResponse.ResponseString != "Send Successful")
                    {
                        log.ErrorCode = 99; // Faild to Send
                        log.ErrorDescription = $"MobiShastra-{mobiShastraResponse.ResponseString}";
                        return false;

                    log.ErrorCode = (int)ESTCSmsResponse.Success;
                    log.ErrorDescription = "MobiShastra-Success";
                    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
            }
        }
        public async Task SendSmsMobiShastraAsync(string phoneNumber, string message, string method)
        {
            SMSLog log = new SMSLog();
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
                if (response == null || response.Content == null)
                    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
                    if (mobiShastraResponseList == null || !mobiShastraResponseList.Any())
                    {
                        log.ErrorCode = 99; // Empty Response List
                        log.ErrorDescription = $"MobiShastra-Empty Response List";
                        return;
                    }
                    var mobiShastraResponse = mobiShastraResponseList.FirstOrDefault();
                    if (mobiShastraResponse == null)
                    {
                        log.ErrorCode = 99; // Empty Response
                        log.ErrorDescription = $"MobiShastra-Empty Response";
                        return;
                    }
                    if (mobiShastraResponse.ResponseString != "Send Successful")
                    {
                        log.ErrorCode = 99; // Faild to Send
                        log.ErrorDescription = $"MobiShastra-{mobiShastraResponse.ResponseString}";
                        return;

                    log.ErrorCode = (int)ESTCSmsResponse.Success;
                    log.ErrorDescription = "MobiShastra-Success";
                    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
            }
        }
        public bool SendWhatsAppMessageForShareQuoteAsync(string phoneNumber, string url,string externalId,string lang, out string exception)
        {
            WhatsAppLog log = new WhatsAppLog();
            log.Method = "ShareQuote";
            exception = string.Empty;
            try
            {
                phoneNumber = validatePhoneNumber(phoneNumber);
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
                if (response == null || response.Content == null)
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
                    }
                {
                    exception = log.ErrorDescription;
                    return false;
                }
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
            SMSLog log = new SMSLog();
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
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

                if (response == null || response.Content == null)
                    output.ErrorCode = log.ErrorCode.Value;
                    output.ErrorDescription = log.ErrorDescription;
                    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
                    if (mobiShastraResponseList == null || !mobiShastraResponseList.Any())
                    {
                        log.ErrorCode = 99; // Empty Response List
                        log.ErrorDescription = $"MobiShastra-Empty Response List";
                        output.ErrorDescription = log.ErrorDescription;
                        return output;
                    }
                    var mobiShastraResponse = mobiShastraResponseList.FirstOrDefault();
                    if (mobiShastraResponse == null)
                    {
                        log.ErrorCode = 99; // Empty Response
                        log.ErrorDescription = $"MobiShastra-Empty Response";
                        output.ErrorDescription = log.ErrorDescription;
                        return output;
                    }
                    if (mobiShastraResponse.ResponseString != "Send Successful")
                    {
                        log.ErrorCode = 99; // Faild to Send
                        log.ErrorDescription = $"MobiShastra-{mobiShastraResponse.ResponseString}";
                        output.ErrorDescription = log.ErrorDescription;
                        return output;

                    log.ErrorCode = (int)ESTCSmsResponse.Success;
                    log.ErrorDescription = "Success";
                    output.ErrorCode = log.ErrorCode.Value;
                    output.ErrorDescription = log.ErrorDescription;
                    SMSLogsDataAccess.AddToSMSLogsDataAccess(log);
            }
                output.ErrorDescription = log.ErrorDescription;
        }
        public SMSOutput SendSmsBySTC(SMSModel model)
            log.ServerIP = Utilities.GetInternalServerIP();
            log.UserAgent = Utilities.GetUserAgent();
                string request = "{  \"userName\": \"" + RepositoryConstants.STCSmsAccountUsername + "\",  \"numbers\": \"" + model.PhoneNumber + "\",  \"userSender\": \"" + RepositoryConstants.STCSmsAccountSender + "\",  \"apiKey\": \"" + RepositoryConstants.STCSmsApiKey + "\",  \"msg\": \"" + model.MessageBody + "\"}";
                var content = new StringContent(request, System.Text.Encoding.UTF8, "application/json");
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                log.ServiceRequest = request;
                DateTime dtBeforeCalling = DateTime.Now;
                var response = _httpClient.Post(RepositoryConstants.STCSmsServiceUrl, content);
                log.ServiceResponseTimeInSeconds = DateTime.Now.Subtract(dtBeforeCalling).TotalSeconds;
                if (response == null || response.Content == null)
                    switch (stcResponse.Code)
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {
                            {

                output.ErrorDescription = log.ErrorDescription;
                output.ErrorDescription = log.ErrorDescription;
        public async Task SendWhatsAppMessageUpdateCustomCardAsync(string phoneNumber, string message, string method, string referenceId, string langCode,string make,string model,string plateText)
        {
            WhatsAppLog log = new WhatsAppLog();
            log.Method = method;
            try
            {
                phoneNumber = validatePhoneNumber(phoneNumber);
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
                if (response == null || response.Content == null)
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
                    }
            catch (Exception exp)
            {
                log.ErrorCode = (int)WhatsAppResponseErrorCodes.GenericError;
                log.ErrorDescription = exp.ToString();
                WhatsAppLogsDataAccess.AddToWhatsAppLogsDataAccess(log);
            }
        }
    }
}