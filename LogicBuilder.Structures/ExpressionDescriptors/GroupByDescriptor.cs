namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class GroupByDescriptor : SelectorMethodDescriptorBase
    {
		public GroupByDescriptor()
		{
		}

		public GroupByDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor selectorBody, string selectorParameterName) : base(sourceOperand, selectorBody, selectorParameterName)
		{
		}
    }
}