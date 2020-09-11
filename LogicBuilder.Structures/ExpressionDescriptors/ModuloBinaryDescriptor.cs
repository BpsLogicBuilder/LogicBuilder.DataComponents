namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ModuloBinaryDescriptor : BinaryDescriptor
    {
		public ModuloBinaryDescriptor()
		{
		}

		public ModuloBinaryDescriptor(IExpressionDescriptor left, IExpressionDescriptor right) : base(left, right)
		{
		}
    }
}