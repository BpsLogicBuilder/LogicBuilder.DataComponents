using Kendo.Mvc;
using Kendo.Mvc.Infrastructure.Implementation.Expressions;
using LogicBuilder.Kendo.ExpressionExtensions.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LogicBuilder.Kendo.ExpressionExtensions.Grouping
{
    internal class GroupDescriptorCollectionExpressionBuilderEx : ExpressionBuilderBase
    {
        private readonly Expression queryable;
        private readonly IEnumerable<GroupDescriptor> groupDescriptors;
        private readonly Expression notPagedData;

        public GroupDescriptorCollectionExpressionBuilderEx(Expression expression, IEnumerable<GroupDescriptor> groupDescriptors, Expression notPagedData)
            : base(expression.GetUnderlyingElementType())
        {
            this.queryable = expression;
            this.groupDescriptors = groupDescriptors;
            this.notPagedData = notPagedData;
        }

        public Expression CreateExpression()
        {
            GroupDescriptorExpressionBuilderEx childBuilder = null;
            foreach (GroupDescriptor groupDescriptor in groupDescriptors.Reverse())
            {
                var builder = new GroupDescriptorExpressionBuilderEx(this.queryable, groupDescriptor, childBuilder, notPagedData);
                //builder.Options.LiftMemberAccessToNull = queryable.Provider.IsLinqToObjectsProvider();
                builder.Options.LiftMemberAccessToNull = false;
                childBuilder = builder;
            }

            if (childBuilder != null)
            {
                return childBuilder.CreateExpression();
            }

            return queryable;
        }
    }
}
