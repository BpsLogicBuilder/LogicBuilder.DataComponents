using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class CountOperator : FilterMethodOperatorBase, IExpressionPart
    {
        public CountOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart filterBody, string filterParameterName) : base(parameters, sourceOperand, filterBody, filterParameterName)
        {
        }

        public CountOperator(IExpressionPart operand) : base(operand)
        {
        }

        protected override Expression Build(Expression operandExpression)
            => operandExpression.GetCountCall(GetParameters(operandExpression));
    }
}
