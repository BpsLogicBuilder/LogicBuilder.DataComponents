namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class SelectDescriptor : SelectorMethodDescriptorBase
    {
		public SelectDescriptor()
		{
		}

		public SelectDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor selectorBody, string selectorParameterName) : base(sourceOperand, selectorBody, selectorParameterName)
		{
		}
    }
}