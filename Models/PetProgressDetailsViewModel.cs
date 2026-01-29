using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class PetProgressDetailsViewModel
    {
        public PetProgress Progress { get; set; }
        public List<WatchedVideo> VideosWatched { get; set; }
        public List<QuizResult> QuizzesCompleted { get; set; }
        public List<CompletedModule> CompletedModules { get; set; }
    }
}