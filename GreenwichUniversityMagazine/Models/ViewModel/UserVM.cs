using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace GreenwichUniversityMagazine.Models.ViewModels
{
    public class UserVM
    {
        public User? User { get; set; }

        public Faculty? Faculty { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyUsers { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyFaculties { get; set; }
        public int NumberOfUsers { get; set; }
    }
}