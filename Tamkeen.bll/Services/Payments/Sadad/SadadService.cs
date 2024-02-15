using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;
using TameenkDAL.UoW;
using Tamkeen.bll.Model;
using Tamkeen.bll.Services.Sadad.Models;

namespace Tamkeen.bll.Services.Sadad
{
    public class SadadService
    {
        private readonly ITameenkUoW _tameenkUoW;
        public SadadService(ITameenkUoW tameenkUoW)
        {
            _tameenkUoW = tameenkUoW;
        }

        public async Task<SadadPaymentResponse> PayUsingSadad(PaymentRequestModel paymentRequestModel)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            SadadPaymentResponse result = new SadadPaymentResponse();

            DateTime NowDateTime = DateTime.Now;

            Models.SadadRequest sadadRequest = new Models.SadadRequest();
            sadadRequest.BillerID = ServicesConst.SadadBillerID;
            sadadRequest.ExactFlag = ServicesConst.SadadExactFlag;
            sadadRequest.BillsCustomerAccount = "03" + paymentRequestModel.InvoiceNumber.ToString();
            sadadRequest.BillsCustomerName = paymentRequestModel.CustomerNameEn;
            sadadRequest.BillslAccountStatus = ServicesConst.SadadBillslAccountStatus;
            sadadRequest.BillsAmountDue = Math.Round(paymentRequestModel.PaymentAmount, 2, MidpointRounding.AwayFromZero);
            sadadRequest.BillsOpenDate = NowDateTime.ToString("yyyy-MM-dd");
            sadadRequest.BillsDueDate = NowDateTime.ToString("yyyy-MM-dd");
            sadadRequest.BillsExpiryDate = (NowDateTime + TimeSpan.FromDays(1)).ToString("yyyy-MM-dd");
            sadadRequest.BillsCloseDate = (NowDateTime + TimeSpan.FromDays(1)).ToString("yyyy-MM-dd");

            // send sadad request to sadad service
            var response = await requestSadadPayment(sadadRequest);


            saveSadadRequestAndResponseInDb(sadadRequest, response);

            if (response != null)
            {
                if (response.errorCode == 0)
                {
                    result.Status = EStatus.Succeeded;
                    result.ErrorMessage = null;
                    result.ReferenceNumber = string.Format("SUBS {0}{1}{2}", sadadRequest.BillerID, sadadRequest.ExactFlag, sadadRequest.BillsCustomerAccount);
                    result.BillDueDate = NowDateTime + TimeSpan.FromHours(16);
                }
                else
                {
                    result.Status = EStatus.Failed;
                    result.ErrorMessage = response.description;
                    result.ReferenceNumber = null;
                }
            }
            else
            {
                result.Status = EStatus.Failed;
                result.ErrorMessage = "Failed to read/parse response from RyadBank/Sadad service";
                result.ReferenceNumber = null;
            }

            return result;
        }

