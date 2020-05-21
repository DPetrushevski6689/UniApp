using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UniApp.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50)]
        [Required]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Degree { get; set; }

        [StringLength(25)]
        public string AcademicRank { get; set; }

        [StringLength(10)]
        public string OfficeNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        public string ProfilePicture { get; set; }

        //[InverseProperty("FirstTeacher")]
        public ICollection<Course> FirstCourses { get; set; } //eden nastavnik so povekje predmeti

        //[InverseProperty("SecondTeacher")]
        public ICollection<Course> SecondCourses { get; set; } //eden nastavnik so povekje predmeti
    }

}
