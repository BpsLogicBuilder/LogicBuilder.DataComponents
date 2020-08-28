using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class SumOperator : SelectorLambdaOperatorBase, IExpressionPart
    {
        public SumOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart selectorBody, string selectorParameterName) : base(parameters, sourceOperand, selectorBody, selectorParameterName)
        {
        }

        public SumOperator(IExpressionPart sourceOperand) : base(sourceOperand)
        {
        }

        protected override Expression Build(Expression operandExpression)
            => operandExpression.GetSumCall(GetParameters(operandExpression));
    }
}
