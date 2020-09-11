namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class SelectManyDescriptor : SelectorMethodDescriptorBase
    {
		public SelectManyDescriptor()
		{
		}

		public SelectManyDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor selectorBody, string selectorParameterName) : base(sourceOperand, selectorBody, selectorParameterName)
		{
		}
    }
}