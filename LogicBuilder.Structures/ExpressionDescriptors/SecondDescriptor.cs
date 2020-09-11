namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class SecondDescriptor : IExpressionDescriptor
    {
		public SecondDescriptor()
		{
		}

		public SecondDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}