using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class SelectOperator : IExpressionPart
    {
        public SelectOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart selectorBody, string selectorParameterName)
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
        {
            return operandExpression.GetSelectCall
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
}
