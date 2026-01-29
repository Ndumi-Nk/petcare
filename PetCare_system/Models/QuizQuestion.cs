using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class QuizQuestion
    {
       
            [Key]
            public int Id { get; set; }

            [Required]
            [StringLength(500)]
            [Display(Name = "Question Text")]
            public string QuestionText { get; set; }

            [StringLength(1000)]
            public string Explanation { get; set; }

            [Required]
            public int TrainingModuleId { get; set; }

            [ForeignKey("TrainingModuleId")]
            public virtual TrainingModule TrainingModule { get; set; }

            public virtual ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();
        }
    }

