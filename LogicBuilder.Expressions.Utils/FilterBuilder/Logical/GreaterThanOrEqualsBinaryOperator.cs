namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class GreaterThanOrEqualsBinaryOperator : BinaryOperator
    {
        public GreaterThanOrEqualsBinaryOperator(FilterPart left, FilterPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.ge;
    }
}
