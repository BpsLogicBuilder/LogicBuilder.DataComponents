using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public abstract class FilterLambdaOperatorBase
    {
        public FilterLambdaOperatorBase(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart filterBody, string filterParameterName)
        {
            SourceOperand = sourceOperand;
            FilterBody = filterBody;
            Parameters = parameters;
            FilterParameterName = filterParameterName;
        }

        public FilterLambdaOperatorBase(IExpressionPart operand)
        {
            SourceOperand = operand;
        }

        public IExpressionPart SourceOperand { get; }
        public IExpressionPart FilterBody { get; }
        public IDictionary<string, ParameterExpression> Parameters { get; }
        public string FilterParameterName { get; }

        public Expression Build() => Build(SourceOperand.Build());

        protected abstract Expression Build(Expression operandExpression);

        protected Expression[] GetParameters(Expression operandExpression)
        {
            if (FilterBody == null)
                return new Expression[0];

            return new Expression[]
            {
                GetFilterLambdaOperator(operandExpression.GetUnderlyingElementType()).Build()
            };
        }

        protected FilterLambdaOperatorHelper GetFilterLambdaOperator(Type elementType) 
            => new FilterLambdaOperatorHelper
            (
                Parameters,
                FilterBody,
                elementType,
                FilterParameterName
            );
    }
}
