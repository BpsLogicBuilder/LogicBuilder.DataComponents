using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class AnyOperator : IExpressionPart
    {
        public AnyOperator(IExpressionPart operand, IExpressionPart filter)
        {
            Operand = operand;
            Filter = filter;
        }

        public AnyOperator(IExpressionPart operand)
        {
            Operand = operand;
        }

        public IExpressionPart Operand { get; }
        public IExpressionPart Filter { get; }

        public Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression) 
            => operandExpression.Type.IsIQueryable()
                ? operandExpression.GetAnyQueryableCall(GetParameters())
                : operandExpression.GetAnyEnumerableCall(GetParameters());

        private Expression[] GetParameters()
                => Filter == null ? new Expression[0] : new Expression[] { Filter.Build() };
    }
}
