namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class AverageDescriptor : SelectorMethodDescriptorBase
    {
		public AverageDescriptor()
		{
		}

		public AverageDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor selectorBody, string selectorParameterName) : base(sourceOperand, selectorBody, selectorParameterName)
		{
		}

		public AverageDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}