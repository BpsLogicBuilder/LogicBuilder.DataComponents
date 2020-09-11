using System.Collections.Generic;
using System;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class SelectorLambdaDescriptor : IExpressionDescriptor
    {
		public SelectorLambdaDescriptor()
		{
		}

		public SelectorLambdaDescriptor(IExpressionDescriptor selector, Type sourceElementType, string parameterName)
		{
			Selector = selector;
			SourceElementType = sourceElementType;
			ParameterName = parameterName;
		}

		public SelectorLambdaDescriptor(IExpressionDescriptor selector, Type sourceElementType, Type bodyType, string parameterName)
		{
			Selector = selector;
			SourceElementType = sourceElementType;
			BodyType = bodyType;
			ParameterName = parameterName;
		}

		public IExpressionDescriptor Selector { get; set; }
		public Type SourceElementType { get; set; }
		public Type BodyType { get; set; }
		public string ParameterName { get; set; }
    }
}