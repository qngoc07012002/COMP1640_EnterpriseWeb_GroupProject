using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenwichUniversityMagazine.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public User User { get; set; }

        public int ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        [ValidateNever]
        public Article Article { get; set; }

        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime Date {  get; set; }
    }
}
