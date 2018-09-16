using Kendo.Mvc;
using LogicBuilder.Kendo.ExpressionExtensions.Expressions;
using LogicBuilder.Kendo.ExpressionExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LogicBuilder.Kendo.ExpressionExtensions.Sorting
{
    internal class SortDescriptorCollectionExpressionBuilderEx
    {
        private readonly IEnumerable<SortDescriptor> sortDescriptors;
        private readonly Expression parentExpression;

        public SortDescriptorCollectionExpressionBuilderEx(Expression parentExpression, IEnumerable<SortDescriptor> sortDescriptors)
        {
            this.parentExpression = parentExpression;
            this.sortDescriptors = sortDescriptors;
        }

        public MethodCallExpression GetSortExpression()
        {
            MethodCallExpression mce = null;
            bool isFirst = true;

            foreach (var descriptor in this.sortDescriptors)
            {
                Type memberType = typeof(object);
                var descriptorBuilder = ExpressionBuilderFactoryEx.MemberAccess(this.parentExpression, memberType, descriptor.Member);
                var expression = descriptorBuilder.CreateLambdaExpression();

                string methodName = "";
                if (isFirst)
                {
                    methodName = descriptor.SortDirection == ListSortDirection.Ascending ?
                        "OrderBy" : "OrderByDescending";
                }
                else
                {
                    methodName = descriptor.SortDirection == ListSortDirection.Ascending ?
                        "ThenBy" : "ThenByDescending";
                }

                mce = Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { parentExpression.GetUnderlyingElementType(), expression.Body.Type },
                        isFirst ? parentExpression : mce,
                        Expression.Quote(expression));

                isFirst = false;
            }

            return mce;
        }
    }
}
