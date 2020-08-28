using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class SelectOperator : SelectorLambdaOperatorBase, IExpressionPart
    {
        public SelectOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart sourceOperand, IExpressionPart selectorBody, string selectorParameterName) : base(parameters, sourceOperand, selectorBody, selectorParameterName)
        {
        }

        protected override Expression Build(Expression operandExpression)
            => operandExpression.GetSelectCall(GetSelector(operandExpression));
    }
}
