using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class AverageOperator : SelectorMethodOperatorBase, IExpressionPart
    {
        public AverageOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart selectorBody, string selectorParameterName) : base(parameters, sourceOperand, selectorBody, selectorParameterName)
        {
        }

        public AverageOperator(IExpressionPart sourceOperand) : base(sourceOperand)
        {
        }

        protected override Expression Build(Expression operandExpression)
            => operandExpression.GetAverageCall(GetParameters(operandExpression));
    }
}
