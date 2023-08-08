using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.DataSource
{
    [System.Obsolete("No longer used. Use LogicBuilder.Expressions.Utils.ExpressionBuilder.")]
    public class Filter
    {
        public Filter
            (
                string Field,
                string Oper,
                string Value = null,
                string ValueSourceMember = null,
                string ValueSourceType = null
            )
        {
            this.Field = Field;
            this.Operator = Oper;
            this.Value = Value;
            this.ValueSourceMember = ValueSourceMember;
            this.ValueSourceType = ValueSourceType;
        }

        public Filter() { }

        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public string ValueSourceType { get; set; }
        public string ValueSourceMember { get; set; }
        public string Logic { get; set; }
        public IEnumerable<Filter> Filters { get; set; }
    }
}
