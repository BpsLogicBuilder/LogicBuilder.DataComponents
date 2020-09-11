namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class HourDescriptor : IExpressionDescriptor
    {
		public HourDescriptor()
		{
		}

		public HourDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}