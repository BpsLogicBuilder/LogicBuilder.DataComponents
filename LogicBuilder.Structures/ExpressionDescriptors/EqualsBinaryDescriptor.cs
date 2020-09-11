namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class EqualsBinaryDescriptor : BinaryDescriptor
    {
		public EqualsBinaryDescriptor()
		{
		}

		public EqualsBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}