using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Contoso.Domain.Entities
{
    public class CourseModel : BaseModelClass
    {
		[Display(Name = "Number")]
		public int CourseID { get; set; }

		[StringLength(50, MinimumLength = 3)]
		public string Title { get; set; }

		[Range(0, 5)]
		public int Credits { get; set; }

		public int DepartmentID { get; set; }

        public string DepartmentName { get; set; }
        //[AlsoKnownAs("Course.Department")]
        //public DepartmentModel Department { get; set; }

        //[AlsoKnownAs("Course.Enrollments")]
        //public ICollection<EnrollmentModel> Enrollments { get; set; }

		public ICollection<CourseAssignmentModel> Assignments { get; set; }
    }
}