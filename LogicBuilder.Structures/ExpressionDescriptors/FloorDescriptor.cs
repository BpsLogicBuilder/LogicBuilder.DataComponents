namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class FloorDescriptor : IExpressionDescriptor
    {
		public FloorDescriptor()
		{
		}

		public FloorDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}