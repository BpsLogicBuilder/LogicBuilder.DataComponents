using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic
{
    public class AverageOperator : IExpressionPart
    {
        public AverageOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart selectorBody, string selectorParameterName)
        {
            SourceOperand = sourceOperand;
            SelectorBody = selectorBody;
            SelectorParameterName = selectorParameterName;
            Parameters = parameters;
        }

        public AverageOperator(IExpressionPart sourceOperand)
        {
            SourceOperand = sourceOperand;
        }

        public IExpressionPart SourceOperand { get; }
        public IExpressionPart SelectorBody { get; }
        public string SelectorParameterName { get; }
        public IDictionary<string, ParameterExpression> Parameters { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
        {
            if (SelectorBody == null)
                return operandExpression.GetAverageMethodCall(new Expression[0]);

            return operandExpression.GetAverageMethodCall
            (
                new Expression[] 
                { 
                    (LambdaExpression)new LambdaOperator
                    (
                        Parameters,
                        SelectorBody,
                        operandExpression.GetUnderlyingElementType(),
                        SelectorParameterName
                    ).Build(),
                }
            );
        }
    }
}
