namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class MonthDescriptor : IExpressionDescriptor
    {
		public MonthDescriptor()
		{
		}

		public MonthDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}