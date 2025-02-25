using System.ComponentModel.DataAnnotations;

namespace BlazorAppIdentityDotNet.Data.Models
{
    public class AgeRangeValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime birthDate)
            {
                int age = DateTime.Today.Year - birthDate.Year;
                if (birthDate > DateTime.Today.AddYears(-age)) // Adjust for birthdate after today’s date
                    age--;

                return age >= 18 && age <= 70;
            }
            return false;
        }
    }
}
