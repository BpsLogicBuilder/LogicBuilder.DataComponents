using System.Collections.Generic;
using System;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class FilterLambdaDescriptor : IExpressionDescriptor
    {
		public FilterLambdaDescriptor()
		{
		}

		public FilterLambdaDescriptor(IExpressionDescriptor selector, Type sourceElementType, string parameterName)
		{
			Selector = selector;
			SourceElementType = sourceElementType;
			ParameterName = parameterName;
		}

		public IExpressionDescriptor Selector { get; set; }
		public Type SourceElementType { get; set; }
		public string ParameterName { get; set; }
    }
}