namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class SumDescriptor : SelectorMethodDescriptorBase
    {
		public SumDescriptor()
		{
		}

		public SumDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor selectorBody, string selectorParameterName) : base(sourceOperand, selectorBody, selectorParameterName)
		{
		}

		public SumDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}