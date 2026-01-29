//using PetCare_system.Attributes;
using PetCare_system.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare_system.Models
{
    public class PaymentForBoard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Payment_boardId { get; set; }

        [Required]
        [ForeignKey("Boarding")]
        [Display(Name = "Boarding Reservation")]
        public int BoardingId { get; set; }
        public virtual Pet_Boarding Boarding { get; set; }

        [Required]
        [Display(Name = "Amount Paid")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal AmountPaid { get; set; }

        [Required]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Bank Type")]
        public string BankType { get; set; }

        [StringLength(100)]
        [Display(Name = "Cardholder Name")]
        public string CardHolderName { get; set; }


        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "CVV is required")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "CVV must be 3  digits.")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be numeric and 3 digits long.")]
        [Display(Name = "CVV")]
        public string CVV { get; set; }

        [Display(Name = "Expiration Date")]
        [DataType(DataType.Date)]
        //[ExpiryDateValidation(ErrorMessage = "Expiration date must not be in the past.")]
        public DateTime? ExpiryDate { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Payment Status")]
        public string Status { get; set; } = "Pending";

        [StringLength(100)]
        [Display(Name = "Transaction ID")]
        public string TransactionId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public void SetAmountFromPackage(string package)
        {
            if (package.Contains("Standard")) AmountPaid = 350;
            else if (package.Contains("Deluxe")) AmountPaid = 500;
            else if (package.Contains("Executive")) AmountPaid = 750;
        }
    }
}








