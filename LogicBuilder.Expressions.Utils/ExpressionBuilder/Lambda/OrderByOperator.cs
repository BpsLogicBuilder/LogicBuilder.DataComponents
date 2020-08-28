using LogicBuilder.Expressions.Utils.Strutures;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class OrderByOperator : IExpressionPart
    {
        public OrderByOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart selectorBody, ListSortDirection sortDirection, string selectorParameterName)
        {
            SourceOperand = sourceOperand;
            SelectorBody = selectorBody;
            SortDirection = sortDirection;
            SelectorParameterName = selectorParameterName;
            Parameters = parameters;
    }

        public IExpressionPart SourceOperand { get; }
        public IExpressionPart SelectorBody { get; }
        public ListSortDirection SortDirection { get; }
        public string SelectorParameterName { get; }
        public IDictionary<string, ParameterExpression> Parameters { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression) 
            => operandExpression.GetOrderByCall
            (
                (LambdaExpression)new LambdaOperator
                (
                    Parameters,
                    SelectorBody,
                    operandExpression.GetUnderlyingElementType(),
                    SelectorParameterName
                ).Build(),
                SortDirection
            );
    }
}
