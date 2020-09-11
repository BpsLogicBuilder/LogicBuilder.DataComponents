namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class AnyDescriptor : FilterMethodDescriptorBase
    {
		public AnyDescriptor()
		{
		}

		public AnyDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor filterBody, string filterParameterName) : base(sourceOperand, filterBody, filterParameterName)
		{
		}

		public AnyDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}