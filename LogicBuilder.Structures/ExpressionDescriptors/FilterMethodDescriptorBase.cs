using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    abstract public class FilterMethodDescriptorBase : IExpressionDescriptor
    {
		public FilterMethodDescriptorBase()
		{
		}

		public FilterMethodDescriptorBase(IExpressionDescriptor sourceOperand, IExpressionDescriptor filterBody, string filterParameterName)
		{
			SourceOperand = sourceOperand;
			FilterBody = filterBody;
			FilterParameterName = filterParameterName;
		}

		public FilterMethodDescriptorBase(IExpressionDescriptor sourceOperand)
		{
			SourceOperand = sourceOperand;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
		public IExpressionDescriptor FilterBody { get; set; }
		public string FilterParameterName { get; set; }
    }
}