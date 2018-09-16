using System;
using System.Collections.Generic;
using System.Text;

namespace LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests
{
    public class DataRequest
    {
        public DataSourceRequestOptions Options { get; set; }
        public IEnumerable<string> Includes { get; set; }
        public IDictionary<string, string> Selects { get; set; }
        public bool Distinct { get; set; }
    }
}
