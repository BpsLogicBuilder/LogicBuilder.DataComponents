namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ContainsDescriptor : IExpressionDescriptor
    {
		public ContainsDescriptor()
		{
		}

		public ContainsDescriptor(IExpressionDescriptor left, IExpressionDescriptor right)
		{
			Left = left;
			Right = right;
		}

		public IExpressionDescriptor Left { get; set; }
		public IExpressionDescriptor Right { get; set; }
    }
}