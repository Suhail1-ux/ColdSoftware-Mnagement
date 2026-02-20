using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models.Bank
{
    public class AddTransactionRequest : IValidatableObject
    {
        [Required(ErrorMessage = "Credit Account is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Credit Account")]
        public int CreditGroupId { get; set; }

        [Required(ErrorMessage = "Debit Account is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Debit Account")]
        public int DebitGroupId { get; set; }

        [Required(ErrorMessage = "Transaction type is required")]
        [StringLength(50, ErrorMessage = "Transaction type is too long")]
        public string TransactionType { get; set; } = string.Empty;

        // Optional – default empty instead of null
        [StringLength(50)]
        public string PaymentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Transaction date is required")]
        public DateTime TransactionDate { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [StringLength(50)]
        public string? ChequeNo { get; set; }

        [StringLength(100)]
        public string? ReferenceNo { get; set; }

        [StringLength(250)]
        public string? Remarks { get; set; }

        // Custom cross-field validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CreditGroupId == DebitGroupId)
            {
                yield return new ValidationResult(
                    "Credit and Debit Account cannot be the same.",
                    new[] { nameof(CreditGroupId), nameof(DebitGroupId) }
                );
            }
        }
    }

    public class UpdateTransactionRequest : AddTransactionRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Transaction Id")]
        public int TransactionId { get; set; }
    }

}
