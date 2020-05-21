using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UniApp.Models;

namespace UniApp.ViewModelsFunctionalities
{
    public class CoursesUnrollStudentsClass
    {
        public Course Course { get; set; }

        [DataType(DataType.Date)]
        public DateTime FinishDate { get; set; }

        public IEnumerable<int> SelectedStudents { get; set; }
        public IEnumerable<SelectListItem> StudentList { get; set; }
    }
}
