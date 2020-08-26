namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class GreaterThanOrEqualsBinaryOperator : BinaryOperator
    {
        public GreaterThanOrEqualsBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.ge;
    }
}
