namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class FirstDescriptor : FilterMethodDescriptorBase
    {
		public FirstDescriptor()
		{
		}

		public FirstDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor filterBody, string filterParameterName) : base(sourceOperand, filterBody, filterParameterName)
		{
		}

		public FirstDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}