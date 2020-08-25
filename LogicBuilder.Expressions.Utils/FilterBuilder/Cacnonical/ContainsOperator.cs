using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Cacnonical
{
    public class ContainsOperator : FilterPart
    {
        public ContainsOperator(IDictionary<string, ParameterExpression> parameters, FilterPart left, FilterPart right) : base(parameters)
        {
            Left = left;
            Right = right;
        }

        public FilterPart Left { get; private set; }
        public FilterPart Right { get; private set; }

        public override Expression Build() => Build(Left.Build());

        private Expression Build(Expression leftExpression)
        {
            if (leftExpression.Type.IsList())
                return leftExpression.GetEnumerableContainsCall(Right.Build());
            else if (leftExpression.Type == typeof(string))
                return leftExpression.GetStringContainsCall(Right.Build());
            else
                throw new ArgumentException(nameof(Left));
        }
    }
}
