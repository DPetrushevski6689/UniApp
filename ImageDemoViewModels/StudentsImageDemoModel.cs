﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UniApp.Models;

namespace UniApp.ImageDemoViewModels
{
    public class StudentsImageDemoModel
    {
        public Student Student { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile ProfileImage { get; set; }
    }
}
