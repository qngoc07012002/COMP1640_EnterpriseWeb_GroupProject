using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenwichUniversityMagazine.Models
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }
        
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public User? User { get; set; }

        public int? MagazinedId { get; set; }
        [ForeignKey("MagazinedId")]
        [ValidateNever]
        public Magazines? Magazines { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string? imgUrl { get; set; }
        public string Body { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool Status { get; set; }
    }
}
