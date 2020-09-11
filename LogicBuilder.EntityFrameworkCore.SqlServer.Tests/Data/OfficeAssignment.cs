using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Contoso.Data.Entities
{
    [Table("OfficeAssignment")]
    public class OfficeAssignment : BaseDataClass
    {
        [Key]
        [ForeignKey("Instructor")]
        public int InstructorID { get; set; }
        [StringLength(50)]
        [Display(Name = "Office Location")]
        public string Location { get; set; }

        
        public virtual Instructor Instructor { get; set; }
    }
}
