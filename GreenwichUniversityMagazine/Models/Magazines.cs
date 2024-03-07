using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenwichUniversityMagazine.Models
{
    public class Magazines
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        [ValidateNever]
        public Faculty Faculty { get; set; }

        public int TermId { get; set; }
        [ForeignKey("TermId")]
        [ValidateNever]
        public Term Term { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
