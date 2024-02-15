using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Enums
{
    public class PromotionProgramOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            CanNotJoin,
            NullResponse,
            ServiceException,
            DeserializeError
        }
    }
}
