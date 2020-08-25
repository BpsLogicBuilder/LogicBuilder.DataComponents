using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Cacnonical
{
    public class SubstringOperator : FilterPart
    {
        public SubstringOperator(IDictionary<string, ParameterExpression> parameters, FilterPart left, params FilterPart[] args) : base(parameters)
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
