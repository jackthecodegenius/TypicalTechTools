using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TypicalTechTools.Models
{
    public class Comment
    {
        [Key]
        [Column("commentId")]
        public int commentId { get; set; }

        [Required]
        [Column("comment_text")]
        public string comment_text { get; set; }

        [Required]
        [ForeignKey(nameof(product_code))]
        [Column("product_code")]
        public string product_code { get; set; }

        [Column("session_id")]
        public string? session_id { get; set; }

        [Column("created_date")]
        public DateTime? created_date { get; set; }

        [Column("userId")]
        public int UserId { get; set; } 
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

    }

    
}
