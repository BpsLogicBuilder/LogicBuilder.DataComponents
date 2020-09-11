namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class AndBinaryDescriptor : BinaryDescriptor
    {
		public AndBinaryDescriptor()
		{
		}

		public AndBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}