using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Model
{
    public class PolicyScheduleResponse
    {
        public string ReferenceId { get; set; }
        public int StatusCode { get; set; }
        public List<Error> Errors { get; set; }
        private string policyFileUrl = null;
        public string PolicyFileUrl
        {
            get
            {
                if (string.IsNullOrEmpty(policyFileUrl))
                {
                    //@TODO: revisit this

                    string fileName = Guid.NewGuid() + ".pdf";
                    FileStream stream = new FileStream("C:\\inetpub\\wwwroot\\Policies\\" + fileName, FileMode.Create, FileAccess.Write);
                    stream.Write(PolicyFile, 0, PolicyFile.Length);
                    stream.Close();

                    policyFileUrl = "C:/inetpub/wwwroot/Policies/" + fileName;
                }

                return policyFileUrl;
            }
            set
            {
                policyFileUrl = value;
            }
        }
        public Byte[] PolicyFile { get; set; }
    }
}
