namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class EndsWithDescriptor : IExpressionDescriptor
    {
		public EndsWithDescriptor()
		{
		}

		public EndsWithDescriptor(IExpressionDescriptor left, IExpressionDescriptor right)
		{
			Left = left;
			Right = right;
		}

		public IExpressionDescriptor Left { get; set; }
		public IExpressionDescriptor Right { get; set; }
    }
}