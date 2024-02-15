using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Enums;
using TameenkDAL.UoW;
using Tamkeen.bll.Model;
using Tamkeen.bll.Services;

namespace Tamkeen.bll.Business
{
    public class CheckoutBusiness
    {
        readonly ITameenkUoW _tameenkUoW;
        public CheckoutBusiness(ITameenkUoW tameenkUoW)
        {
            _tameenkUoW = tameenkUoW;
        }
        

        /// <summary>
        /// Get Policy update request payment model
        /// </summary>
        /// <param name="policyUpdReqRefId">Policy update request reference Id</param>
        /// <param name="userId">User id</param>
        /// <param name="userEmail">User name</param>
        /// <param name="userFullName">User Full Name</param>
        /// <param name="amount">Amount</param>
        /// <returns></returns>
        public PaymentRequestModel GetPolicyUpdReqPayment(string policyUpdReqRefId, CheckoutDetail checkoutDetail, decimal amount)
        {
            //create invoice data
            var invoice = CreatePolicyUpdInvoiceData(policyUpdReqRefId, checkoutDetail.UserId, amount);
            //save invoice
            _tameenkUoW.InvoiceRepository.Insert(invoice);
            _tameenkUoW.Save();
            return new PaymentRequestModel()
            {
                UserEmail = checkoutDetail.Email,
                UserId = checkoutDetail.UserId,
                CustomerNameAr = $"{checkoutDetail.Driver.FirstName} {checkoutDetail.Driver.SecondName} {checkoutDetail.Driver.LastName}",
                CustomerNameEn = $"{checkoutDetail.Driver.EnglishFirstName} {checkoutDetail.Driver.EnglishSecondName} {checkoutDetail.Driver.EnglishLastName}",
                PaymentAmount = amount,
                InvoiceNumber = invoice.InvoiceNo
            };
        }
        


        /// <summary>
        /// Create invoice for Policy update request
        /// </summary>
        /// <param name="referenceId">Reference Id for Policy update request</param>
        /// <param name="userId">User id</param>
        /// <param name="amount">Policy update request Amount</param>
        /// <returns>Invoice entity</returns>
        private Tameenk.Core.Domain.Entities.Invoice CreatePolicyUpdInvoiceData(string referenceId, string userId, decimal amount)
        {
            return new Tameenk.Core.Domain.Entities.Invoice()
            {
                InvoiceNo = getNewInvoiceNumber(),
                ReferenceId = referenceId,
                InvoiceDate = DateTime.Now,
                InvoiceDueDate = DateTime.Now + TimeSpan.FromHours(16),
                UserId = userId,
                TotalPrice = amount
            };
        }

        private int getNewInvoiceNumber()
        {
            Random rnd = new Random(System.Environment.TickCount);
            int invoiceNumber = rnd.Next(111111111, 999999999);
            if (_tameenkUoW.InvoiceRepository.Exists(i => i.InvoiceNo == invoiceNumber))
                return getNewInvoiceNumber();

            return invoiceNumber;
        }

        public string GetLatestUsedIbanByUser(string userId)
        {
            return _tameenkUoW.CheckoutRepository.GetLatestUsedIbanByUser(userId);
        }

        
    }
}
