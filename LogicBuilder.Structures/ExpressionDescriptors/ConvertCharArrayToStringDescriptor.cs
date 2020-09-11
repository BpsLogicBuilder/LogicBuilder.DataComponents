namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ConvertCharArrayToStringDescriptor : IExpressionDescriptor
    {
		public ConvertCharArrayToStringDescriptor()
		{
		}

		public ConvertCharArrayToStringDescriptor(IExpressionDescriptor sourceOperand)
		{
			SourceOperand = sourceOperand;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
    }
}