namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class WhereDescriptor : FilterMethodDescriptorBase
    {
		public WhereDescriptor()
		{
		}

		public WhereDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor filterBody, string filterParameterName) : base(sourceOperand, filterBody, filterParameterName)
		{
		}
    }
}