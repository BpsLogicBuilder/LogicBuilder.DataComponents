namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class StartsWithDescriptor : IExpressionDescriptor
    {
		public StartsWithDescriptor()
		{
		}

		public StartsWithDescriptor(IExpressionDescriptor left, IExpressionDescriptor right)
		{
			Left = left;
			Right = right;
		}

		public IExpressionDescriptor Left { get; set; }
		public IExpressionDescriptor Right { get; set; }
    }
}