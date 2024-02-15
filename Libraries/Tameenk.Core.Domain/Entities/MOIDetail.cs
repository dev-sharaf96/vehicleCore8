using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Core.Domain.Entities
{

    public class MOIDetail : BaseEntity
    {
        public int Id { get; set; }
        /// <summary>
        /// User Id
        /// </summary>
        [Required]
        public string UserId { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; set; }

        [NotMapped]
        public string FileUrl { get; set; }

        /// <summary>
        /// File as byte array
        /// </summary>
        [Column(TypeName = "image")]
        public byte[] FileByteArray { get; set; } = new byte[0];

        /// <summary>
        /// File mime type 
        /// </summary>
        public string FileMimeType { get; set; }

        public bool? Approved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
