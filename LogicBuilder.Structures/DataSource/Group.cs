using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.DataSource
{
    [System.Obsolete("No longer used.")]
    public class Group : Sort
    {
        public IEnumerable<Aggregator> Aggregates { get; set; }
    }
}
