namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class TotalSecondsDescriptor : IExpressionDescriptor
    {
		public TotalSecondsDescriptor()
		{
		}

		public TotalSecondsDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}