namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class LessThanOrEqualsBinaryDescriptor : BinaryDescriptor
    {
		public LessThanOrEqualsBinaryDescriptor()
		{
		}

		public LessThanOrEqualsBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}