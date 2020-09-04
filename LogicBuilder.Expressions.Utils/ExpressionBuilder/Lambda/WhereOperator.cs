using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class WhereOperator : FilterMethodOperatorBase, IExpressionPart
    {
        public WhereOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart filterBody, string filterParameterName) : base(parameters, sourceOperand, filterBody, filterParameterName)
        {
        }

        protected override Expression Build(Expression operandExpression) 
            => operandExpression.GetWhereCall(GetParameters(operandExpression));
    }
}
