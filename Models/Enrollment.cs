using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UniApp.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [StringLength(10)]
        public string Semester { get; set; }

        public int? Year { get; set; }

        public int? Grade { get; set; }

        [StringLength(255)]
        public string SeminarUrl { get; set; }

        [StringLength(255)]
        public string ProjectUrl { get; set; }

        public int? ExamPoints { get; set; }

        public int? SeminarPoints { get; set; }

        public int? ProjectPoints { get; set; }

        public int? AdditionalPoints { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FinishDate { get; set; }
    }
}
