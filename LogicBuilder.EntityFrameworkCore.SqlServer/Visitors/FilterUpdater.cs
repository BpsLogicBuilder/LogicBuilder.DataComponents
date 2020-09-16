using AutoMapper;
using LogicBuilder.Expressions.Utils.Expansions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Visitors
{
    internal class FilterUpdater : ChildCollectionVisitor
    {
        public FilterUpdater(List<ExpansionOptions> expansions, IMapper mapper) : base(expansions, mapper)
        {
        }

        public static Expression UpdaterExpansion(Expression expression, List<ExpansionOptions> expansions, IMapper mapper)
                => new FilterUpdater(expansions, mapper).Visit(expression);

        protected override Expression GetBindingExpression(MemberAssignment binding, ExpansionOptions expansion)
        {
            if (expansion.FilterOption != null)
            {
                return FilterAppender.AppendFilter(binding.Expression, expansion, mapper);
            }
            else if (expansions.Count > 1)  //Mutually exclusive with expansion.Filter != null.                            
            {                               //There can be only one filter in the list.
                return UpdaterExpansion
                (
                    binding.Expression,
                    expansions.Skip(1).ToList(),
                    mapper
                );
            }
            else
                throw new ArgumentException("Last expansion in the list must have a filter", nameof(expansions));
        }
    }
}
