namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ConvertToNumericTimeDescriptor : IExpressionDescriptor
    {
		public ConvertToNumericTimeDescriptor()
		{
		}

		public ConvertToNumericTimeDescriptor(IExpressionDescriptor sourceOperand)
		{
			SourceOperand = sourceOperand;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
    }
}