using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class FirstOrDefaultOperator : FilterLambdaOperatorBase, IExpressionPart
    {
        public FirstOrDefaultOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart filterBody, string filterParameterName) : base(parameters, sourceOperand, filterBody, filterParameterName)
        {
        }

        public FirstOrDefaultOperator(IExpressionPart operand) : base(operand)
        {
        }

        protected override Expression Build(Expression operandExpression)
            => operandExpression.GetFirstOrDefaultCall(GetParameters(operandExpression));
    }
}
