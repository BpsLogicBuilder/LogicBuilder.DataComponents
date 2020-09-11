namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class CeilingDescriptor : IExpressionDescriptor
    {
		public CeilingDescriptor()
		{
		}

		public CeilingDescriptor(IExpressionDescriptor operand)
		{
			Operand = operand;
		}

		public IExpressionDescriptor Operand { get; set; }
    }
}