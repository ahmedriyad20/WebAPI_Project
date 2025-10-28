using WebAPI.Data.Context;
using WebAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Data.Validators
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Get the DbContext from the service provider
            var context = validationContext.GetService(typeof(UniversityContext)) as UniversityContext;

            if (context == null)
            {
                throw new InvalidOperationException("UniversityContext not found in service provider");
            }

            var studentObj = validationContext.ObjectInstance as Student;
            var ssnProperty = validationContext.ObjectInstance.GetType().GetProperty("SSN");

            // to avoid error when update student without change email
            if (studentObj?.SSN == 0)
            {
                string Email = value as string;

                var student = context.Students.FirstOrDefault(s => s.Email == Email);
                if (student != null)
                {
                    return new ValidationResult("Email Already Exist");
                }
            }
             

            return ValidationResult.Success;
        }
    }
}
