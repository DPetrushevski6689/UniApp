using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UniApp.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Display(Name = "Title")]
        [StringLength(100)]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Credits")]
        [Required]
        public int Credits { get; set; }

        [Display(Name = "Semester")]
        [Required]
        public int Semester { get; set; }

        [StringLength(100)]
        public string Programme { get; set; }

        [StringLength(25)]
        public string EducationLevel { get; set; }

        [ForeignKey("FirstTeacherId")]
        [Display(Name = "First Teacher")]
        public int? FirstTeacherId { get; set; }
        public Teacher FirstTeacher { get; set; }

        [ForeignKey("SecondTeacherId")]
        [Display(Name = "Second Teacher")]
        public int? SecondTeacherId { get; set; }
        public Teacher SecondTeacher { get; set; }


        public ICollection<Enrollment> Students { get; set; } //junction so student => many-to-many
    }
}
