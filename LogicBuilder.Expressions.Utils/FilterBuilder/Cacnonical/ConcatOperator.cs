using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Cacnonical
{
    public class ConcatOperator : FilterPart
    {
        public ConcatOperator(IDictionary<string, ParameterExpression> parameters, FilterPart left, FilterPart right) : base(parameters)
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
                return leftExpression.GetEnumerableConcatCall(Right.Build());
            else if (leftExpression.Type == typeof(string))
                return LinqHelpers.GetStringConcatCall(leftExpression, Right.Build());
            else
                throw new ArgumentException(nameof(leftExpression));
        }
    }
}
