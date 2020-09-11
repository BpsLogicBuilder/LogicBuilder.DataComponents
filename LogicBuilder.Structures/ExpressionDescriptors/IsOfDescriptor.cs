using System;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class IsOfDescriptor : IExpressionDescriptor
    {
		public IsOfDescriptor()
		{
		}

		public IsOfDescriptor(IExpressionDescriptor operand, Type type)
		{
			Operand = operand;
			Type = type;
		}

		public IExpressionDescriptor Operand { get; set; }
		public Type Type { get; set; }
    }
}