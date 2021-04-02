using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Contoso.Domain.Entities
{
    public class StudentModel : BaseModelClass
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
		[Display(Name = "Enrollment Date")]
		public System.DateTime EnrollmentDate { get; set; }

		public ICollection<EnrollmentModel> Enrollments { get; set; }
    }
}