using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Contoso.Domain.Entities
{
    public class DepartmentModel : BaseModelClass
    {
		public int DepartmentID { get; set; }

		[StringLength(50, MinimumLength = 3)]
		public string Name { get; set; }

		[DataType(DataType.Currency)]
		public decimal Budget { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString  = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Display(Name = "Start Date")]
		public System.DateTime StartDate { get; set; }

		public int? InstructorID { get; set; }

		public byte[] RowVersion { get; set; }

        public string AdministratorName { get; set; }
        //[AlsoKnownAs("Department.Administrator")]
        //public InstructorModel Administrator { get; set; }

		public ICollection<CourseModel> Courses { get; set; }
    }
}