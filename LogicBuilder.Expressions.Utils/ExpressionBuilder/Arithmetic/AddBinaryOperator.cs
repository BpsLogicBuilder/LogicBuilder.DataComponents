namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic
{
    public class AddBinaryOperator : BinaryOperator
    {
        public AddBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.add;
    }
}