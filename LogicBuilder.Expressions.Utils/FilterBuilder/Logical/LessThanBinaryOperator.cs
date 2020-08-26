namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class LessThanBinaryOperator : BinaryOperator
    {
        public LessThanBinaryOperator(FilterPart left, FilterPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.lt;
    }
}
