using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UniApp.Models
{
    public class Student
    {
        public int Id { get; set; } //ne e long bidejki ima kolizija so foreign key vo enrollment tabelata

        [Display(Name = "Student ID")]
        [StringLength(10)]
        [Required]
        public string StudentId { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)]
        [Required]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; }

        public int AcquiredCredits { get; set; }

        public int CurrentSemestar { get; set; }

        [StringLength(25)]
        public string EducationLevel { get; set; }

        public string ProfilePicture { get; set; }

        public ICollection<Enrollment> Courses { get; set; } //many-to-many so courses => junction table enrollment
    }
}
