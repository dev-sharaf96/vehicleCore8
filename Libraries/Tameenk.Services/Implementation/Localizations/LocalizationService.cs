using System;
using System.Linq;
using Tameenk.Core;
using Tameenk.Core.Caching;
using Tameenk.Core.Data;
using Tameenk.Core.Domain.Entities;
using Tameenk.Services.Localizations;

namespace Tameenk.Services.Implementation.Localizations
{
    public class LocalizationService : ILocalizationService
    {
        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly ICacheManager _cacheManager;
        private const string LANGUAGE_ALL_CACHE_KEY = "Tammenk.language.all";
        private const string LANGUAGE_BY_GUID_CACHE_KEY = "Tammenk.language.by.guid.{0}";
        private const string LANGUAGE_BY_ENGLISH_NAME_CACHE_KEY = "Tammenk.language.by.english.name.{0}";

        #endregion

        #region Ctor

        public LocalizationService(ICacheManager cacheManager, IRepository<Language> languageRepository)
        {
            _cacheManager = cacheManager;
            _languageRepository = languageRepository;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the language by the english name in db.
        /// </summary>
        /// <param name="englishName">The english name of the language.</param>
        /// <returns>The language object.</returns>
        public Language GetLanguageByEnglishName(string englishName)
        {
            var cacheKey = string.Format(LANGUAGE_BY_ENGLISH_NAME_CACHE_KEY, englishName);
            return _cacheManager.Get(cacheKey, () => {
                return _languageRepository.Table.FirstOrDefault(l => l.NameEN.Equals(englishName, StringComparison.OrdinalIgnoreCase));
            });
        }

        /// <summary>
        /// Get the language by Id
        /// </summary>
        /// <param name="guid">The unique identifier of the language.</param>
        /// <returns>The language object.</returns>
        public Language GetLanguageByGuid(Guid guid)
        {
            var cacheKey = string.Format(LANGUAGE_BY_GUID_CACHE_KEY, guid);
            return _cacheManager.Get(cacheKey, () => {
                return _languageRepository.Table.FirstOrDefault(l => l.Id == guid);
            });
        }

        /// <summary>
        /// Get all languages.
        /// </summary>
        /// <param name="pageSize">The page size.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <returns>Paged list fo all language</returns>
        public IPagedList<Language> GetAllLanguage(int pageSize = int.MaxValue, int pageIndex = 0)
        {
            return new PagedList<Language>(_languageRepository.Table, pageIndex, pageSize);
        }

        /// <summary>
        /// Get defualt language.
        /// </summary>
        /// <returns>The language object.</returns>
        public Language GetDefaultLanguage()
        {
            return _languageRepository.Table.FirstOrDefault(l => l.isDefault);
        }

        #endregion
    }
}
