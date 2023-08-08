using System.Collections;
using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.DataSource
{
    [System.Obsolete("No longer used.")]
    public class DataSourceResponse
    {
        public IEnumerable Data { get; set; }
        public IEnumerable Groups { get; set; }
        public int Total { get; set; }
        public Dictionary<string, object> Aggregates { get; set; }
    }
}
