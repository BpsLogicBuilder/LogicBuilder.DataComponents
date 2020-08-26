using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class AllOperator : IExpressionPart
    {
        public AllOperator(IExpressionPart operand, IExpressionPart filter)
        {
            Operand = operand;
            Filter = filter;
        }

        public AllOperator(IExpressionPart operand)
        {
            Operand = operand;
        }

        public IExpressionPart Operand { get; }
        public IExpressionPart Filter { get; }

        public Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression)
            => operandExpression.Type.IsIQueryable()
                ? operandExpression.GetAllQueryableCall(GetParameters())
                : operandExpression.GetAllEnumerableCall(GetParameters());

        private Expression[] GetParameters()
                => Filter == null ? new Expression[0] : new Expression[] { Filter.Build() };
    }
}
