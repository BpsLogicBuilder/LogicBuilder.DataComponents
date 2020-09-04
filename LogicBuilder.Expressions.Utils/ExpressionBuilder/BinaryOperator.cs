using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder
{
    abstract public class BinaryOperator : IExpressionPart
    {
        public BinaryOperator(IExpressionPart left, IExpressionPart right)
        {
            Left = left;
            Right = right;
            BinaryOperatorHandler = new BinaryOperatorHandler(Left, Right, Operator);
        }

        public abstract FilterFunction Operator { get; }
        public IExpressionPart Left { get; }
        public IExpressionPart Right { get; }
        protected virtual BinaryOperatorHandler BinaryOperatorHandler { get; }

        public Expression Build() 
            => BinaryOperatorHandler.Build();
    }
}
