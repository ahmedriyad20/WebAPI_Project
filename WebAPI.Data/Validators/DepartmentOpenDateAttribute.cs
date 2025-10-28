using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Data.Context;

namespace WebAPI.Data.Validators
{
    public class DepartmentOpenDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Get the DbContext from the service provider
            var context = validationContext.GetService(typeof(UniversityContext)) as UniversityContext;

            if (context == null)
            {
                throw new InvalidOperationException("UniversityContext not found in service provider");
            }

            DateTime? OpenDate = value as DateTime?;

            if(OpenDate != null && OpenDate > DateTime.Now)
            {
                return new ValidationResult("Department Open Date cannot be in the future");
            }

            return ValidationResult.Success;
        }
    }
}
