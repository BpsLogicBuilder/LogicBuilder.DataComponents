namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class MinDescriptor : SelectorMethodDescriptorBase
    {
		public MinDescriptor()
		{
		}

		public MinDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor selectorBody, string selectorParameterName) : base(sourceOperand, selectorBody, selectorParameterName)
		{
		}

		public MinDescriptor(IExpressionDescriptor sourceOperand) : base(sourceOperand)
		{
		}
    }
}