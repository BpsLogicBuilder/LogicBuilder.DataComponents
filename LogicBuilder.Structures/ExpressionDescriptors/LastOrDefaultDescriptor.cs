namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class LastOrDefaultDescriptor : FilterMethodDescriptorBase
    {
		public LastOrDefaultDescriptor()
		{
		}

		public LastOrDefaultDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor filterBody, string filterParameterName) : base(sourceOperand, filterBody, filterParameterName)
		{
		}

		public LastOrDefaultDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}