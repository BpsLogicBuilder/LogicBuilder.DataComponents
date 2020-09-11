namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class TakeDescriptor : IExpressionDescriptor
    {
		public TakeDescriptor()
		{
		}

		public TakeDescriptor(IExpressionDescriptor sourceOperand, int count)
		{
			SourceOperand = sourceOperand;
			Count = count;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
		public int Count { get; set; }
    }
}