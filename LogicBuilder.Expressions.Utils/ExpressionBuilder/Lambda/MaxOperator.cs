using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class MaxOperator : SelectorMethodOperatorBase, IExpressionPart
    {
        public MaxOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart selectorBody, string selectorParameterName) : base(parameters, sourceOperand, selectorBody, selectorParameterName)
        {
        }

        public MaxOperator(IExpressionPart sourceOperand) : base(sourceOperand)
        {
        }

        protected override Expression Build(Expression operandExpression)
            => operandExpression.GetMaxCall(GetParameters(operandExpression));
    }
}
