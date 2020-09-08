using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class FirstOperator : FilterMethodOperatorBase, IExpressionPart
    {
        public FirstOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart filterBody, string filterParameterName) : base(parameters, sourceOperand, filterBody, filterParameterName)
        {
        }

        public FirstOperator(IExpressionPart operand) : base(operand)
        {
        }

        protected override Expression Build(Expression operandExpression)
            => operandExpression.GetFirstCall(GetParameters(operandExpression));
    }
}
