using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenwichUniversityMagazine.Models.ViewModel
{
    public class TermVM
    {
        public Term Terms { get; set; }
        [ValidateNever]
        public IEnumerable<SelectList> TermsList { get; set; }
        public string Season { get; set; }
        public int Year { get; set; }
    }
}
