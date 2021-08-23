namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
	public class AsEnumerableDescriptor : IExpressionDescriptor
	{
		public AsEnumerableDescriptor()
		{
		}

		public AsEnumerableDescriptor(IExpressionDescriptor sourceOperand)
		{
			SourceOperand = sourceOperand;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
	}
}