using ColdStoreManagement.BLL.Data;
using ColdStoreManagement.BLL.Models.Chamber;
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Validators
{
    public class QuantityCheckAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (AppDbContext)validationContext.GetService(typeof(AppDbContext));
            int editedQiantity = Convert.ToInt32(value);

            var model = (AddChamberVM)validationContext.ObjectInstance;
            int currentId = model.ChamberId; // 0 if adding, non-zero if editing

            var checkQuantityConsumed = dbContext.chamber
                .FirstOrDefault(c => c.chamberid == currentId)?.QuantityConsumed;

            if (checkQuantityConsumed > editedQiantity)
            {
                return new ValidationResult($"Chamber capacity cannot be less than quantity already consumed : {checkQuantityConsumed} ");
            }

            return ValidationResult.Success;
        }
    }
}
