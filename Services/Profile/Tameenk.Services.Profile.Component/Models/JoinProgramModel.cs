using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Profile.Component
{
    public class JoinProgramModel
    {
        public string UserEmail { get; set; }
        public string Nin { get; set; }
        public string Channel { get; set; }
        public string Lang { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string FileBase64String { get; set; }
        public byte[] FileBytes { get; set; }
        public string FileName { get; set; }
        public string FileExt { get; set; }
        public long FileSize { get; set; }
        public int JoinTypeId { get; set; }
    }
}