﻿using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenwichUniversityMagazine.Models.ViewModel
{
    public class ArticleVM
    {
        public Article article { get; set; }
        [ValidateNever]
        public Magazines Magazines { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyMagazines { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> MyResources { get; set; }
    }
}