using LogicBuilder.Expressions.Utils.Strutures;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class OrderByOperator : SelectorLambdaOperatorBase, IExpressionPart
    {
        public OrderByOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart selectorBody, ListSortDirection sortDirection, string selectorParameterName) : base(parameters, sourceOperand, selectorBody, selectorParameterName)
        {
            SortDirection = sortDirection;
        }

        public ListSortDirection SortDirection { get; }

        protected override Expression Build(Expression operandExpression)
            => operandExpression.GetOrderByCall(GetSelector(operandExpression), SortDirection);
    }
}
