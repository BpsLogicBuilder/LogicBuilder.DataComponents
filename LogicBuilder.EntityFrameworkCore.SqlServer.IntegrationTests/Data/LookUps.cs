using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Contoso.Data.Entities
{
    [Table("LookUps")]
    public class LookUps : BaseDataClass
    {
        [Key]
        public int LookUpsID { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Text { get; set; }

        [MaxLength(100)]
        [Required]
        public string ListName { get; set; }

        [MaxLength(256)]
        public string Value { get; set; }

        public double? NumericValue { get; set; }

        public bool? BooleanValue { get; set; }

        public DateTime? DateTimeValue { get; set; }

        public char? CharValue { get; set; }

        public Guid? GuidValue { get; set; }

        public TimeSpan? TimeSpanValue { get; set; }

        public int Order { get; set; }
    }
}
