using LogicBuilder.Expressions.Utils.Strutures;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ThenByDescriptor : SelectorMethodDescriptorBase
    {
		public ThenByDescriptor()
		{
		}

		public ThenByDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor selectorBody, ListSortDirection sortDirection, string selectorParameterName) : base(sourceOperand, selectorBody, selectorParameterName)
		{
			SortDirection = sortDirection;
		}

		public ListSortDirection SortDirection { get; set; }
    }
}