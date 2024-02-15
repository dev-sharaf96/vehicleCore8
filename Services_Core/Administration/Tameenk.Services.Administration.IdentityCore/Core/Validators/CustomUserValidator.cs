using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Tameenk.Services.Administration.Identity.Core.Domain;

namespace Tameenk.Services.Administration.Identity.Core.Validators
{
    //public class CustomUserValidator<TUser> : UserValidator<TUser>
    //where TUser : AppUser
    //{
    //    private static readonly Regex EmailRegex = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    //    private readonly UserManager<TUser> _manager;

    //    public CustomUserValidator()
    //    {
    //    }

    //    public CustomUserValidator(UserManager<TUser> manager)
    //    {
    //        _manager = manager;
    //    }

    //    public async Task<IdentityResult> ValidateAsync(TUser item)
    //    {
    //        var errors = new List<string>();
    //        if (!EmailRegex.IsMatch(item.UserName))
    //            errors.Add("Enter a valid email address from ahmed hassan.");

    //        if (_manager != null)
    //        {
    //            var otherAccount = await _manager.FindByNameAsync(item.UserName);
    //            if (otherAccount != null && otherAccount.Id != item.Id)
    //                errors.Add("Select a different email address. An account has already been created with this email address.");
    //        }

    //        return errors.Any()
    //            ? IdentityResult.Failed(errors.ToArray())
    //            : IdentityResult.Success;
    //    }
    //}


    public class CustomUserValidator<TUser> : UserValidator<TUser, int>
       where TUser : AppUser
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="manager"></param>
        public CustomUserValidator(// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
UserManager<TUser, int> manager) : base(manager)
        {
            this.Manager = manager;
            AllowOnlyAlphanumericUserNames = false;
            RequireUniqueEmail = true;
        }

        private // TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
// TODO ASP.NET identity should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/identity.
UserManager<TUser, int> Manager { get; set; }

        /// <summary>
        ///     Validates a user before saving
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<IdentityResult> ValidateAsync(TUser item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            var errors = new List<string>();
            //  await ValidateUserName(item, errors);
            if (RequireUniqueEmail)
            {
                await ValidateEmail(item, errors);
            }
            if (errors.Count > 0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }
            return IdentityResult.Success;
        }

        //private async Task ValidateUserName(TUser user, List<string> errors)
        //{
        //    if (string.IsNullOrWhiteSpace(user.UserName))
        //    {
        //        errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.PropertyTooShort, "Name"));
        //    }
        //    else if (AllowOnlyAlphanumericUserNames && !Regex.IsMatch(user.UserName, @"^[A-Za-z0-9@_\.]+$"))
        //    {
        //        // If any characters are not letters or digits, its an illegal user name
        //        errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.InvalidUserName, user.UserName));
        //    }
        //    else
        //    {
        //        var owner = await Manager.FindByNameAsync(user.UserName);
        //        if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
        //        {
        //            errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.DuplicateName, user.UserName));
        //        }
        //    }
        //}

        // make sure email is not empty, valid, and unique
        private async Task ValidateEmail(TUser user, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                errors.Add("Email can't be null");
                return;
            }
            try
            {
                var m = new MailAddress(user.Email);
            }
            catch (FormatException)
            {
                errors.Add("Enter a valid email address from ahmed hassan.");
                return;
            }
            //var owner = await Manager.FindByEmailAsync(user.Email);
            //if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
            //{
            //    errors.Add(String.Format(CultureInfo.CurrentCulture, Resources.DuplicateEmail, email));
            //}
        }
    }
}
