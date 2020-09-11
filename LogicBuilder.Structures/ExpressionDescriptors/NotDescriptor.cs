namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class NotDescriptor : IExpressionDescriptor
    {
		public NotDescriptor()
		{
		}

		public NotDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}