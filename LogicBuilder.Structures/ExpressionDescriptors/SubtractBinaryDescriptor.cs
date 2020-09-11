namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class SubtractBinaryDescriptor : BinaryDescriptor
    {
		public SubtractBinaryDescriptor()
		{
		}

		public SubtractBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}