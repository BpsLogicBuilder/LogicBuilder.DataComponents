namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class DistinctDescriptor : IExpressionDescriptor
    {
		public DistinctDescriptor()
		{
		}

		public DistinctDescriptor(IExpressionDescriptor sourceOperand)
		{
			SourceOperand = sourceOperand;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
    }
}