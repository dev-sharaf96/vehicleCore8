using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Tameenk.Common.Utilities;
using Tameenk.Core.Data;
using Tameenk.Core.Domain;
using Tameenk.PaymentNotificationsApi.Services.Core;

namespace Tameenk.PaymentNotificationsApi.Services.Implementation
{
    public class HyperPayNotificationSersvice : IHyperPayNotificationSersvice
    {
        private readonly IRepository<HyperPayNotification> _hyperPayNotificationRepository;

        public HyperPayNotificationSersvice(IRepository<HyperPayNotification> hyperPayNotificationRepository)
        {
            _hyperPayNotificationRepository = hyperPayNotificationRepository;
        }

        #region HyperPay payment notifications

        public HyperPayNotificationOutput HandleHyperPayNotification(HyperPaysplitModel model, IEnumerable<string> iv, IEnumerable<string> tag)
        {
            var output = new HyperPayNotificationOutput();
            string notificationJson = string.Empty;
            HyperPayNotification hyperPayNotification = new HyperPayNotification();
            hyperPayNotification.CreatedDate = DateTime.Now;
            hyperPayNotification.ServerIp = Utilities.GetInternalServerIP();
            hyperPayNotification.UserIp = Utilities.GetUserIPAddress();
            hyperPayNotification.UserAgent = Utilities.GetUserAgent();
            try
            {
                if (Utilities.GetUserIPAddress() != "52.16.12.26")
                {
                    hyperPayNotification.ErrorCode = 401;
                    hyperPayNotification.ErrorDescirption = "Unauthorized request recived IP is:" + Utilities.GetUserIPAddress();
                    _hyperPayNotificationRepository.Insert(hyperPayNotification);
                    output.Code = hyperPayNotification.ErrorCode.ToString();
                    output.Message = hyperPayNotification.ErrorDescirption;
                    return output;
                }
                if (model == null)
                {
                    hyperPayNotification.ErrorCode = 2;
                    hyperPayNotification.ErrorDescirption = "Model is null";
                    _hyperPayNotificationRepository.Insert(hyperPayNotification);
                    output.Code = hyperPayNotification.ErrorCode.ToString();
                    output.Message = hyperPayNotification.ErrorDescirption;
                    return output;
                }
                hyperPayNotification.EncryptedResponse = JsonConvert.SerializeObject(model);
                if (iv == null || string.IsNullOrEmpty(iv.FirstOrDefault()))
                {
                    hyperPayNotification.ErrorCode = 3;
                    hyperPayNotification.ErrorDescirption = "X-Initialization-Vector is null";
                   
                    _hyperPayNotificationRepository.Insert(hyperPayNotification);
                    output.Code = hyperPayNotification.ErrorCode.ToString();
                    output.Message = hyperPayNotification.ErrorDescirption;
                    return output;
                }
                if (tag == null || string.IsNullOrEmpty(tag.FirstOrDefault()))
                {
                    hyperPayNotification.ErrorCode = 4;
                    hyperPayNotification.ErrorDescirption = "X-Authentication-Tag is null";
                    _hyperPayNotificationRepository.Insert(hyperPayNotification);
                    output.Code = hyperPayNotification.ErrorCode.ToString();
                    output.Message = hyperPayNotification.ErrorDescirption;
                    return output;
                }
              
                if (string.IsNullOrEmpty(model.EncryptedBody))
                {
                    hyperPayNotification.ErrorCode = 5;
                    hyperPayNotification.ErrorDescirption = "Encrypted data is null";
                    _hyperPayNotificationRepository.Insert(hyperPayNotification);
                    output.Code = hyperPayNotification.ErrorCode.ToString();
                    output.Message = hyperPayNotification.ErrorDescirption;
                    return output;
                }

                string exceptiopn = string.Empty;
                notificationJson =SecurityUtilities.HyperpayDecryption(model.EncryptedBody, iv.FirstOrDefault(), tag.FirstOrDefault(), out exceptiopn);
                if (!string.IsNullOrEmpty(exceptiopn))
                {
                    hyperPayNotification.ErrorCode = 6;
                    hyperPayNotification.ErrorDescirption = "Decryption returned " + exceptiopn;
                    _hyperPayNotificationRepository.Insert(hyperPayNotification);
                    output.Code = hyperPayNotification.ErrorCode.ToString();
                    output.Message = hyperPayNotification.ErrorDescirption;
                    return output;
                }
                hyperPayNotification.DecryptedResponse = notificationJson;
                HyperpaySplitNotificationModel notificationModel = JsonConvert.DeserializeObject<HyperpaySplitNotificationModel>(notificationJson);
                if (notificationModel == null)
                {
                    hyperPayNotification.ErrorCode = 7;
                    hyperPayNotification.ErrorDescirption = "Failed to Deserialize Object";
                    _hyperPayNotificationRepository.Insert(hyperPayNotification);
                    output.Code = hyperPayNotification.ErrorCode.ToString();
                    output.Message = hyperPayNotification.ErrorDescirption;
                    return output;
                }

                hyperPayNotification.Status = notificationModel.status;
                hyperPayNotification.Message = notificationModel.message;
                hyperPayNotification.Errors = (!string.IsNullOrEmpty(notificationModel.errors)) ? notificationModel.errors : null;
                hyperPayNotification.BatchId = notificationModel.data?.batchId;
                hyperPayNotification.BeneficiaryAccountId = notificationModel.data?.beneficiaryAccountId;
                hyperPayNotification.BeneficiaryName = notificationModel.data?.beneficiaryName;
             
                hyperPayNotification.ErrorCode = (notificationModel.status) ? 1 : 8;
                hyperPayNotification.ErrorDescirption = notificationModel.message;
                hyperPayNotification.ReferenceId = notificationModel.data?.transactions?[0].merchantTransactionId;
                hyperPayNotification.NotificationRefernece = notificationModel.data?.reference;
                hyperPayNotification.DebitAmount = notificationModel.data?.debitAmount;
                foreach (var item in notificationModel.data?.transactions)
                {
                    hyperPayNotification.Transactions = new List<HyperPayNotificationTransactions>();
                    hyperPayNotification.Transactions.Add(new HyperPayNotificationTransactions
                    {
                        MerchantTransactionId = item.merchantTransactionId,
                        PayoutTransferAmount = item.payoutTransferAmount,
                        UniqueId = item.uniqueId,
                        CreatedDate = DateTime.Now
                    });
                }
                _hyperPayNotificationRepository.Insert(hyperPayNotification);

                output.Code = "200";
                output.Message = "successfully recived";

                return output;
            }
            catch (Exception ex)
            {
                hyperPayNotification.ErrorCode = 500;
                hyperPayNotification.ErrorDescirption = ex.ToString();
                _hyperPayNotificationRepository.Insert(hyperPayNotification);
                output.Code = hyperPayNotification.ErrorCode.ToString();
                output.Message = ex.ToString();
                return output;
            }
        }

     
        #endregion
    }
}