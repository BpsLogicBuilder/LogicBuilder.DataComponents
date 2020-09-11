namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class SingleOrDefaultDescriptor : FilterMethodDescriptorBase
    {
		public SingleOrDefaultDescriptor()
		{
		}

		public SingleOrDefaultDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor filterBody, string filterParameterName) : base(sourceOperand, filterBody, filterParameterName)
		{
		}

		public SingleOrDefaultDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}