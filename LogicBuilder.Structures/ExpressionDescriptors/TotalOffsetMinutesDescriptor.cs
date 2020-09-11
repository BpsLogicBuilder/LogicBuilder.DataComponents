namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class TotalOffsetMinutesDescriptor : IExpressionDescriptor
    {
		public TotalOffsetMinutesDescriptor()
		{
		}

		public TotalOffsetMinutesDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}