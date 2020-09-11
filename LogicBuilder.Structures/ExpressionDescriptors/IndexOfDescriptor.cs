namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class IndexOfDescriptor : IExpressionDescriptor
    {
		public IndexOfDescriptor()
		{
		}

		public IndexOfDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor itemToFind)
		{
			SourceOperand = sourceOperand;
			ItemToFind = itemToFind;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
		public IExpressionDescriptor ItemToFind { get; set; }
    }
}