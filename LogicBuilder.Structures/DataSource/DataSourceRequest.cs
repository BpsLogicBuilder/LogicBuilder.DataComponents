using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.DataSource
{
    public class DataSourceRequest
    {
        public DataSourceRequestOptions Options { get; set; }
        public string ModelType { get; set; }
        public string DataType { get; set; }
        public IEnumerable<string> Includes { get; set; }
        public IEnumerable<string> Selects { get; set; }
        public bool Distinct { get; set; }
    }
}
