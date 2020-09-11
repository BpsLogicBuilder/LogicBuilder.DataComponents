namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ToLowerDescriptor : IExpressionDescriptor
    {
		public ToLowerDescriptor()
		{
		}

		public ToLowerDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}