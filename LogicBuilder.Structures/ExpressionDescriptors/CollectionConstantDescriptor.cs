using System.Collections.Generic;
using System;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class CollectionConstantDescriptor : IExpressionDescriptor
    {
		public CollectionConstantDescriptor()
		{
		}

		public CollectionConstantDescriptor(ICollection<object> constantValues, Type elementType)
		{
			ConstantValues = constantValues;
			ElementType = elementType;
		}

		public Type ElementType { get; set; }
		public ICollection<object> ConstantValues { get; set; }
    }
}