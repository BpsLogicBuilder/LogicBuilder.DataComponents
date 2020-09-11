namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class GreaterThanOrEqualsBinaryDescriptor : BinaryDescriptor
    {
		public GreaterThanOrEqualsBinaryDescriptor()
		{
		}

		public GreaterThanOrEqualsBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}