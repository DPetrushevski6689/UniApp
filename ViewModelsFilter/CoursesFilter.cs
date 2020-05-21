using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniApp.Models;

namespace UniApp.ViewModelsFilter
{
    public class CoursesFilter
    {
        public IList<Course> Courses { get; set; }

        public string searchTitle { get; set; }

        public string searchProgramme { get; set; }
    }
}
