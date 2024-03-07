using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenwichUniversityMagazine.Models
{
    public class Resource
    {
        [Key]
        public int Id { get; set; }
        public int ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        [ValidateNever]
        public Article Article { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
    }
}
