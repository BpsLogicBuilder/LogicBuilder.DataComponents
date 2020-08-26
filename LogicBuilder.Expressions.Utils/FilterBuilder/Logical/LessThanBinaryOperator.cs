namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class LessThanBinaryOperator : BinaryOperator
    {
        public LessThanBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.lt;
    }
}
