using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenwichUniversityMagazine.Models.ViewModel
{
    public class ViewVM : Controller
    {
        
        
        public List<Article> ListArticle { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyMagazines { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyFaculties { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyTerms { get; set; }

        
    }
}
