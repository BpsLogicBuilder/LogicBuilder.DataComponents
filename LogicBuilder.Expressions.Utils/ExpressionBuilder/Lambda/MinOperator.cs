using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class MinOperator : SelectorLambdaOperatorBase, IExpressionPart
    {
        public MinOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart selectorBody, string selectorParameterName) : base(parameters, sourceOperand, selectorBody, selectorParameterName)
        {
        }

        public MinOperator(IExpressionPart sourceOperand) : base(sourceOperand)
        {
        }

        protected override Expression Build(Expression operandExpression)
            => operandExpression.GetMinCall(GetParameters(operandExpression));
    }
}
