using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Contoso.Domain.Entities
{
    public class InstructorModel : BaseModelClass
    {
		public int ID { get; set; }

		[Required]
		[StringLength(50)]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required]
		[StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

        public string FullName { get; set; }

        [DataType(DataType.Date)]
		[DisplayFormat(DataFormatString  = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Display(Name = "Hire Date")]
		public System.DateTime HireDate { get; set; }

		public ICollection<CourseAssignmentModel> Courses { get; set; }

		public OfficeAssignmentModel OfficeAssignment { get; set; }
    }
}