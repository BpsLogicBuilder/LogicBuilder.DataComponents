using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class InOperator : IExpressionPart
    {
        public InOperator(IExpressionPart itemToFind, IExpressionPart listToSearch)
        {
            ItemToFind = itemToFind;
            ListToSearch = listToSearch;
        }

        public IExpressionPart ItemToFind { get; private set; }
        public IExpressionPart ListToSearch { get; private set; }

        public Expression Build()
            => ListToSearch.Build().GetEnumerableContainsCall(ItemToFind.Build());
    }
}
