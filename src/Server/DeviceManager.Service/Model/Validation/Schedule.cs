using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service.Model
{
    public class Schedule : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string))
            {
                return new ValidationResult("Schedule value should be a string.");
            }

            var scheduleValidator = (IScheduleParser)validationContext.GetService(typeof(IScheduleParser));

            if (scheduleValidator == null)
            {
                return new ValidationResult("Cannot validate this schedule statement.");
            }

            var result = scheduleValidator.Check((string)value).IsValid;

            if (result == false)
            {
                return new ValidationResult("This schedule statement is not valid.");
            }

            return ValidationResult.Success;
        }
    }
}
