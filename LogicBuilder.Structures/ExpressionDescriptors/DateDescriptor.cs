namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class DateDescriptor : IExpressionDescriptor
    {
		public DateDescriptor()
		{
		}

		public DateDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}