namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class ExceptDescriptor : IExpressionDescriptor
    {
        public ExceptDescriptor()
        {
        }

        public ExceptDescriptor(IExpressionDescriptor left, IExpressionDescriptor right)
        {
            Left = left;
            Right = right;
        }

        public IExpressionDescriptor Left { get; set; }
        public IExpressionDescriptor Right { get; set; }
    }
}
