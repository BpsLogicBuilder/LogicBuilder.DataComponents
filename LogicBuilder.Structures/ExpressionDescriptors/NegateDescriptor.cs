namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class NegateDescriptor : IExpressionDescriptor
    {
		public NegateDescriptor()
		{
		}

		public NegateDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}