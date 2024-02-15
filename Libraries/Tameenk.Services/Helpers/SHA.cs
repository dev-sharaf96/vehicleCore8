using System;
using System.Security.Cryptography;
using System.Text;

namespace Tameenk.Services.Helpers
{
    public class SHA
    {
        public static String Sha256HashRequest(string signature)//(IEnumerable<BLL.PaymentRequestDetails> paymentrequestdetails)
        {
            StringBuilder Sb = new StringBuilder();
            using (System.Security.Cryptography.SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(signature));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            var temp = Sb.ToString();
            return temp;
        }
    }
}