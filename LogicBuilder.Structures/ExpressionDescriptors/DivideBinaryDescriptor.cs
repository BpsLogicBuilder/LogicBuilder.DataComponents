namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class DivideBinaryDescriptor : BinaryDescriptor
    {
		public DivideBinaryDescriptor()
		{
		}

		public DivideBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}