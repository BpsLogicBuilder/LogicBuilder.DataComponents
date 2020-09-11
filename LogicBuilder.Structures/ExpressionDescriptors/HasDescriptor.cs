namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class HasDescriptor : IExpressionDescriptor
    {
		public HasDescriptor()
		{
		}

		public HasDescriptor(IExpressionDescriptor left, IExpressionDescriptor right)
		{
			Left = left;
			Right = right;
		}

		public IExpressionDescriptor Left { get; set; }
		public IExpressionDescriptor Right { get; set; }
    }
}