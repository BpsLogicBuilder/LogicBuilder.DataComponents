namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class FractionalSecondsDescriptor : IExpressionDescriptor
    {
		public FractionalSecondsDescriptor()
		{
		}

		public FractionalSecondsDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}