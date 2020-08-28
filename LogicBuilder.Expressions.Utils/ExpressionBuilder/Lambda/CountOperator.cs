﻿using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class CountOperator : IExpressionPart
    {
        public CountOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart filterBody, string filterParameterName)
        {
            SourceOperand = sourceOperand;
            FilterBody = filterBody;
            Parameters = parameters;
            FilterParameterName = filterParameterName;
        }

        public CountOperator(IExpressionPart operand)
        {
            SourceOperand = operand;
        }

        public IExpressionPart SourceOperand { get; }
        public IExpressionPart FilterBody { get; }
        public IDictionary<string, ParameterExpression> Parameters { get; }
        public string FilterParameterName { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
            => operandExpression.GetCountCall(GetParameters(operandExpression));

        private Expression[] GetParameters(Expression operandExpression)
        {
            if (FilterBody == null)
                return new Expression[0];

            return new Expression[]
            {
                new FilterLambdaOperator
                (
                    Parameters,
                    FilterBody,
                    operandExpression.GetUnderlyingElementType(),
                    FilterParameterName
                ).Build()
            };
        }
    }
}
