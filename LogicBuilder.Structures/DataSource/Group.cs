using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.DataSource
{
    public class Group : Sort
    {
        public IEnumerable<Aggregator> Aggregates { get; set; }
    }
}
