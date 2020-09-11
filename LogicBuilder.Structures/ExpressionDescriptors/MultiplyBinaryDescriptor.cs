namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class MultiplyBinaryDescriptor : BinaryDescriptor
    {
		public MultiplyBinaryDescriptor()
		{
		}

		public MultiplyBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}