using System;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class CollectionCastDescriptor : IExpressionDescriptor
    {
		public CollectionCastDescriptor()
		{
		}

		public CollectionCastDescriptor(IExpressionDescriptor operand, Type type)
		{
			Operand = operand;
			Type = type;
		}

		public IExpressionDescriptor Operand { get; set; }
		public Type Type { get; set; }
    }
}