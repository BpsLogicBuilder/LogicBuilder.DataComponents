namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic
{
    public class DivideBinaryOperator : BinaryOperator
    {
        public DivideBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.div;
    }
}
