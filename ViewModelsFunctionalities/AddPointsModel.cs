using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UniApp.Models;

namespace UniApp.ViewModelsFunctionalities
{
    public class AddPointsModel
    {
        public Enrollment Enrollment { get; set; }

        public int inputExamPoints { get; set; }
        public int inputSeminarPoints { get; set; }
        public int inputProjectPoints { get; set; }
        public int inputAdditionalPoints { get; set; }

        public int inputGrade { get; set; }

        [DataType(DataType.Date)]
        public DateTime inputFinishDate { get; set; }
    }
}
