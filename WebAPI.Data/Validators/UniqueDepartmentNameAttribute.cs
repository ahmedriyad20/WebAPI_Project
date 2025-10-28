using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Data.Context;
using WebAPI.Data.Models;

namespace WebAPI.Data.Validators
{
    internal class UniqueDepartmentNameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Get the DbContext from the service provider
            var context = validationContext.GetService(typeof(UniversityContext)) as UniversityContext;

            if (context == null)
            {
                throw new InvalidOperationException("UniversityContext not found in service provider");
            }

            var departmentObj = validationContext.ObjectInstance as Department;

            //to make sure we are in add operation not update
            if (departmentObj?.Id == 0)
            {
                string? Name = value as string;

                var department = context.Departments.FirstOrDefault(d => d.Name == Name);

                if (department != null)
                {
                    return new ValidationResult("Department Already Exist");
                }
            }

            return ValidationResult.Success;
        }
    }
}

