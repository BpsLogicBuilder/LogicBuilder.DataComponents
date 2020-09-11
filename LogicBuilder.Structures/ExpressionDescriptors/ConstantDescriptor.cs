using System;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ConstantDescriptor : IExpressionDescriptor
    {
		public ConstantDescriptor()
		{
		}

		public ConstantDescriptor(object constantValue, Type type)
		{
			ConstantValue = constantValue;
			Type = type;
		}

		public ConstantDescriptor(object constantValue)
		{
			ConstantValue = constantValue;
		}

		public Type Type { get; set; }
		public object ConstantValue { get; set; }
    }
}