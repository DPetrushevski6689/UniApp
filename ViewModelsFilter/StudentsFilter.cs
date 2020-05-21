using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniApp.Models;

namespace UniApp.ViewModelsFilter
{
    public class StudentsFilter
    {
        public IList<Student> Students { get; set; }
        public string searchIndeks { get; set; }
        public string searchFirstName { get; set; }
        public string searchLastName { get; set; }
    }
}
