using LogicBuilder.Attributes;
using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.DataSource
{
    public class Filter
    {
        public Filter
            (
                [ParameterEditorControl(ParameterControlType.ParameterSourcedPropertyInput)]
                [NameValue(AttributeNames.PROPERTYSOURCEPARAMETER, "ValueSourceType")]
                [Comments("Update ValueSourceType with System.Type's fullName property.")]
                string Field,

                [Domain("eq, neq, lt, lte, gt, gte, contains, doesnotcontain, startswith, endswith, isnotempty, isempty, isnotnull, isnull")]
                [ParameterEditorControl(ParameterControlType.DropDown)]
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
