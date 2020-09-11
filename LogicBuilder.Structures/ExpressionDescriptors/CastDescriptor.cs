using System;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class CastDescriptor : IExpressionDescriptor
    {
		public CastDescriptor()
		{
		}

		public CastDescriptor(IExpressionDescriptor operand, Type type)
		{
			Operand = operand;
			Type = type;
		}

		public IExpressionDescriptor Operand { get; set; }
		public Type Type { get; set; }
    }
}