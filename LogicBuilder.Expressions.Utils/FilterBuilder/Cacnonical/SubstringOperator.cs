using System;
using System.Linq;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Cacnonical
{
    public class SubstringOperator : FilterPart
    {
        public SubstringOperator(FilterPart left, params FilterPart[] args)
        {
            Left = left;
            Args = args;
        }

        public FilterPart Left { get; private set; }
        public FilterPart[] Args { get; private set; }

        public override Expression Build() => Build(Left.Build());

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
