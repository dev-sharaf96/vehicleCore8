using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Wathq
{
    public class WathqOutput
    { 
        public enum ErrorCodes
        {
            Success = 1,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            ResFileIsNull,
            ServiceException,
            HttpStatusCodeNotOk
        }
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }
        public string ErrorDescription  {  get;  set;}
        public WathqResponseModel WathqResponseModel {get; set; }
    }
}
