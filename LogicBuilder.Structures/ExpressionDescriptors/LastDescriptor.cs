namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class LastDescriptor : FilterMethodDescriptorBase
    {
		public LastDescriptor()
		{
		}

		public LastDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor filterBody, string filterParameterName) : base(sourceOperand, filterBody, filterParameterName)
		{
		}

		public LastDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}