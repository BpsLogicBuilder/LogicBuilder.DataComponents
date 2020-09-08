using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class SingleOrDefaultOperator : FilterMethodOperatorBase, IExpressionPart
    {
        public SingleOrDefaultOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart filterBody, string filterParameterName) : base(parameters, sourceOperand, filterBody, filterParameterName)
        {
        }

        public SingleOrDefaultOperator(IExpressionPart operand) : base(operand)
        {
        }

        protected override Expression Build(Expression operandExpression)
            => operandExpression.GetSingleOrDefaultCall(GetParameters(operandExpression));
    }
}
