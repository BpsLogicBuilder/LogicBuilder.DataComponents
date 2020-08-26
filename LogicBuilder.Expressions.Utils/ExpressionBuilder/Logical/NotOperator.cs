using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical
{
    public class NotOperator : IExpressionPart
    {
        public NotOperator(IExpressionPart operand)
        {
            this.Operand = operand;
        }

        public IExpressionPart Operand { get; private set; }

        public Expression Build() 
            => Expression.Not(this.Operand.Build());
    }
}
