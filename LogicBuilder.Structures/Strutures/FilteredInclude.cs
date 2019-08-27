using LogicBuilder.Expressions.Utils.DataSource;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicBuilder.Expressions.Utils.Strutures
{
    public class FilteredInclude
    {
        public string Filter { get; set; }
        public FilterGroup FilterGroup { get; set; }
        public string Include { get; set; }
        public ICollection<FilteredInclude> FilteredIncludes { get; set; }
    }
}
