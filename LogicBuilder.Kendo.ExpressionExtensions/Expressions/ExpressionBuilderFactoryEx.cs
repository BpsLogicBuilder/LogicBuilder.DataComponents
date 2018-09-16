using Kendo.Mvc.Infrastructure.Implementation.Expressions;
using LogicBuilder.Kendo.ExpressionExtensions.Extensions;
using System;
using System.Linq.Expressions;

namespace LogicBuilder.Kendo.ExpressionExtensions.Expressions
{
    internal static class ExpressionBuilderFactoryEx
    {
        public static MemberAccessExpressionBuilderBase MemberAccess(Expression expression, Type memberType, string memberName)
        {
            var builder = ExpressionBuilderFactory.MemberAccess(expression.GetUnderlyingElementType(), memberType, memberName);
            //builder.Options.LiftMemberAccessToNull = source.Provider.IsLinqToObjectsProvider();
            builder.Options.LiftMemberAccessToNull = false;

            return builder;
        }
    }
}
