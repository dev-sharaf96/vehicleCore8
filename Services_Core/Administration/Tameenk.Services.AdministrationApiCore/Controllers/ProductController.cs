using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tameenk.Api.Core;
using Tameenk.Api.Core.Models;
using Tameenk.Services.AdministrationApi.Models;
using Tameenk.Services.Core.Products;
using Tameenk.Services.AdministrationApi.Extensions;
using Tameenk.Api.Core.Context;
using Tameenk.Core.Exceptions;
using Tameenk.Core.Domain.Enums;
using Tameenk.Services.Administration.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.AdministrationApi.Controllers
{
    /// <summary>
    /// product Controller
    /// </summary>
    [AdminAuthorizeAttribute(pageNumber: 0)]
    public class ProductController : AdminBaseApiController
    {


        #region Fields
        private readonly IProductService _productService;
        private IWebApiContext _webApiContext;
        #endregion


        #region Ctor
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="productService">The product Service.</param>
        /// <param name="webApiContext">Web api Context</param> 
        public ProductController(IProductService productService, IWebApiContext webApiContext)
        {
            _productService = productService;
            _webApiContext = webApiContext ?? throw new TameenkArgumentNullException(nameof(IWebApiContext));
        }

        #endregion


        #region methods
        /// <summary>
        /// get all products type
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/product-type/all")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CommonResponseModel<IEnumerable<IdNamePairModel>>))]
        public IActionResult GetAllProductsType()
        {

            try
            {
                var language = _webApiContext.CurrentLanguage;
                var result = _productService.GetProductTypes();
                
                //then convert to model
                var dataModel = result.Select(e => new IdNamePairModel() {
                    Id = e.Code,
                    Name = (language == LanguageTwoLetterIsoCode.En) ? e.EnglishDescription : e.ArabicDescription
                });

                return Ok(dataModel, result.Count());
            }
            catch (Exception ex)
            {
                return Error("an error has occured");
            }
            

        }


        #endregion
    }
}
