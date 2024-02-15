using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.Localizations
{
    public interface ILocalizationService
    {
        /// <summary>
        /// Get all languages.
        /// </summary>
        /// <param name="pageSize">The page size.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <returns>Paged list fo all language</returns>
        IPagedList<Language> GetAllLanguage(int pageSize = int.MaxValue, int pageIndex = 0);

        /// <summary>
        /// Get the language by Id
        /// </summary>
        /// <param name="guid">The unique identifier of the language.</param>
        /// <returns>The language object.</returns>
        Language GetLanguageByGuid(Guid guid);

        /// <summary>
        /// Get the language by the english name in db.
        /// </summary>
        /// <param name="englishName">The english name of the language.</param>
        /// <returns>The language object.</returns>
        Language GetLanguageByEnglishName(string englishName);

        /// <summary>
        /// Get defualt language.
        /// </summary>
        /// <returns>The language object.</returns>
        Language GetDefaultLanguage();
    }
}
