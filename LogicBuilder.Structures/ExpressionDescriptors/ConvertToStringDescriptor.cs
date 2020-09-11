namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ConvertToStringDescriptor : IExpressionDescriptor
    {
		public ConvertToStringDescriptor()
		{
		}

		public ConvertToStringDescriptor(IExpressionDescriptor sourceOperand)
		{
			SourceOperand = sourceOperand;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
    }
}