using LogicBuilder.Attributes;
using System.Collections.Generic;

namespace LogicBuilder.Expressions.Utils.DataSource
{
    public class FilterGroup
    {
        public FilterGroup
            (
                [Domain("and, or")]
                [ParameterEditorControl(ParameterControlType.DropDown)]
                string Logic, 
                IEnumerable<Filter> Filters, 
                IEnumerable<FilterGroup> FilterGroups
            )
        {
            this.Logic = Logic;
            this.Filters = Filters;
            this.FilterGroups = FilterGroups;
        }

        public FilterGroup() { }

        public string Logic { get; set; }
        public IEnumerable<Filter> Filters { get; set; }
        public IEnumerable<FilterGroup> FilterGroups { get; set; }
    }
}
