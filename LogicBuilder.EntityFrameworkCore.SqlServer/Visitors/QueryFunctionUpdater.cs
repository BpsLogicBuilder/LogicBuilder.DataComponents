using AutoMapper;
using LogicBuilder.Expressions.Utils.Expansions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Visitors
{
    internal class QueryFunctionUpdater : ChildCollectionVisitor
    {
        public QueryFunctionUpdater(List<ExpansionOptions> expansions, IMapper mapper) : base(expansions, mapper)
        {
        }

        public static Expression UpdaterExpansion(Expression expression, List<ExpansionOptions> expansions, IMapper mapper)
                => new QueryFunctionUpdater(expansions, mapper).Visit(expression);

        protected override Expression GetBindingExpression(MemberAssignment binding, ExpansionOptions expansion)
        {
            if (expansion.QueryOption != null)
            {
                return QueryFunctionAppender.AppendQueryMethod(binding.Expression, expansion, mapper);
            }
            else if (expansions.Count > 1)  //Mutually exclusive with expansion.QueryOption != null.                            
            {                               //There can be only one set of QueryOptions in the list.
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
