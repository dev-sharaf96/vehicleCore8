// MIGS payment gateway using Asp.Net MVC5
// https://github.com/mwd-au/MIGS-payment-gateway-MVC5
// Based off https://gist.github.com/samnaseri/2211309

using MigsPayments.Helpers;
using MigsPayments.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MigsPayments.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Index()
        {
            var request = new PaymentRequestModel
            {
                MerchantId = "2000000061",
                OrderInfo = "BMT123456",
                Version = "1",
                Command = "pay",
                MerchTxnRef = $"BMT{DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")}",
                Amount = "500",
                ReturnUrl = Url.RouteUrl("Default", new { action = "Response", controller = "Payment", String.Empty }, Request.Url.Scheme),
                Locale = "en"
            };

            return View(request);

        }


        public ActionResult Response()
        {
            /*
             * >>>> Sample response <<<< 
             vpc_3DSECI=02&vpc_3DSXID=8gdweqMdylGUiMKQrlVfe8080IM%3D&vpc_3DSenrolled=Y&vpc_3DSstatus=Y&vpc_AVSResultCode=Unsupported&vpc_AcqAVSRespCode=Unsupported&vpc_AcqCSCRespCode=M&vpc_AcqResponseCode=00&vpc_Amount=50&vpc_AuthorizeId=807812&vpc_BatchNo=20181019&vpc_CSCResultCode=M&vpc_Card=MC&vpc_CardNum=xxxxxxxxxxxx9019&vpc_Command=pay&vpc_Locale=en&vpc_MerchTxnRef=BMT201810181512304490&vpc_Merchant=2000000061&vpc_Message=Approved&vpc_OrderInfo=BMT123456&vpc_ReceiptNo=829202582635&vpc_SecureHash=61A6A16D89749E74D509169925FF79D6F946DD8B71A3BE29103C8E18CFABE064&vpc_SecureHashType=SHA256&vpc_TransactionNo=2090000304&vpc_TxnResponseCode=0&vpc_VerSecurityLevel=05&vpc_VerStatus=Y&vpc_VerToken=jBSW5SV2q870CBADi6uoCRUAAAA%3D&vpc_VerType=3DS&vpc_Version=1
             * >>>> End of sample response <<<<<
             */

            var PaymentStatus = "none";

            try
            {
                string hashSecret = ConfigurationManager.AppSettings["MigsSecureHashSecret"];
                var secureHash = Request.QueryString["vpc_SecureHash"];
                var txnResponseCode = Request.QueryString["vpc_TxnResponseCode"];
                if (!string.IsNullOrEmpty(secureHash))
                {
                    if (!string.IsNullOrEmpty(hashSecret))
                    {
                        var rawHashData =  Request.QueryString.AllKeys.Where(k => k != "vpc_SecureHash" && k != "vpc_SecureHashType").ToDictionary(key => key, k =>  Request.QueryString[k]);
                        var signature =  PaymentHelperMethods.CreateSHA256Signature(rawHashData,hashSecret);
                        if (signature != secureHash || txnResponseCode != "0")
                        {
                            PaymentStatus = "invalid";
                            //return View("Error", new ApplicationException("Invalid request."));
                        }
                        else
                        {
                            PaymentStatus = "approved";
                        }
                    }
                }

                ViewBag.PaymentStatus = PaymentStatus;

                var vpcResponse = new PaymentResponse(Request);
                return View(vpcResponse);

            }
            catch (Exception ex)
            {

                var message = "Exception encountered. " + ex.Message;
                return View("Error", ex);

            }
        }

        // POST: Payment/InitiatePayment
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InitiatePayment(PaymentRequestModel model)
        {
            try
            {

                //region parameters
                var VPC_URL = "https://migs.mastercard.com.au/vpcpay";
                var paymentRequest = new PaymentRequest
                {
                    Amount = model.Amount,
                    MerchTxnRef = model.MerchTxnRef,
                    OrderInfo = model.OrderInfo,
                    ReturnUrl = model.ReturnUrl,
                    AccessCode = model.AccessCode,
                    Merchant = model.MerchantId,
                    Version = model.Version,
                    Command = model.Command,
                    Locale = model.Locale

                };

                string hashSecret = model.SecureHashSecret;
                //endregion


                //region redirect to payment gateway
                var transactionData = paymentRequest.GetParameters().OrderBy(t => t.Key, new VPCStringComparer()).ToList();
                // Add custom data, transactionData.Add(new KeyValuePair<string, string>("Title", title));
                var paramters = string.Join("&", transactionData.Select(item => $"{item.Key}={item.Value}"));
                var urlParamters = string.Join("&", transactionData.Select(item => $"{HttpUtility.UrlEncode(item.Key)}={HttpUtility.UrlEncode(item.Value)}"));
                var redirectUrl = $"{VPC_URL}?{urlParamters}";
                model.GeneratedHash = PaymentHelperMethods.CreateSHA256Signature(paymentRequest.GetParameters().OrderBy(t => t.Key, new VPCStringComparer()), hashSecret);
                if (!string.IsNullOrEmpty(hashSecret))
                    redirectUrl = string.Format("{0}&vpc_SecureHash={1}&vpc_SecureHashType=SHA256", redirectUrl, model.GeneratedHash);
                model.MessageToHash = paramters;
                model.RedirectUrl = redirectUrl;
                return View("Index" , model);
                //endregion

            }
            catch (Exception ex)
            {
                var message = "Exception encountered. " + ex.Message;
                return View("Error", ex);
            }
        }
    }
}