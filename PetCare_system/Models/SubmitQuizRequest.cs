using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class SubmitQuizRequest
    {
       
       
            public int PetId { get; set; }
            public int ModuleId { get; set; }
            public List<QuizAnswer> Answers { get; set; }
        }
    public class QuizSubmission
    {
        public int PetId { get; set; }
        public int ModuleId { get; set; }
        public List<QuizAnswer> Answers { get; set; }
    }

   
    public class QuizAnswer
        {
            public int QuestionId { get; set; }
            public int SelectedOptionId { get; set; }
        }
    }
