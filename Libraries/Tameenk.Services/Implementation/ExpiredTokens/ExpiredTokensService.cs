using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Core;
using Tameenk.Services.Core.Files;

namespace Tameenk.Services.Implementation
{
    public class ExpiredTokensService : IExpiredTokensService
    {
        private readonly IRepository<ExpiredTokens> _expiredCookiesRepository;


        public ExpiredTokensService(IRepository<ExpiredTokens> expiredCookiesRepository)
        {
            this._expiredCookiesRepository = expiredCookiesRepository;
        }
        public ExpiredTokens GetFromExpiredCookie(string key)
        {
            return _expiredCookiesRepository.TableNoTracking.Where(x => x.Key == key).FirstOrDefault();
        }
    }
}
