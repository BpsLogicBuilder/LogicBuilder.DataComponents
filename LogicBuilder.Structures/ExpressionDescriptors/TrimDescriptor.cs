namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class TrimDescriptor : IExpressionDescriptor
    {
		public TrimDescriptor()
		{
		}

		public TrimDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}