namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class MemberSelectorDescriptor : IExpressionDescriptor
    {
		public MemberSelectorDescriptor()
		{
		}

		public MemberSelectorDescriptor(string memberFullName, IExpressionDescriptor sourceOperand)
		{
			MemberFullName = memberFullName;
			SourceOperand = sourceOperand;
		}

		public string MemberFullName { get; set; }
		public IExpressionDescriptor SourceOperand { get; set; }
    }
}