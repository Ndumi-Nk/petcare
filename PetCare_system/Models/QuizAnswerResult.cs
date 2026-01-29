using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class QuizAnswerResult
    {
            public int QuestionId { get; set; }
            public string QuestionText { get; set; }
            public int SelectedOptionId { get; set; }
            public string SelectedOptionText { get; set; }
            public bool IsCorrect { get; set; }
            public int CorrectOptionId { get; set; }
            public string CorrectOptionText { get; set; }
            public string Explanation { get; set; }
        }
    }
