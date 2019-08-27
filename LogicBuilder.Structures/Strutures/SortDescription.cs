namespace LogicBuilder.Expressions.Utils.Strutures
{
    public class SortDescription
    {
        public SortDescription()
        {

        }

        public SortDescription(string propertyName, ListSortDirection order)
        {
            this.PropertyName = propertyName;
            this.SortDirection = order;
        }

        public string PropertyName { get; set; }
        public ListSortDirection SortDirection { get; set; }
    }
}
