using System;
using System.Linq;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Cacnonical
{
    public class SubstringOperator : IExpressionPart
    {
        public SubstringOperator(IExpressionPart left, params IExpressionPart[] args)
        {
            Left = left;
            Args = args;
        }

        public IExpressionPart Left { get; private set; }
        public IExpressionPart[] Args { get; private set; }

        public Expression Build() => Build(Left.Build());

        private Expression Build(Expression leftExpression)
        {
            if (leftExpression.Type == typeof(string))
                return leftExpression.GetSubStringCall
                (
                    Args.Select(arg => arg.Build()).ToArray()
                );
            else
                throw new ArgumentException(nameof(Args));
        }
    }
}
