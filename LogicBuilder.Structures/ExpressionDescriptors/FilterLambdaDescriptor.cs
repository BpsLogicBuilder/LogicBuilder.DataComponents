using System;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class FilterLambdaDescriptor : IExpressionDescriptor
    {
		public FilterLambdaDescriptor()
		{
		}

		public FilterLambdaDescriptor(IExpressionDescriptor filterBody, Type sourceElementType, string parameterName)
		{
			FilterBody = filterBody;
			SourceElementType = sourceElementType;
			ParameterName = parameterName;
		}

		public IExpressionDescriptor FilterBody { get; set; }
		public Type SourceElementType { get; set; }
		public string ParameterName { get; set; }
    }
}