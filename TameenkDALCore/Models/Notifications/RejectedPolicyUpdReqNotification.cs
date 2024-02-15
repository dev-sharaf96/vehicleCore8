﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Messages;

namespace TameenkDAL.Models.Notifications
{
    public class RejectedPolicyUpdReqNotification : NotificationModel
    {
        #region Ctor
        public RejectedPolicyUpdReqNotification()
        {

        }
        public RejectedPolicyUpdReqNotification(Notification notification)
        {
            CreatedAt = notification.CreatedAt;
            Id = notification.Id;
            Status = notification.Status;
            Type = notification.Type;
        }
        #endregion

        public string PolicyUpdateRequestGuid { get; set; }
        public string PolicyId { get; set; }
    }
}