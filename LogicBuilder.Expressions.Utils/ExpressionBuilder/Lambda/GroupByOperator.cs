using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class GroupByOperator : IExpressionPart
    {
        public GroupByOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart selectorBody, string selectorParameterName)
        {
            SourceOperand = sourceOperand;
            SelectorBody = selectorBody;
            SelectorParameterName = selectorParameterName;
            Parameters = parameters;
        }

        public IExpressionPart SourceOperand { get; }
        public IExpressionPart SelectorBody { get; }
        public string SelectorParameterName { get; }
        public IDictionary<string, ParameterExpression> Parameters { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
            => operandExpression.GetGroupByCall
            (
                (LambdaExpression)new LambdaOperator
                (
                    Parameters,
                    SelectorBody,
                    operandExpression.GetUnderlyingElementType(),
                    SelectorParameterName
                ).Build()
            );
    }
}
