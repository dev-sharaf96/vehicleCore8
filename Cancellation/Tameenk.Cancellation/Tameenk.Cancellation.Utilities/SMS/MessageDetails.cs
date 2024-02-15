using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Cancellation.Utilities
{
    public class MessageDetails
    {
        /// <summary>
        /// MessageDate
        /// </summary>
        public DateTime MessageDate;
        /// <summary>
        /// MessageTarget
        /// </summary>
        public string MessageTarget;
        /// <summary>
        /// ReferenceID
        /// </summary>
        public string ReferenceID;
        /// <summary>
        /// Alias
        /// </summary>
        public string Alias;
        /// <summary>
        /// MessageOriginator
        /// </summary>
        public string MessageOriginator;
        /// <summary>
        /// MessageStatus
        /// </summary>
        public string MessageStatus;
        /// <summary>
        /// MessageBody
        /// </summary>
        public string MessageBody;
        /// <summary>
        /// MessageOption
        /// </summary>
        public int MessageOption = 2;
    }
}
