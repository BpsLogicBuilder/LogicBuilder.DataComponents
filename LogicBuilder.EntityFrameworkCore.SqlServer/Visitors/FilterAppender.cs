using AutoMapper;
using LogicBuilder.Expressions.Utils;
using LogicBuilder.Expressions.Utils.Expansions;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Visitors
{
    internal class FilterAppender : ExpressionVisitor
    {
        public FilterAppender(Expression expression, ExpansionOptions expansion, IMapper mapper)
        {
            this.expansion = expansion;
            this.expression = expression;
            this.mapper = mapper;
        }

        private readonly ExpansionOptions expansion;
        private readonly Expression expression;
        private readonly IMapper mapper;

        public static Expression AppendFilter(Expression expression, ExpansionOptions expansion, IMapper mapper)
            => new FilterAppender(expression, expansion, mapper).Visit(expression);

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Select"
                && expansion.MemberType.GetUnderlyingElementType() == node.GetUnderlyingElementType()
                && this.expression.ToString().StartsWith(node.ToString()))//makes sure we're not updating some nested "Select"
            {
                return node.GetWhereCall(expansion.FilterOption.FilterLambdaOperator.Build());
            }

            return base.VisitMethodCall(node);
        }
    }
}
