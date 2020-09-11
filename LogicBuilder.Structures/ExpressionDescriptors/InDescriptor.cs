namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class InDescriptor : IExpressionDescriptor
    {
		public InDescriptor()
		{
		}

		public InDescriptor(IExpressionDescriptor itemToFind, IExpressionDescriptor listToSearch)
		{
			ItemToFind = itemToFind;
			ListToSearch = listToSearch;
		}

		public IExpressionDescriptor ItemToFind { get; set; }
		public IExpressionDescriptor ListToSearch { get; set; }
    }
}