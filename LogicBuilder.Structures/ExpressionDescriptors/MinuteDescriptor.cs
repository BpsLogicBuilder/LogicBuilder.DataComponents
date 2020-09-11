namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class MinuteDescriptor : IExpressionDescriptor
    {
		public MinuteDescriptor()
		{
		}

		public MinuteDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}