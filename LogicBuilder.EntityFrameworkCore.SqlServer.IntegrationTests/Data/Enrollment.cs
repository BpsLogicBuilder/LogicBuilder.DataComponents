using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Contoso.Data.Entities
{
    [Table("Enrollment")]
    public class Enrollment : BaseDataClass
    {
        [Key]
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }

        [ForeignKey("CourseID")]
        public Course Course { get; set; }
        [ForeignKey("StudentID")]
        public Student Student { get; set; }
    }

    public enum Grade
    {
        A, B, C, D, F
    }
}
