namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical
{
    public class AndBinaryOperator : BinaryOperator
    {
        public AndBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.and;
    }
}
