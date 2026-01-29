using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare_system.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }
        public string UserId { get; set; }
        [Required]
        public string Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserName { get; set; }

        // Foreign key to Post
        public int Post_Id { get; set; }

        [ForeignKey("Post_Id")]
        public virtual Post Post { get; set; }
    }


}