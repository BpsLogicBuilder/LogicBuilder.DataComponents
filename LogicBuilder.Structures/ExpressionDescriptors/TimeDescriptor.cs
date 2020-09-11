namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class TimeDescriptor : IExpressionDescriptor
    {
		public TimeDescriptor()
		{
		}

		public TimeDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}