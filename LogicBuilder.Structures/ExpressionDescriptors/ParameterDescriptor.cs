using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ParameterDescriptor : IExpressionDescriptor
    {
		public ParameterDescriptor()
		{
		}

		public ParameterDescriptor(string parameterName)
		{
			ParameterName = parameterName;
		}

		public string ParameterName { get; set; }
    }
}