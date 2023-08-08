using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.DataSource
{
    [System.Obsolete("No longer used.")]
    public class GroupResult
    {
        public object Value { get; set; }
        public string Field { get; set; }
        public int Count { get; set; }
        public Dictionary<string, object> Aggregates { get; set; }
        public System.Collections.IEnumerable Items { get; set; }
        public bool HasSubgroups { get; set; }
    }
}
