using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenwichUniversityMagazine.Models.ViewModel
{
    public class ArticleVM
    {
        public List<Term>? Terms { get; set; }
        public List<Article>? Articles { get; set; }
        public List<Magazines>? Magazine { get; set; }
        public List<Faculty>? Facultys { get; set; }
        public Article article { get; set; }
        [ValidateNever]
        public string? status { get; set; }
        [ValidateNever]
        public Magazines Magazines { get; set; }
        public User? User { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyUsers { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyMagazines { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyResources { get; set; }

        public string? FormattedModifyDate { get; set; }
        [ValidateNever]
        public IEnumerable<Comment> MyComments { get; set; }
        [ValidateNever]
        public List<User>? CommentUsers { get; set; }

    }
}
