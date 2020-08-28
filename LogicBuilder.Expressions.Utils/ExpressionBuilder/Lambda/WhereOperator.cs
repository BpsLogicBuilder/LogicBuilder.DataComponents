﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class WhereOperator : IExpressionPart
    {
        public WhereOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart filterBody, string filterParameterName)
        {
            SourceOperand = sourceOperand;
            FilterBody = filterBody;
            Parameters = parameters;
            FilterParameterName = filterParameterName;
        }

        public IExpressionPart SourceOperand { get; }
        public IExpressionPart FilterBody { get; }
        public IDictionary<string, ParameterExpression> Parameters { get; }
        public string FilterParameterName { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression) 
            => operandExpression.GetWhereCall
            (
                new FilterLambdaOperator
                (
                    Parameters,
                    FilterBody,
                    operandExpression.GetUnderlyingElementType(),
                    FilterParameterName
                ).Build()
            );
    }
}
