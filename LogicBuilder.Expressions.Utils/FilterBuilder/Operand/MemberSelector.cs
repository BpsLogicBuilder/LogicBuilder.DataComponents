using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Operand
{
    public class MemberSelector : FilterPart
    {
        public MemberSelector(IDictionary<string, ParameterExpression> parameters, string memberFullName, FilterPart sourceOperand) : base(parameters)
        {
            MemberFullName = memberFullName;
            SourceOperand = sourceOperand;
        }

        public string MemberFullName { get; set; }
        public FilterPart SourceOperand { get; set; }
        public string ParameterName { get; set; }

        public override Expression Build() 
            => SourceOperand.Build().MakeSelector(MemberFullName);
    }
}
