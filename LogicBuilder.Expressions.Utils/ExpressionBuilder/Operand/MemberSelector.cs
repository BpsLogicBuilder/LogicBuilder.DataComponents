using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Operand
{
    public class MemberSelector : IExpressionPart
    {
        public MemberSelector(string memberFullName, IExpressionPart sourceOperand)
        {
            MemberFullName = memberFullName;
            SourceOperand = sourceOperand;
        }

        public string MemberFullName { get; set; }
        public IExpressionPart SourceOperand { get; set; }
        public string ParameterName { get; set; }

        public Expression Build() 
            => SourceOperand.Build().MakeSelector(MemberFullName);
    }
}
