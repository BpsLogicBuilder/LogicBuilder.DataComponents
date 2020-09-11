namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class LengthDescriptor : IExpressionDescriptor
    {
		public LengthDescriptor()
		{
		}

		public LengthDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}