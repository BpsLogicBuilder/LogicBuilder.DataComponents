using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    abstract public class FilterPart
    {
        abstract public Expression Build();
    }
}
