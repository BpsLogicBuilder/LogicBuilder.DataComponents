using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.Expansions
{
    public class SelectExpandItem
    {
        public string MemberName { get; set; }
        public SelectExpandItemFilter Filter { get; set; }
        public SelectExpandItemQueryFunction QueryFunction { get; set; }
        public List<string> Selects { get; set; } = new List<string>();
        public List<SelectExpandItem> ExpandedItems { get; set; } = new List<SelectExpandItem>();
    }
}
