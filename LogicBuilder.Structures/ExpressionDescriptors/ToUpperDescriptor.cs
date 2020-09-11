namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ToUpperDescriptor : IExpressionDescriptor
    {
		public ToUpperDescriptor()
		{
		}

		public ToUpperDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}