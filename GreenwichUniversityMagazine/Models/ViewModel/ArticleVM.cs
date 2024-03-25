using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenwichUniversityMagazine.Models.ViewModel
{
    public class ArticleVM
    {
        public Article article { get; set; }
        [ValidateNever]
        public string? status { get; set; }
        [ValidateNever]
        public Magazines Magazines { get; set; }
        public List<SelectListItem> MonthYearOptions { get; set; }
        public User? User { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyUsers { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyMagazines { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyResources { get; set; }

        public string FormattedModifyDate { get; set; }
        [ValidateNever]
        public IEnumerable<Comment> MyComments { get; set; }
    }
}
