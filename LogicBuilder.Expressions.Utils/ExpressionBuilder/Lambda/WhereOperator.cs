using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class WhereOperator : IExpressionPart
    {
        public WhereOperator(IExpressionPart operand, IExpressionPart filter)
        {
            Operand = operand;
            Filter = filter;
        }

        public IExpressionPart Operand { get; }
        public IExpressionPart Filter { get; }

        public Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression)
            => operandExpression.GetWhereCall(Filter.Build());
    }
}
