namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ConcatDescriptor : IExpressionDescriptor
    {
		public ConcatDescriptor()
		{
		}

		public ConcatDescriptor(IExpressionDescriptor left, IExpressionDescriptor right)
		{
			Left = left;
			Right = right;
		}

		public IExpressionDescriptor Left { get; set; }
		public IExpressionDescriptor Right { get; set; }
    }
}