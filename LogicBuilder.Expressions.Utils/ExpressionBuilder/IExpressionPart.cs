using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder
{
    public interface IExpressionPart
    {
        Expression Build();
    }
}
