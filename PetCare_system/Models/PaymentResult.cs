using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class PaymentResult
    {
       
            public bool Success { get; set; }
            public string Status { get; set; }
            public string Reference { get; set; }
            public string Message { get; set; }
        }
    }
