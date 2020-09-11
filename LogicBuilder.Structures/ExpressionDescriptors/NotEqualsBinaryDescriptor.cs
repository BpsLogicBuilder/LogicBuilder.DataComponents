namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class NotEqualsBinaryDescriptor : BinaryDescriptor
    {
		public NotEqualsBinaryDescriptor()
		{
		}

		public NotEqualsBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}