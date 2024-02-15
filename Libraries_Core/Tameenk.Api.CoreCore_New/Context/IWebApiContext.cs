using Tameenk.Core.Domain.Enums;

namespace Tameenk.Api.Core.Context
{
    public interface IWebApiContext
    {
        /// <summary>
        /// Read Current request language and return it as Iso Code
        /// </summary>
        LanguageTwoLetterIsoCode CurrentLanguage { get; set; }
    }
}
