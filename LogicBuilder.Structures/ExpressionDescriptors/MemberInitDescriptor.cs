using System.Collections.Generic;
using System;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class MemberInitDescriptor : IExpressionDescriptor
    {
		public MemberInitDescriptor()
		{
		}

		public MemberInitDescriptor(IDictionary<string, IExpressionDescriptor> memberBindings, Type newType)
		{
			MemberBindings = memberBindings;
			NewType = newType;
		}

		public MemberInitDescriptor(IDictionary<string, IExpressionDescriptor> memberBindings)
		{
			MemberBindings = memberBindings;
		}

		public IDictionary<string, IExpressionDescriptor> MemberBindings { get; set; }
		public Type NewType { get; set; }
    }
}