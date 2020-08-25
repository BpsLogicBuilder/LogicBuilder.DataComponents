using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
	internal class Constants
	{
		//public static IDictionary<FilterFunction, string> FilterFunctions = new Dictionary<FilterFunction, string>
		//{
		//	//Logical
		//	[FilterFunction.eq] = "eq",
		//	[FilterFunction.ne] = "ne",
		//	[FilterFunction.gt] = "gt",
		//	[FilterFunction.ge] = "ge",
		//	[FilterFunction.lt] = "lt",
		//	[FilterFunction.le] = "le",
		//	[FilterFunction.and] = "and",
		//	[FilterFunction.or] = "or",
		//	[FilterFunction.not] = "not",
		//	[FilterFunction.has] = "has",
		//	[FilterFunction.@in] = "in",

		//	//Arithmetic
		//	[FilterFunction.add] = "add",
		//	[FilterFunction.sub] = "sub",
		//	[FilterFunction.negation] = "negation",
		//	[FilterFunction.mul] = "mul",
		//	[FilterFunction.div] = "div",
		//	[FilterFunction.mod] = "mod",

		//	//Cacnonical
		//	[FilterFunction.concat] = "concat",
		//	[FilterFunction.contains] = "contains",
		//	[FilterFunction.endswith] = "endswith",
		//	[FilterFunction.indexof] = "indexof",
		//	[FilterFunction.length] = "length",
		//	[FilterFunction.startswith] = "startswith",
		//	[FilterFunction.substring] = "substring",

		//	//Collection
		//	[FilterFunction.hassubset] = "hassubset",
		//	[FilterFunction.hassubsequence] = "hassubsequence",

		//	//String
		//	[FilterFunction.tolower] = "tolower",
		//	[FilterFunction.toupper] = "toupper",
		//	[FilterFunction.trim] = "trim",

		//	//DateTime
		//	[FilterFunction.date] = "date",
		//	[FilterFunction.day] = "day",
		//	[FilterFunction.fractionalseconds] = "fractionalseconds",
		//	[FilterFunction.hour] = "hour",
		//	[FilterFunction.maxdatetime] = "maxdatetime",
		//	[FilterFunction.mindatetime] = "mindatetime",
		//	[FilterFunction.minute] = "minute",
		//	[FilterFunction.month] = "month",
		//	[FilterFunction.now] = "now",
		//	[FilterFunction.second] = "second",
		//	[FilterFunction.time] = "time",
		//	[FilterFunction.totaloffsetminutes] = "totaloffsetminutes",
		//	[FilterFunction.totalseconds] = "totalseconds",
		//	[FilterFunction.year] = "year",

		//	//Arithmetic
		//	[FilterFunction.ceiling] = "ceiling",
		//	[FilterFunction.floor] = "floor",
		//	[FilterFunction.round] = "round",

		//	//Type
		//	[FilterFunction.cast] = "cast",
		//	[FilterFunction.isof] = "isof",

		//	//Lambda
		//	[FilterFunction.any] = "any",
		//	[FilterFunction.all] = "all"
		//};

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
