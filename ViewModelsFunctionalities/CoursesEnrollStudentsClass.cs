using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniApp.Models;

namespace UniApp.ViewModelsFunctionalities
{
    public class CoursesEnrollStudentsClass
    {
        public Course Course { get; set; }

        public int Year { get; set; }

        public string Semester { get; set; }

        public IEnumerable<int> SelectedStudents { get; set; }
        public IEnumerable<SelectListItem> StudentList { get; set; }
    }
}
