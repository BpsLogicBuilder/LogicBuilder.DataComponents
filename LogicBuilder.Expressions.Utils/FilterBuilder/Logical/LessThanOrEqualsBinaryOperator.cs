namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class LessThanOrEqualsBinaryOperator : BinaryOperator
    {
        public LessThanOrEqualsBinaryOperator(FilterPart left, FilterPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.le;
    }
}
