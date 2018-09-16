using System;
using System.Collections.Generic;
using System.Text;

namespace LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests
{
    public class DataSourceRequestOptions
    {
        public string Aggregate { get; set; }
        public string Filter { get; set; }
        public string Group { get; set; }
        public int Page { get; set; }
        public string Sort { get; set; }
        public int PageSize { get; set; }
    }
}
