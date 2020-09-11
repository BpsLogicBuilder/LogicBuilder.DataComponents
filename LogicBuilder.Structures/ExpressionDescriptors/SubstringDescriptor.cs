namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class SubstringDescriptor : IExpressionDescriptor
    {
		public SubstringDescriptor()
		{
		}

		public SubstringDescriptor(IExpressionDescriptor sourceOperand, params IExpressionDescriptor[] indexes)
		{
			SourceOperand = sourceOperand;
			Indexes = indexes;
		}

		public IExpressionDescriptor SourceOperand { get; set; }
		public IExpressionDescriptor[] Indexes { get; set; }
    }
}