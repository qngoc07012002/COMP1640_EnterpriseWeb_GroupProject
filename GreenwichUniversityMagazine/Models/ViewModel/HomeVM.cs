using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenwichUniversityMagazine.Models.ViewModel
{
    public class HomeVM 
    {
        public List<Term> Terms { get; set; }
        public List<Article> Articles { get; set; }
        public List<Magazines> Magazines { get; set; }
       
        public List<Faculty> Facultys { get; set;}

       
    }
}
