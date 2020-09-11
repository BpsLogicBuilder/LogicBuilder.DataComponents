namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class SkipDescriptor : IExpressionDescriptor
    {
		public SkipDescriptor()
		{
		}

		public SkipDescriptor(IExpressionDescriptor sourceOperand, int count)
		{
			SourceOperand = sourceOperand;
			Count = count;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
		public int Count { get; set; }
    }
}