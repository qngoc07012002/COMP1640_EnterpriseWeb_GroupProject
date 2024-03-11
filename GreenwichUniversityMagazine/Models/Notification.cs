using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GreenwichUniversityMagazine.Models
{
    public class Notification
    {
        [Key]
        public int id { get; set; }

        public string description {  get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public User User { get; set; }

        public int ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        [ValidateNever]
        public Article Article { get; set; }

    }
}
