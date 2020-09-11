namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class FirstOrDefaultDescriptor : FilterMethodDescriptorBase
    {
		public FirstOrDefaultDescriptor()
		{
		}

		public FirstOrDefaultDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor filterBody, string filterParameterName) : base(sourceOperand, filterBody, filterParameterName)
		{
		}

		public FirstOrDefaultDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}