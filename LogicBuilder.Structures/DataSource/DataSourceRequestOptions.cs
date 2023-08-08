using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.DataSource
{
    [System.Obsolete("No longer used.")]
    public class DataSourceRequestOptions
    {
        public IEnumerable<Aggregator> Aggregate { get; set; }
        public Filter Filter { get; set; }
        public IEnumerable<Group> Group { get; set; }
        public int Skip { get; set; }
        public IEnumerable<Sort> Sort { get; set; }
        public int Take { get; set; }
    }
}
