namespace LogicBuilder.Expressions.Utils.FilterBuilder.Arithmetic
{
    public class DivideBinaryOperator : BinaryOperator
    {
        public DivideBinaryOperator(FilterPart left, FilterPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.div;
    }
}
