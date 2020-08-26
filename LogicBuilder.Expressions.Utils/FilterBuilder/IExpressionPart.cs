using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    public interface IExpressionPart
    {
        Expression Build();
    }
}
