using Kendo.Mvc;
using Kendo.Mvc.Infrastructure.Implementation.Expressions;
using LogicBuilder.Kendo.ExpressionExtensions.Extensions;
using System.Linq.Expressions;

namespace LogicBuilder.Kendo.ExpressionExtensions.Grouping
{
    internal abstract class GroupDescriptorExpressionBuilderBaseEx : ExpressionBuilderBase
    {
        protected Expression queryable;

        protected GroupDescriptorExpressionBuilderBaseEx(Expression parentExpression) : base(parentExpression.GetUnderlyingElementType())
        {
            this.queryable = parentExpression;
        }

        public virtual Expression Queryable
        {
            get
            {
                return this.queryable;
            }
            protected set
            {
                this.queryable = value;
            }
        }

        public virtual MethodCallExpression CreateExpression()
        {
            return
                queryable.
                    GroupBy(this.CreateGroupByExpression()).
                    OrderBy(this.CreateOrderByExpression(), SortDirection).
                    Select(this.CreateSelectExpression());

        }

        protected virtual ListSortDirection? SortDirection
        {
            get
            {
                return null;
            }
        }

        protected abstract LambdaExpression CreateGroupByExpression();
        protected abstract LambdaExpression CreateOrderByExpression();
        protected abstract LambdaExpression CreateSelectExpression();
    }
}
