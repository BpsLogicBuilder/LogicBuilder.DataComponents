namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class YearDescriptor : IExpressionDescriptor
    {
		public YearDescriptor()
		{
		}

		public YearDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}