namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class GreaterThanBinaryDescriptor : BinaryDescriptor
    {
		public GreaterThanBinaryDescriptor()
		{
		}

		public GreaterThanBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}