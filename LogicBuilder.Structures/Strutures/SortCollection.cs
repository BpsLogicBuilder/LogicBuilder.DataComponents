using System;
using System.Collections.Generic;
using System.Text;

namespace LogicBuilder.Expressions.Utils.Strutures
{
    public class SortCollection
    {
        public SortCollection() { }
        public SortCollection(ICollection<SortDescription> sortDescriptions)
        {
            this.SortDescriptions = sortDescriptions;
            this.Skip = 0;
            this.Take = int.MaxValue;
        }

        public SortCollection(ICollection<SortDescription> sortDescriptions, int skip, int take)
        {
            this.SortDescriptions = sortDescriptions;
            this.Skip = skip;
            this.Take = take <= 0 ? int.MaxValue : take;
        }

        public ICollection<SortDescription> SortDescriptions { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; } = int.MaxValue;
    }
}
