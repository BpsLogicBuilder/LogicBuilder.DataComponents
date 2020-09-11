namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class OrBinaryDescriptor : BinaryDescriptor
    {
		public OrBinaryDescriptor()
		{
		}

		public OrBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}