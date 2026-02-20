using ColdStoreManagement.BLL.Data;
using ColdStoreManagement.BLL.Models.Chamber;
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Validators
{
    public class UniqueChamberNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (AppDbContext)validationContext.GetService(typeof(AppDbContext));
            var chamberName = (string)value;

            //if (dbContext.MixChambers.Any(c => c.ChamberName == chamberName))
            //{
            //    return new ValidationResult(ErrorMessage ?? "Chamber name already exists.", new[] { validationContext.MemberName });
            //}
            var model = (AddChamberVM)validationContext.ObjectInstance;
            int currentId = model.ChamberId; // 0 if adding, non-zero if editing

            var exists = dbContext.chamber
                .Any(c => c.chambername == chamberName && c.chamberid != currentId);

            if (exists)
            {
                return new ValidationResult("Chamber name already exists.");
            }

            return ValidationResult.Success;
        }
    }
}
