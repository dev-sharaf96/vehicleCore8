using Newtonsoft.Json;

namespace Tameenk.Services.Core
{
    public class DownloadClaimFileModel
    {
        [JsonProperty("fileData")]
        public  string? FileData { get; set; }

        [JsonProperty("fileExtension")]
        public  string? FileExtension { get; set; }
    }
}