using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class QuizOption
    {
      

        
            [Key]
            public int Id { get; set; }

            [Required]
            [StringLength(500)]
            [Display(Name = "Option Text")]
            public string OptionText { get; set; }

            [Required]
            [Display(Name = "Is Correct")]
            public bool IsCorrect { get; set; }

            [Required]
            public int QuestionId { get; set; }

            [ForeignKey("QuestionId")]
            public virtual QuizQuestion Question { get; set; }
        }
    }
