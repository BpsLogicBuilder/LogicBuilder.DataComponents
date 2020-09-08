using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class LastOperator : FilterMethodOperatorBase, IExpressionPart
    {
        public LastOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart filterBody, string filterParameterName) : base(parameters, sourceOperand, filterBody, filterParameterName)
        {
        }

        public LastOperator(IExpressionPart operand) : base(operand)
        {
        }

        protected override Expression Build(Expression operandExpression)
            => operandExpression.GetLastCall(GetParameters(operandExpression));
    }
}
