using Tameenk.Core.Domain.Enums.Policies;

namespace Tameenk.Services.Core.Policies
{
    public class PolicyUpdateFileDetails
    {
        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// File as byte array
        /// </summary>
        public byte[] FileByteArray { get; set; }

        /// <summary>
        /// Document type (ex: Policy, user id, Driver license ...)
        /// </summary>
        public PolicyUpdateRequestDocumentType DocType { get; set; }

        /// <summary>
        /// File mime type 
        /// </summary>
        public string FileMimeType{ get; set; }
    }
}
