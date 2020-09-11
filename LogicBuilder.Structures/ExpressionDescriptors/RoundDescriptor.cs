namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class RoundDescriptor : IExpressionDescriptor
    {
		public RoundDescriptor()
		{
		}

		public RoundDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}