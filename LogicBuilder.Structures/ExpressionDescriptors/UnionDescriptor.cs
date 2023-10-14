namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class UnionDescriptor : IExpressionDescriptor
    {
        public UnionDescriptor()
        {
        }

        public UnionDescriptor(IExpressionDescriptor left, IExpressionDescriptor right)
        {
            Left = left;
            Right = right;
        }

        public IExpressionDescriptor Left { get; set; }
        public IExpressionDescriptor Right { get; set; }
    }
}
