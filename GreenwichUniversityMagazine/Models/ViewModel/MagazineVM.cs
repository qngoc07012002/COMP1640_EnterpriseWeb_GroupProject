using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace GreenwichUniversityMagazine.Models.ViewModels
{
    public class MagazineVM
    {
        public Magazines? Magazines { get; set; }
        public Faculty? Faculty { get; set; }

        public Term? Term { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyMagazines { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyFaculties { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyTerms { get; set; }
    }
}