namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class MaxDescriptor : SelectorMethodDescriptorBase
    {
		public MaxDescriptor()
		{
		}

		public MaxDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor selectorBody, string selectorParameterName) : base(sourceOperand, selectorBody, selectorParameterName)
		{
		}

		public MaxDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}