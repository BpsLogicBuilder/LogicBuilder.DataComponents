namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class AllDescriptor : FilterMethodDescriptorBase
    {
		public AllDescriptor()
		{
		}

		public AllDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor filterBody, string filterParameterName) : base(sourceOperand, filterBody, filterParameterName)
		{
		}

		public AllDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}