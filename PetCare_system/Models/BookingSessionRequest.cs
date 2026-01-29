using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class BookingSessionRequest
    {
        
            public int PetId { get; set; }
            public int TrainerId { get; set; }
            public string TrainingType { get; set; }
            public string SessionDate { get; set; }
            public string StartTime { get; set; }
        public string PaymentMethod { get; set; }
        public Dictionary<string, string> PaymentData { get; set; }
    }
    }
