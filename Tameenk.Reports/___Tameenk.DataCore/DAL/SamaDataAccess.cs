using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Common.Utilities;
using Tameenk.Data.Model;

namespace Tameenk.Data.DAL
{
   public class SamaDataAccess
    {
        public static List<Sama> GetSamaReport(int commandTimeout)
        {
            try
            {
                using (Tameenk context = new Tameenk())
                {
                    context.Database.CommandTimeout = commandTimeout;

                    List<Sama> samas = context.Database.SqlQuery<Sama>(@"
                                SELECT i.InvoiceDate AS 'InvoiceDate', i.InvoiceNo AS 'BcareInvoiceNumber', 'Taamnk' AS 'SCHEME',
                                CONCAT(d.FirstName, ' ', d.SecondName, ' ', d.LastName) AS 'PolicyHolder', checkout.Phone AS 'Mob', checkout.Email As 'Email', comp.NameAR AS 'Insurer',
                                p.PolicyNo AS 'PolicyNo', 'Payfort' AS 'PaymentMethod', res.CardNumber AS 'CardNumber', insuranceType.ArabicDescription AS 'InsuranceProduct',
                                i.ExtraPremiumPrice As 'ExtraPremiumPrice', i.Vat AS 'Vat'
                                FROM PayfortPaymentResponse res
                                INNER JOIN PayfortPaymentRequest req on res.RequestId = req.ID
                                INNER JOIN Invoice i on i.InvoiceNo = CONVERT(INT, SUBSTRING(req.ReferenceNumber, 4, 9))
                                INNER JOIN ProductType insuranceType on insuranceType.Code = i.InsuranceTypeCode
                                INNER JOIN InsuranceCompany comp on comp.InsuranceCompanyID = i.InsuranceCompanyId
                                INNER JOIN CheckoutDetails checkout on i.ReferenceId = checkout.ReferenceId
                                INNER JOIN Policy p on p.CheckOutDetailsId = checkout.ReferenceId
                                INNER JOIN Driver d on d.DriverId = checkout.MainDriverId
                                WHERE res.ResponseCode = 14000

                                Union ALL

                                SELECT i.InvoiceDate AS 'InvoiceDate', i.InvoiceNo AS 'BcareInvoiceNumber', 'Taamnk' AS 'SCHEME',
                                CONCAT(d.FirstName, ' ', d.SecondName, ' ', d.LastName) AS 'PolicyHolder', checkout.Phone AS 'Mob', checkout.Email As 'Email', comp.NameAR AS 'Insurer',
                                p.PolicyNo AS 'PolicyNo', 'Sadad' AS 'PaymentMethod', msg.BodysCustomerRefNo AS 'CardNumber', insuranceType.ArabicDescription AS 'InsuranceProduct',
                                i.ExtraPremiumPrice As 'ExtraPremiumPrice', i.Vat AS 'Vat'
                                FROM SadadNotificationResponse res
                                INNER JOIN SadadNotificationMessage msg on res.NotificationMessageId = msg.id
                                INNER JOIN Invoice i on i.InvoiceNo = CONVERT(INT, SUBSTRING(msg.BodysCustomerRefNo, 12, 9))
                                INNER JOIN ProductType insuranceType on insuranceType.Code = i.InsuranceTypeCode
                                INNER JOIN InsuranceCompany comp on comp.InsuranceCompanyID = i.InsuranceCompanyId
                                INNER JOIN CheckoutDetails checkout on i.ReferenceId = checkout.ReferenceId
                                INNER JOIN Policy p on p.CheckOutDetailsId = checkout.ReferenceId
                                INNER JOIN Driver d on d.DriverId = checkout.MainDriverId
                                WHERE res.Status = 'OK'
                                Union ALL

                                SELECT i.InvoiceDate AS 'InvoiceDate', i.InvoiceNo AS 'BcareInvoiceNumber', 'Taamnk' AS 'SCHEME',
                                CONCAT(d.FirstName, ' ', d.SecondName, ' ', d.LastName) AS 'PolicyHolder', checkout.Phone AS 'Mob', checkout.Email As 'Email', comp.NameAR AS 'Insurer',
                                p.PolicyNo AS 'PolicyNo', 'Riyad Bank' AS 'PaymentMethod', res.CardNum AS 'CardNumber', insuranceType.ArabicDescription AS 'InsuranceProduct',
                                i.ExtraPremiumPrice As 'ExtraPremiumPrice', i.Vat AS 'Vat'
                                FROM RiyadBankMigsResponse res
                                INNER JOIN RiyadBankMigsRequest req on res.RiyadBankMigsRequestId = req.Id
                                INNER JOIN CheckoutDetails checkout on checkout.ReferenceId = res.OrderInfo
                                INNER JOIN Invoice i on i.ReferenceId = checkout.ReferenceId
                                INNER JOIN ProductType insuranceType on insuranceType.Code = i.InsuranceTypeCode
                                INNER JOIN InsuranceCompany comp on comp.InsuranceCompanyID = i.InsuranceCompanyId
                                INNER JOIN Policy p on p.CheckOutDetailsId = checkout.ReferenceId
                                INNER JOIN Driver d on d.DriverId = checkout.MainDriverId
                                WHERE res.Message = 'Approved'
                                ORDER BY i.InvoiceDate desc ").ToList();

                    if (samas.Count > 0)
                        return samas;
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
    }
}
