using System;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ConvertDescriptor : IExpressionDescriptor
    {
		public ConvertDescriptor()
		{
		}

		public ConvertDescriptor(IExpressionDescriptor sourceOperand, Type type)
		{
			SourceOperand = sourceOperand;
			Type = type;
		}

		public Type Type { get; set; }
		public IExpressionDescriptor SourceOperand { get; set; }
    }
}