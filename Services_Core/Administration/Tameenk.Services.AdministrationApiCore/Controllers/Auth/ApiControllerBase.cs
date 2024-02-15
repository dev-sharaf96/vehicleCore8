using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tameenk.Api.Core.ActionResults;
using Tameenk.Api.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected IActionResult Single<T>(T entity)
        {
           // if (entity == null) return NotFound();
            return Ok(entity);
        }

        protected IActionResult Collection<T>(List<T> list)
        {
           // if (list.Count() == 0) return NotFound();
            return Ok(list);
        }

        protected IActionResult Result<T>(Task<int> changes)
        {
           // if (changes.Result == 0) return BadRequest();
            return Ok();
        }
        [NonAction]
        public RawJsonActionResult Error(IEnumerable<ErrorModel> errors, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            var commonModel = new CommonResponseModel<bool>(false);
            commonModel.Errors = errors;
            return new RawJsonActionResult(commonModel.Serialize(), httpStatusCode);
        }


        //[NonAction]
        //public RawJsonActionResult Error(ModelStateDictionary modelState, string errorMessage, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        //{
        //    TransalateModelStatePropertyName(modelState, ActionContext);
        //    var commonModel = new CommonResponseModel<ModelStateDictionary>(modelState);
        //    commonModel.Errors = new List<ErrorModel>() { new ErrorModel(errorMessage) };
        //    return new RawJsonActionResult(commonModel.Serialize(), httpStatusCode);
        //}
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


        //[NonAction]
        //public RawJsonActionResult Error(Exception ex)
        //{
        //    var logKey = $"api_{DateTime.Now.GetTimestamp()}";
        //    Logger.Log($"Api error [Key={logKey}]", ex, LogLevel.Error);
        //    var error = new ErrorModel
        //    {
        //        Code = logKey,
        //        Description = ex.GetBaseException().Message
        //    };
        //    return Error(new List<ErrorModel> { error });
        //}


        //[NonAction]
        //private void TransalateModelStatePropertyName(ModelStateDictionary modelState, HttpActionContext actionContext)
        //{
        //    var descriptor = actionContext.ActionDescriptor;

        //    if (descriptor != null)
        //    {
        //        var parameters = descriptor.GetParameters();

        //        var subParameterIssues = modelState.Keys
        //                                           .Where(s => s.Contains("."))
        //                                           .Where(s => modelState[s].Errors.Any())
        //                                           .GroupBy(s => s.Substring(0, s.IndexOf('.')))
        //                                           .ToDictionary(g => g.Key, g => g.ToArray());

        //        foreach (var parameter in parameters)
        //        {
        //            var argument = actionContext.ActionArguments[parameter.ParameterName];

        //            if (subParameterIssues.ContainsKey(parameter.ParameterName))
        //            {
        //                var subProperties = subParameterIssues[parameter.ParameterName];
        //                foreach (var subProperty in subProperties)
        //                {
        //                    var propName = subProperty.Substring(subProperty.IndexOf('.') + 1);
        //                    var propertyName = $"{subProperty.Split('.')[0]}.{GetPropertyName(propName, parameter.ParameterType)}";
        //                    if (!string.IsNullOrWhiteSpace(propertyName) && modelState[subProperty] != null)
        //                    {
        //                        if (modelState[propertyName] != null
        //                            && !subProperty.Equals(propertyName, StringComparison.Ordinal))
        //                        {
        //                            var modelErrors = modelState[subProperty].Errors.ToList();

        //                            modelState.Remove(subProperty);
        //                            // in case the name was not equal due to case senstivity
        //                            // the remove will remove modelState[propertyName]
        //                            if (modelState[propertyName] == null)
        //                                modelState.Add(propertyName, new ModelState());

        //                            foreach (var error in modelErrors)
        //                            {
        //                                modelState[propertyName].Errors.Add(error);
        //                            }
        //                        }
        //                        else if (modelState[propertyName] == null)
        //                        {
        //                            modelState.Add(propertyName, modelState[subProperty]);
        //                            modelState.Remove(subProperty);
        //                        }
        //                    }
        //                }
        //            }


        //        }
        //    }

        //}

    }
}