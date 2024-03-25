using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenwichUniversityMagazine.Models.ViewModel
{
    public class CoordinateVM
    {
        public Faculty faculties { get; set; }
        public Magazines magazines { get; set; }
        public Article articles { get; set; }
        public User user { get; set; }
        public string CurrentMagazineId { get; set; }

        public Resource resource { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> AvailableMagazines { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> AvailableFaculties { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> AvailableArticles { get; set; }
        public List<Article> ListArticle {  get; set; }
        public List<Magazines> ListMagazines { get; set; }
        public List<Faculty> ListFaculty { get; set; }
        public List<Resource> ListResource { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> MyResources { get; set; }
        [ValidateNever]
        public IEnumerable<Comment> MyComments { get; set; }

    }
}
