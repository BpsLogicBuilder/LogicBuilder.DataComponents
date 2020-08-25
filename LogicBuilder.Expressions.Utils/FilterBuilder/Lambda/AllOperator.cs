using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Lambda
{
    public class AllOperator : LambdaMethodOperator
    {
        public AllOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand, FilterPart filter, string parameterName)
            : base(parameters, operand, filter, parameterName)
        {
        }

        public AllOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand)
            : base(parameters, operand)
        {
        }

        protected override Func<Expression, Expression[], MethodCallExpression> GetMethod(Type operandExpressionType)
        {
            if (operandExpressionType.IsIQueryable())
                return LinqHelpers.GetAllQueryableCall;
            else
                return LinqHelpers.GetAllEnumerableCall;
        }
    }
}
