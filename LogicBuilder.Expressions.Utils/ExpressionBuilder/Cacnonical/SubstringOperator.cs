using System;
using System.Linq;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Cacnonical
{
    public class SubstringOperator : IExpressionPart
    {
        public SubstringOperator(IExpressionPart sourceOperand, params IExpressionPart[] indexes)
        {
            SourceOperand = sourceOperand;
            Indexes = indexes;
        }

        public IExpressionPart SourceOperand { get; private set; }
        public IExpressionPart[] Indexes { get; private set; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression leftExpression)
        {
            if (leftExpression.Type == typeof(string))
                return leftExpression.GetSubStringCall
                (
                    Indexes.Select(arg => arg.Build()).ToArray()
                );
            else
                throw new ArgumentException(nameof(Indexes));
        }
    }
}
