namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class AddBinaryDescriptor : BinaryDescriptor
    {
		public AddBinaryDescriptor()
		{
		}

		public AddBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}