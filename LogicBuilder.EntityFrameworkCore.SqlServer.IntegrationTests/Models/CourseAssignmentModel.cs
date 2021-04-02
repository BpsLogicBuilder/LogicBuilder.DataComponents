using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Contoso.Domain.Entities
{
    public class CourseAssignmentModel : BaseModelClass
    {
		public int InstructorID { get; set; }

		public int CourseID { get; set; }

        public string CourseTitle { get; set; }

        public string CourseNumberAndTitle { get; set; }

        public string Department { get; set; }
        //[AlsoKnownAs("CourseAssignment.Instructor")]
        //public InstructorModel Instructor { get; set; }

        //[AlsoKnownAs("CourseAssignment.Course")]
        //public CourseModel Course { get; set; }
    }
}