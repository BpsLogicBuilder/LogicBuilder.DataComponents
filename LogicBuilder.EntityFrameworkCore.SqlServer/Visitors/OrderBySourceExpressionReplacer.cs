using System.Linq.Expressions;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Visitors
{
    internal class OrderBySourceExpressionReplacer : ExpressionVisitor
    {
        public OrderBySourceExpressionReplacer(Expression newSource, Expression expression)
        {
            this.expression = expression;
            this.newSource = newSource;
        }

        private readonly Expression newSource;
        private readonly Expression expression;

        public static Expression ReplaceOrderBySource(Expression newSource, Expression expression)
            => new OrderBySourceExpressionReplacer(newSource, expression).Visit(expression);

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "OrderBy" || node.Method.Name == "OrderByDescending" 
                && this.expression.ToString().StartsWith(node.ToString()))//makes sure we're not updating some nested "OrderBy"
            {
                return Expression.Call(node.Method, new Expression[] { newSource, node.Arguments[1] });
            }

            return base.VisitMethodCall(node);
        }
    }
}
