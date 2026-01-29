using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class QuizResult
    {
     
            [Key]
            public int Id { get; set; }

            [Required]
            public int PetId { get; set; }

            [Required]
            public int ModuleId { get; set; }

            [Required]
            [Range(0, 100)]
            public int Score { get; set; }

            [Required]
            public DateTime CompletedAt { get; set; } = DateTime.Now;

            [ForeignKey("PetId")]
            public virtual Pet Pet { get; set; }

            [ForeignKey("ModuleId")]
            public virtual TrainingModule TrainingModule { get; set; }
        }
    }
