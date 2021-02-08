using LogicBuilder.Expressions.Utils.Expansions;
using LogicBuilder.Expressions.Utils.Strutures;
using System.Collections.Generic;

namespace LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests
{
    public class DataRequest
    {
        public DataSourceRequestOptions Options { get; set; }
        public IEnumerable<string> Includes { get; set; }
        public ICollection<FilteredInclude> FilteredIncludes { get; set; }
        public IDictionary<string, string> Selects { get; set; }
        public SelectExpandDefinition SelectExpandDefinition { get; set; }
        public bool Distinct { get; set; }
    }
}
