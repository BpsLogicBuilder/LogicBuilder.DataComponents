using AutoMapper;
using LogicBuilder.Expressions.Utils;
using LogicBuilder.Expressions.Utils.Expansions;
using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Visitors
{
    internal class QueryFunctionAppender : ExpressionVisitor
    {
        public QueryFunctionAppender(Expression expression, ExpansionOptions expansion, IMapper mapper)
        {
            this.expansion = expansion;
            this.expression = expression;
            this.mapper = mapper;
        }

        private readonly ExpansionOptions expansion;
        private readonly Expression expression;
        private readonly IMapper mapper;

        public static Expression AppendQueryMethod(Expression expression, ExpansionOptions expansion, IMapper mapper)
            => new QueryFunctionAppender(expression, expansion, mapper).Visit(expression);

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Select"//both expansion.MemberType and node.Type will be lists
                && expansion.MemberType.GetUnderlyingElementType() == node.Type.GetUnderlyingElementType()
                && this.expression.ToString().StartsWith(node.ToString()))//makes sure we're not updating some nested "Select"
            {
                return OrderBySourceExpressionReplacer.ReplaceOrderBySource(node, GetMethodCallExpressionPart().Build());
            }

            return base.VisitMethodCall(node);

            IExpressionPart GetMethodCallExpressionPart()
                => mapper.Map<IExpressionPart>
                (
                    expansion.QueryOption.QueryFunction,
                    opts => opts.Items[ExpressionOperators.PARAMETERS_KEY] = new Dictionary<string, ParameterExpression>()
                );
        }
    }
}
