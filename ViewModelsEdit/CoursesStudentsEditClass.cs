using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniApp.Models;

namespace UniApp.ViewModelsEdit
{
    public class CoursesStudentsEditClass
    {
        public Course Course { get; set; }

        public IEnumerable<int> SelectedStudents { get; set; }

        public IEnumerable<SelectListItem> StudentList { get; set; }
    }
}
