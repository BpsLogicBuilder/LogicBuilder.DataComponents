using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Contoso.Domain.Entities
{
    public class EnrollmentModel : BaseModelClass
    {
		public int EnrollmentID { get; set; }

		public int CourseID { get; set; }

		public int StudentID { get; set; }

		[DisplayFormat(NullDisplayText = "No grade")]
		public Grade? Grade { get; set; }

        public string GradeLetter { get; set; }

        public string CourseTitle { get; set; }

        public string StudentName { get; set; }

        //[AlsoKnownAs("Enrollment.Course")]
        //public CourseModel Course { get; set; }

        //[AlsoKnownAs("Enrollment.Student")]
        //public StudentModel Student { get; set; }
    }
}