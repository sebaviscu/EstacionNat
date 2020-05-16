using MVCBlog.Core.Entities;
using MVCBlog.Core.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCBlog.Website.Models.OutputModels.Society
{
    public class SMFPViewModels
    {
        [MaxLength(10, ErrorMessage = "*")]
        [Display(Name = nameof(Labels.UserName), ResourceType = typeof(Labels))]
        public string UserName { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = nameof(Labels.FirstSemester), ResourceType = typeof(Labels))]
        public bool FirstSemester { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(2000, 9999, ErrorMessage = "*")]
        [Display(Name = nameof(Labels.Year), ResourceType = typeof(Labels))]
        public int Year { get; set; }
    }
}