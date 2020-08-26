namespace LogicBuilder.Expressions.Utils.FilterBuilder.Arithmetic
{
    public class AddBinaryOperator : BinaryOperator
    {
        public AddBinaryOperator(FilterPart left, FilterPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.add;
    }
}