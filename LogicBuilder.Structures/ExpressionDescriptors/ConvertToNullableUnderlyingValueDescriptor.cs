namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ConvertToNullableUnderlyingValueDescriptor : IExpressionDescriptor
    {
		public ConvertToNullableUnderlyingValueDescriptor()
		{
		}

		public ConvertToNullableUnderlyingValueDescriptor(IExpressionDescriptor sourceOperand)
		{
			SourceOperand = sourceOperand;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
    }
}