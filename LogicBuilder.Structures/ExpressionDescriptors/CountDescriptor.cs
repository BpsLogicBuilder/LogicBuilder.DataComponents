namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class CountDescriptor : FilterMethodDescriptorBase
    {
		public CountDescriptor()
		{
		}

		public CountDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor filterBody, string filterParameterName) : base(sourceOperand, filterBody, filterParameterName)
		{
		}

		public CountDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}