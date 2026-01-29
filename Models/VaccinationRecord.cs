using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare_system.Models
{
    public class VaccinationRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VaccinationId { get; set; }

        [Required]
        [Display(Name = "Vaccine Name")]
        public string VaccineName { get; set; }

        [Required]
        [Display(Name = "Date Given")]
        [DataType(DataType.Date)]
        public DateTime DateGiven { get; set; }

        [Display(Name = "Next Due Date")]
        [DataType(DataType.Date)]
        public DateTime? NextDueDate { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Required]
        [Display(Name = "Pet")]
        public int PetId { get; set; }

        public virtual Pet Pet { get; set; }

        // ======== ANALYTICS PROPERTIES ========

        [Display(Name = "Is Overdue")]
        public bool IsOverdue
        {
            get
            {
                return NextDueDate.HasValue && NextDueDate.Value.Date < DateTime.Today;
            }
        }

        [Display(Name = "Days Until Next Dose")]
        public int? DaysUntilNextDose
        {
            get
            {
                return NextDueDate.HasValue ? (NextDueDate.Value.Date - DateTime.Today).Days : (int?)null;
            }
        }

        [Display(Name = "Month Given")]
        public string MonthGiven
        {
            get
            {
                return DateGiven.ToString("yyyy-MM");
            }
        }

        [Display(Name = "Vaccination Complete")]
        public bool IsComplete
        {
            get
            {
                return !NextDueDate.HasValue;
            }
        }

        [Display(Name = "Treatment Type")]
        public string VaccineType { get; set; } // Optional for classification
    }


}
