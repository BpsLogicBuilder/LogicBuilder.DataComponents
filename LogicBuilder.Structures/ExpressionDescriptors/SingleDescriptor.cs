namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class SingleDescriptor : FilterMethodDescriptorBase
    {
		public SingleDescriptor()
		{
		}

		public SingleDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor filterBody, string filterParameterName) : base(sourceOperand, filterBody, filterParameterName)
		{
		}

		public SingleDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}