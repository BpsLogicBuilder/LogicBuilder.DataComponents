namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    abstract public class BinaryDescriptor : IExpressionDescriptor
    {
		public BinaryDescriptor()
		{
		}

		public BinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right)
		{
			Left = left;
			Right = right;
		}

		public IExpressionDescriptor Left { get; set; }
		public IExpressionDescriptor Right { get; set; }
    }
}