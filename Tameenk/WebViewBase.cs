using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tameenk
{
    public abstract class WebViewBase<T> : WebViewPage<T>
    {
        private string _baseUrl;

        public string BaseUrl
        {
            get
            {
                _baseUrl = _baseUrl ?? Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                return _baseUrl;
            }
        }
    }
}