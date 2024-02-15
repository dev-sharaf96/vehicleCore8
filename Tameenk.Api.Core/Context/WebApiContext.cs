using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Api.Core.Context
{
    public class WebApiContext : IWebApiContext
    {
        #region Fields
        private readonly HttpContextBase _httpContext;
        private LanguageTwoLetterIsoCode? _currentLanguage;
        #endregion

        #region Ctor
        public WebApiContext(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Read Current request language and return it as Iso Code
        /// </summary>
        public LanguageTwoLetterIsoCode CurrentLanguage
        {
            get
            {
                if (_currentLanguage != null)
                    return _currentLanguage.Value;

                //check if there is a lang value in the header
                var headerVals = _httpContext.Request.Headers.GetValues("Language");
                if (headerVals != null)
                {
                    var val = headerVals.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        int languageId = 1;
                        if (int.TryParse(val, out languageId))
                        {
                            _currentLanguage = (LanguageTwoLetterIsoCode)languageId;
                        }
                    }
                }
                _currentLanguage = _currentLanguage.GetValueOrDefault(LanguageTwoLetterIsoCode.Ar);
                //return default value Ar if there is no header language
                return _currentLanguage.GetValueOrDefault(LanguageTwoLetterIsoCode.Ar);
            }
            set
            {
                _currentLanguage = value;
            }

        }
        #endregion

    }
}
