using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tamkeen.bll.Utils
{
    /// <summary>
    /// Validation Method .. 
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// TODO Edit XML Comment Template for Validate
    [AttributeUsage(AttributeTargets.Method)]
    public class Validate : System.Attribute
    {
        public bool TryValidate(object @object, out List<ValidationResult> results)
        {
            var context = new ValidationContext(@object, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(
                @object, context, results,
                validateAllProperties: true
            );
        }


    }
}
