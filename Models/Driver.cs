using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;





namespace PetCare_system.Models
{
    public class Driver
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DriverId { get; set; }
        [Required]
        public string DriverName { get; set; }
        [Required]
        public string DriverType { get; set; }
        public string DriverSurname { get; set; }
        [Required]
        public string Username { get; set; }
        [NotMapped]
        [Required]
        public string Password { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string License { get; set; }
        
        public string CarInfo { get; set; }
        
        public string Destination { get; set; }
       
        public string Driverstatus { get; set; }
       
        public string DeliveryStatus { get; set; }

        public string ProductId { get; set; }
        public string Userr_Id { get; set; }
        public string Pet_Id { get; set; }
        public string RequestId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        

        public string PasswordHash { get; set; } // Store hashed password
    }
}