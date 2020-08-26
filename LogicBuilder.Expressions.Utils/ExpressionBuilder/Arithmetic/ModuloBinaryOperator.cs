namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic
{
    public class ModuloBinaryOperator : BinaryOperator
    {
        public ModuloBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.mod;
    }
}
