using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetCare_system.Models
{

    public class PaymentProd
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentProdId { get; set; }

        [Required(ErrorMessage = "Billing address is required.")]
        [StringLength(250, ErrorMessage = "Billing address cannot exceed 250 characters.")]
        public string BillingAddress { get; set; }

        [Required(ErrorMessage = "Card number is required.")]
        [CreditCard(ErrorMessage = "Invalid credit card number.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiry date is required.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Expiry date must be in MM/YY format.")]
        public string ExpiryDate { get; set; }

        [Required(ErrorMessage = "CVV is required.")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "CVV must be 3 or 4 digits.")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be numeric.")]
        public string CVV { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters.")]


        public string PaymentMethod { get; set; }


        public bool IsDelivery { get; set; } // Indicates if delivery is selected
        public string DeliveryAddress { get; set; } // Delivery address if IsDelivery is true
        public decimal Amount { get; set; }


        public DateTime CreatedAt { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string CartItems { get; set; } = "";

    }
}