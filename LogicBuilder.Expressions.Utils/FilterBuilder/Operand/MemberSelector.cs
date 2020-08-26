using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Operand
{
    public class MemberSelector : FilterPart
    {
        public MemberSelector(string memberFullName, FilterPart sourceOperand)
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
