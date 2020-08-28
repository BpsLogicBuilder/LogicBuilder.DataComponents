using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class GroupByOperator : SelectorLambdaOperatorBase, IExpressionPart
    {
        public GroupByOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart selectorBody, string selectorParameterName) : base(parameters, sourceOperand, selectorBody, selectorParameterName)
        {
        }

        protected override Expression Build(Expression operandExpression)
            => operandExpression.GetGroupByCall(GetSelector(operandExpression));
    }
}
