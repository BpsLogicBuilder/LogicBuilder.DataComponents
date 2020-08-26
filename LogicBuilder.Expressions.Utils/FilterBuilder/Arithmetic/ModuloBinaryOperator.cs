namespace LogicBuilder.Expressions.Utils.FilterBuilder.Arithmetic
{
    public class ModuloBinaryOperator : BinaryOperator
    {
        public ModuloBinaryOperator(FilterPart left, FilterPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.mod;
    }
}
