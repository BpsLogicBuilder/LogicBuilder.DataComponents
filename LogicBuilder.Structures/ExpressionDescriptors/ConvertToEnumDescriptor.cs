using System;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ConvertToEnumDescriptor : IExpressionDescriptor
    {
		public ConvertToEnumDescriptor()
		{
		}

		public ConvertToEnumDescriptor(object constantValue, Type type)
		{
			ConstantValue = constantValue;
			Type = type;
		}

		public Type Type { get; set; }
		public object ConstantValue { get; set; }
    }
}