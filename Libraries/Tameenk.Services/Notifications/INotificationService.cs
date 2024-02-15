using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Notifications
{
    public interface INotificationService
    {
        Task SendEmailAsync(string toEmailAddress, string emailSubject, string emailPlainText, IEnumerable<EmailAttacmentFileModel> attachementFiles = null, bool IsBodyHtml = false);

        // TODO: Check this method it's not working properly
        Task SendEmailAsync(IEnumerable<string> toEmailAddresses, string emailSubject, string emailPlainText, IEnumerable<EmailAttacmentFileModel> attachementFiles = null, bool IsBodyHtml = false);

    }
}
