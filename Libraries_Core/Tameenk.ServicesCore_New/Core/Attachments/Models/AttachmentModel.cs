using System.Net.Mime;

namespace Tameenk.Services.Core.Attachments.Models
{
    public class AttachmentModel
    {
        public string? FileName { get; set; }
        public byte[]? FileAsByteArray { get; set; }
        public ContentType? ContentType { get; set; }
    }
}
