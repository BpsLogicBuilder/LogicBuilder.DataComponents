namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class LessThanBinaryDescriptor : BinaryDescriptor
    {
		public LessThanBinaryDescriptor()
		{
		}

		public LessThanBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}