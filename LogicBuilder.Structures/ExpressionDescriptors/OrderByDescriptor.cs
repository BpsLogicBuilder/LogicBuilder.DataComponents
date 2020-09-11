using LogicBuilder.Expressions.Utils.Strutures;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class OrderByDescriptor : SelectorMethodDescriptorBase
    {
		public OrderByDescriptor()
		{
		}

		public OrderByDescriptor(IExpressionDescriptor sourceOperand, IExpressionDescriptor selectorBody, ListSortDirection sortDirection, string selectorParameterName) : base(sourceOperand, selectorBody, selectorParameterName)
		{
			SortDirection = sortDirection;
		}

		public ListSortDirection SortDirection { get; set; }
    }
}