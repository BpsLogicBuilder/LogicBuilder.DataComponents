namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical
{
    public class EqualsBinaryOperator : BinaryOperator
    {
        public EqualsBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
            BinaryOperatorHandler = new EqualsBinaryOperatorHandler(Left, Right, Operator);
        }

        protected override BinaryOperatorHandler BinaryOperatorHandler { get; }

        public override FilterFunction Operator => FilterFunction.eq;
    }
}
