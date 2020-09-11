namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ConvertToNumericDateDescriptor : IExpressionDescriptor
    {
		public ConvertToNumericDateDescriptor()
		{
		}

		public ConvertToNumericDateDescriptor(IExpressionDescriptor sourceOperand)
		{
			SourceOperand = sourceOperand;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
    }
}