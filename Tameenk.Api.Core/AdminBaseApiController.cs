using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Tameenk.Api.Core.ActionResults;
using Tameenk.Api.Core.Attributes;
using Tameenk.Api.Core.Models;
using Tameenk.Core.Configuration;
using Tameenk.Core.Infrastructure;
using Tameenk.Security.CustomAttributes;
using Tameenk.Services.Administration.Identity;
using Tameenk.Services.Core.Http;
using Tameenk.Services.Extensions;
using Tameenk.Services.Logging;

namespace Tameenk.Api.Core
{
    [WebApiLanguage]
    [AdminSecurityAuthorizeAttribute]
    public abstract class AdminBaseApiController : ApiController
    {
        private ILogger _logger;
        private IHttpClient _httpClient; 
        private string _accessToken;
        private TameenkConfig _config;
        [NonAction]
        public RawJsonActionResult Ok<T>(T content, int totalCount = 0)
        {
            return new RawJsonActionResult(new CommonResponseModel<T>(content, totalCount).Serialize());
        }
        public IHttpActionResult Single(object result)
        {
            return base.Ok(result);
        }

        [NonAction]
        public RawJsonActionResult Error(IEnumerable<ErrorModel> errors, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            var commonModel = new CommonResponseModel<bool>(false);
            commonModel.Errors = errors;
            return new RawJsonActionResult(commonModel.Serialize(), httpStatusCode);
        }


        [NonAction]
        public RawJsonActionResult Error(ModelStateDictionary modelState, string errorMessage, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            TransalateModelStatePropertyName(modelState, ActionContext);
            var commonModel = new CommonResponseModel<ModelStateDictionary>(modelState);
            commonModel.Errors = new List<ErrorModel>() { new ErrorModel(errorMessage) };
            return new RawJsonActionResult(commonModel.Serialize(), httpStatusCode);
        }
        [NonAction]
        public RawJsonActionResult Error(IEnumerable<string> errors, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            return Error(errors.Select(e => new ErrorModel(e)), httpStatusCode);
        }

        [NonAction]
        public RawJsonActionResult Error(string error, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            return Error(new List<string> { error }, httpStatusCode);
        }


        [NonAction]
        public RawJsonActionResult Error(Exception ex)
        {
            var logKey = $"api_{DateTime.Now.GetTimestamp()}";
            Logger.Log($"Api error [Key={logKey}]", ex, LogLevel.Error);
            var error = new ErrorModel
            {
                Code = logKey,
                Description = ex.GetBaseException().Message
            };
            return Error(new List<ErrorModel> { error });
        }

        protected string AuthorizationToken
        {
            get
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(_accessToken) && Request.Headers != null && Request.Headers.Authorization != null)
                    {
                        _accessToken = Request.Headers.Authorization.Parameter;
                    }
                    return _accessToken;

                }
                catch (Exception ex)
                {
                    var logId = DateTime.Now.GetTimestamp();
                    _logger.Log($"Base api controller -> GetAccessToken [key={logId}]", ex);
                    return "";
                }
            }
        }



        public class AccessTokenResult
        {
            [JsonProperty("access_token")]
            public string access_token { get; set; }
            [JsonProperty("expires_in")]
            public int expires_in { get; set; }


        }


        public ILogger Logger
        {
            get
            {
                _logger = _logger ?? EngineContext.Current.Resolve<ILogger>();
                return _logger;
            }
        }


        protected IHttpClient HttpClient
        {
            get
            {
                _httpClient = _httpClient ?? EngineContext.Current.Resolve<IHttpClient>();
                return _httpClient;
            }
        }


        protected TameenkConfig Config
        {
            get
            {
                _config = _config ?? EngineContext.Current.Resolve<TameenkConfig>();
                return _config;
            }
        }

        [NonAction]
        private void TransalateModelStatePropertyName(ModelStateDictionary modelState, HttpActionContext actionContext)
        {
            var descriptor = actionContext.ActionDescriptor;

            if (descriptor != null)
            {
                var parameters = descriptor.GetParameters();

                var subParameterIssues = modelState.Keys
                                                   .Where(s => s.Contains("."))
                                                   .Where(s => modelState[s].Errors.Any())
                                                   .GroupBy(s => s.Substring(0, s.IndexOf('.')))
                                                   .ToDictionary(g => g.Key, g => g.ToArray());

                foreach (var parameter in parameters)
                {
                    var argument = actionContext.ActionArguments[parameter.ParameterName];

                    if (subParameterIssues.ContainsKey(parameter.ParameterName))
                    {
                        var subProperties = subParameterIssues[parameter.ParameterName];
                        foreach (var subProperty in subProperties)
                        {
                            var propName = subProperty.Substring(subProperty.IndexOf('.') + 1);
                            var propertyName = $"{subProperty.Split('.')[0]}.{GetPropertyName(propName, parameter.ParameterType)}";
                            if (!string.IsNullOrWhiteSpace(propertyName) && modelState[subProperty] != null)
                            {
                                if (modelState[propertyName] != null 
                                    && !subProperty.Equals(propertyName, StringComparison.Ordinal))
                                {
                                    var modelErrors = modelState[subProperty].Errors.ToList();

                                    modelState.Remove(subProperty);
                                    // in case the name was not equal due to case senstivity
                                    // the remove will remove modelState[propertyName]
                                    if (modelState[propertyName] == null)
                                        modelState.Add(propertyName, new ModelState());

                                    foreach (var error in modelErrors)
                                    {
                                        modelState[propertyName].Errors.Add(error);
                                    }
                                }
                                else if (modelState[propertyName] == null)
                                {
                                    modelState.Add(propertyName, modelState[subProperty]);
                                    modelState.Remove(subProperty);
                                }
                            }
                        }
                    }


                }
            }

        }
        private string GetPropertyName(string propertyName, Type type)
        {
            string name = null;
            var parts = propertyName.Split('.');
            if (parts.Length < 1)
            {
                return null;
            }
            string arrayIndex = string.Empty;
            if (parts[0].Contains('['))
            {
                var arrayParts = parts[0].Split('[');
                parts[0] = arrayParts.FirstOrDefault();
                arrayIndex = $"[{arrayParts.LastOrDefault()}";
            }
            var parentProp = type.GetProperty(parts[0]);
            if (parentProp == null) { return propertyName; }
            name = $"{GetParameterName(parentProp)}{arrayIndex}";
            if (parts.Length > 1)
            {
                var childPorpName = GetPropertyName(parts[1], parentProp.PropertyType);
                return $"{name}.{childPorpName}";
            }
            return name;
        }
        private string GetParameterName(PropertyInfo property)
        {
            var dataMemberAttribute = property.GetCustomAttributes<DataMemberAttribute>().FirstOrDefault();
            if (dataMemberAttribute?.Name != null)
            {
                return dataMemberAttribute.Name;
            }

            var jsonProperty = property.GetCustomAttributes<JsonPropertyAttribute>().FirstOrDefault();
            if (jsonProperty?.PropertyName != null)
            {
                return jsonProperty.PropertyName;
            }

            return property.Name;
        }

        public RawJsonActionResult Error<T>(T content, int totalCount = 0)
        {
            return new RawJsonActionResult(new CommonResponseModel<T>(content, totalCount).Serialize());
        }
        public RawJsonActionResult Error2<T>(T content, int totalCount = 0)
        {
            return new RawJsonActionResult(new CommonResponseModel<T>(content, totalCount).Serialize(), HttpStatusCode.BadRequest);
        }

    }
}