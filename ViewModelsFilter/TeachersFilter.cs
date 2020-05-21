using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniApp.Models;

namespace UniApp.ViewModelsFilter
{
    public class TeachersFilter
    {
        public IList<Teacher> Teachers { get; set; }
        public string searchFirstName { get; set; }
        public string searchLastName { get; set; }
        public string searchDegree { get; set; }
        public string searchAcademicRank { get; set; }
    }
}
