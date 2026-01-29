using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PetCare_system.Models
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Post_Id { get; set; }

        [Required]
        public string Category { get; set; } // "General", "Vet", "Grooming", "Photos"
        public string UserId { get; set; }
        [Required]
        public string Title { get; set; }

        public string Body { get; set; }

        public string AttachmentUrl { get; set; } // image/video/file link

        public string UserName { get; set; }
        // Navigation Property
        public virtual ICollection<Comment> Comments { get; set; }



        //public string UserProfilePic { get; set; }

        //public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
