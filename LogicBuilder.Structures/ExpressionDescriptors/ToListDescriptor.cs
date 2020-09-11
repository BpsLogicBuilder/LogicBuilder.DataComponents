namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ToListDescriptor : IExpressionDescriptor
    {
		public ToListDescriptor()
		{
		}

		public ToListDescriptor(IExpressionDescriptor sourceOperand)
		{
			SourceOperand = sourceOperand;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
    }
}