using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniApp.Models;

namespace UniApp.ViewModelsFunctionalities
{
    public class CourseListModel
    {
        public IList<Enrollment> Enrollments { get; set; }

        //public int Year { get; set; }
    }
}