        private void saveSadadRequestAndResponseInDb(Models.SadadRequest sadadRequest, SadadResponse sadadResponse)
        {
            Tameenk.Core.Domain.Entities.Payments.Sadad.SadadRequest sadadReqData = new Tameenk.Core.Domain.Entities.Payments.Sadad.SadadRequest();
            sadadReqData.CustomerAccountNumber = sadadRequest.BillsCustomerAccount;
            sadadReqData.CustomerAccountName = sadadRequest.BillsCustomerName;
            sadadReqData.BillAmount = sadadRequest.BillsAmountDue;
            sadadReqData.BillOpenDate = DateTime.ParseExact(sadadRequest.BillsOpenDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            sadadReqData.BillDueDate = DateTime.ParseExact(sadadRequest.BillsDueDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            sadadReqData.BillExpiryDate = DateTime.ParseExact(sadadRequest.BillsExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            sadadReqData.BillCloseDate = DateTime.ParseExact(sadadRequest.BillsCloseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            sadadReqData.BillMaxAdvanceAmount = sadadRequest.BillsMaxAdvanceAmt;
            sadadReqData.BillMinAdvanceAmount = sadadRequest.BillsMinAdvanceAmt;
            sadadReqData.BillMinPartialAmount = sadadRequest.BillsMinPartialAmt;

            if (sadadResponse != null)
            {
                Tameenk.Core.Domain.Entities.Payments.Sadad.SadadResponse sadadResponseData = new Tameenk.Core.Domain.Entities.Payments.Sadad.SadadResponse();
                sadadResponseData.SadadRequest = sadadReqData;
                sadadResponseData.Status = sadadResponse.status;
                sadadResponseData.ErrorCode = sadadResponse.errorCode;
                sadadResponseData.Description = sadadResponse.description;
                sadadResponseData.TrackingId = sadadResponse.trackingId;

                sadadReqData.SadadResponses.Add(sadadResponseData);
            }

            _tameenkUoW.SadadRequestsRepository.Insert(sadadReqData);
            _tameenkUoW.Save();
        }

        private async Task<SadadResponse> requestSadadPayment(Models.SadadRequest sadadRequest)
        {
            try
            {
                byte[] bytesToWrite = GenerateRequestXML(sadadRequest);
                var request = HttpWebRequest.Create(ServicesConst.SadadUrl) as HttpWebRequest;
                request.Method = "POST";
                request.ContentLength = bytesToWrite.Length;
                request.ContentType = "text/xml; charset=utf-8";
                request.ClientCertificates.Add(
                    new X509Certificate2(
                        HttpContext.Current.Server.MapPath(ServicesConst.SadadKeyRelativePath),
                        "b2b123",
                        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet));
                ServicePointManager.ServerCertificateValidationCallback = acceptAllCertifications;

                // write to request stream and close
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());
                Stream dataStream = response.GetResponseStream();

                XmlSerializer serializer = new XmlSerializer(typeof(SadadResponse));
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    XDocument xDoc = XDocument.Load(reader);
                    var unwrappedResponse = xDoc.Descendants(ServicesConst.SoapEnvelopNameSpace + "Body").First().FirstNode;
                    var responseBodyAsObj = serializer.Deserialize(unwrappedResponse.CreateReader()) as SadadResponse;
                    return responseBodyAsObj;
                }
            }
            catch (Exception ex)
            {
                SadadResponse sadadResponse = new SadadResponse();
                sadadResponse.errorCode = 5;
                sadadResponse.description = ex.ToString();

                return sadadResponse;
            }
        }

        private byte[] GenerateRequestXML(Models.SadadRequest sadadRequest)
        {
            StringBuilder xmlsb = new StringBuilder();
            xmlsb.AppendLine("<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:bil='http://sadadonlinepayment.com/billreq' >");
            xmlsb.AppendLine("<soapenv:Header/>");
            xmlsb.AppendLine("<soapenv:Body>");
            xmlsb.AppendLine("<SadadPmtReq>");
            xmlsb.AppendLine("<BillingAcct>");
            xmlsb.AppendLine("<BillerId>" + sadadRequest.BillerID + "</BillerId>");
            xmlsb.AppendLine("<ExactPmtRq>" + sadadRequest.ExactFlag + "</ExactPmtRq>");
            xmlsb.AppendLine("<CustomerAccount>" + sadadRequest.BillsCustomerAccount + "</CustomerAccount>");
            xmlsb.AppendLine("<CustomerName>" + sadadRequest.BillsCustomerName + "</CustomerName>");
            xmlsb.AppendLine("<Status>ACTIVE</Status>");
            xmlsb.AppendLine("<AmountDue>" + sadadRequest.BillsAmountDue + "</AmountDue>");
            xmlsb.AppendLine("<BillOpenDate>" + sadadRequest.BillsOpenDate + "</BillOpenDate>");
            xmlsb.AppendLine("<BillDueDate>" + sadadRequest.BillsDueDate + "</BillDueDate>");
            xmlsb.AppendLine("<BillExpiryDate>" + sadadRequest.BillsExpiryDate + "</BillExpiryDate>");
            xmlsb.AppendLine("<BillCloseDate>" + sadadRequest.BillsCloseDate + "</BillCloseDate>");
            //param += "<MaxAdvanceAmount>" + MaxAdvanceAmount + "</MaxAdvanceAmount>";
            //param += "<MinAdvanceAmount>" + MinAdvanceAmount + "</MinAdvanceAmount>";
            //param += "<MinPartialAmount>" + MinPartialAmount + "</MinPartialAmount>";
            xmlsb.AppendLine("</BillingAcct>");
            xmlsb.AppendLine("</SadadPmtReq>");
            xmlsb.AppendLine("</soapenv:Body>");
            xmlsb.AppendLine("</soapenv:Envelope>");

            string xmlString = xmlsb.ToString();
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(xmlString);
        }

        private bool acceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
