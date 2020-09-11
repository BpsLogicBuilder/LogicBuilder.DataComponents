namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class DayDescriptor : IExpressionDescriptor
    {
		public DayDescriptor()
		{
		}

		public DayDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}