using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    internal class Constants
	{
        public static IDictionary<FilterFunction, ExpressionType> BinaryOperatorExpressionType = new Dictionary<FilterFunction, ExpressionType>
		{
			//Logical
			[FilterFunction.eq] = ExpressionType.Equal,
			[FilterFunction.ne] = ExpressionType.NotEqual,
			[FilterFunction.gt] = ExpressionType.GreaterThan,
			[FilterFunction.ge] = ExpressionType.GreaterThanOrEqual,
			[FilterFunction.lt] = ExpressionType.LessThan,
			[FilterFunction.le] = ExpressionType.LessThanOrEqual,
			[FilterFunction.and] = ExpressionType.AndAlso,
			[FilterFunction.or] = ExpressionType.OrElse,
			[FilterFunction.add] = ExpressionType.Add,
			[FilterFunction.sub] = ExpressionType.Subtract,
			[FilterFunction.mul] = ExpressionType.Multiply,
			[FilterFunction.div] = ExpressionType.Divide,
			[FilterFunction.mod] = ExpressionType.Modulo
		};
	}
}
