using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class InOperator : FilterPart
    {
        public InOperator(FilterPart itemToFind, FilterPart listToSearch)
        {
            ItemToFind = itemToFind;
            ListToSearch = listToSearch;
        }

        public FilterPart ItemToFind { get; private set; }
        public FilterPart ListToSearch { get; private set; }

        public override Expression Build()
            => ListToSearch.Build().GetEnumerableContainsCall(ItemToFind.Build());
    }
}
