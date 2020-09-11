using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    abstract public class SelectorMethodDescriptorBase : IExpressionDescriptor
    {
		public SelectorMethodDescriptorBase()
		{
		}

		public SelectorMethodDescriptorBase(IExpressionDescriptor sourceOperand, IExpressionDescriptor selectorBody, string selectorParameterName)
		{
			SourceOperand = sourceOperand;
			SelectorBody = selectorBody;
			SelectorParameterName = selectorParameterName;
		}

		public SelectorMethodDescriptorBase(IExpressionDescriptor sourceOperand)
		{
			SourceOperand = sourceOperand;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
		public IExpressionDescriptor SelectorBody { get; set; }
		public string SelectorParameterName { get; set; }
    }
}