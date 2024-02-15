using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos.Profile;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services;

namespace Tameenk.Models
{
    public class CorporateUsersViewModel
    {
        public List<CorporateUserModel> CorporateUsersList { get; set; }
        public int CorporateUsersTotalCount { get; set; }
        public int CurrentPage { get; set; }
        public string Lang { get; internal set; }
    }
}