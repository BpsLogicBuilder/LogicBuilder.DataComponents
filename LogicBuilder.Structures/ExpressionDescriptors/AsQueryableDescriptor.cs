namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class AsQueryableDescriptor : IExpressionDescriptor
    {
		public AsQueryableDescriptor()
		{
		}

		public AsQueryableDescriptor(IExpressionDescriptor sourceOperand)
		{
			SourceOperand = sourceOperand;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
    }
}