namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class AndBinaryOperator : BinaryOperator
    {
        public AndBinaryOperator(FilterPart left, FilterPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.and;
    }
}
