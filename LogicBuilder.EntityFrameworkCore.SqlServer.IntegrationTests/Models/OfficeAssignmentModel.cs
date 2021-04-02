using System.ComponentModel.DataAnnotations;


namespace Contoso.Domain.Entities
{
    public class OfficeAssignmentModel : BaseModelClass
    {
		public int InstructorID { get; set; }

		[StringLength(50)]
		[Display(Name = "Office Location")]
		public string Location { get; set; }

		//[AlsoKnownAs("OfficeAssignment.Instructor")]
		//public InstructorModel Instructor { get; set; }
    }
}