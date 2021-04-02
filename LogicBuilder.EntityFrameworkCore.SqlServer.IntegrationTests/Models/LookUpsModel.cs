using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Domain.Entities
{
    public class LookUpsModel : BaseModelClass
    {
        public int LookUpsID { get; set; }

        public string Text { get; set; }

        public string ListName { get; set; }

        public string Value { get; set; }

        public double? NumericValue { get; set; }

        public bool? BooleanValue { get; set; }

        public System.DateTime? DateTimeValue { get; set; }

        public char? CharValue { get; set; }

        public System.Guid? GuidValue { get; set; }

        public System.TimeSpan? TimeSpanValue { get; set; }

        public int Order { get; set; }
    }
}
