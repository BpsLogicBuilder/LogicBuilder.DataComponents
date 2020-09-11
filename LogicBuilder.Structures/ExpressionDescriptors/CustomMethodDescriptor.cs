using System.Reflection;

namespace LogicBuilder.Expressions.Utils.ExpressionDescriptors
{
    public class CustomMethodDescriptor : IExpressionDescriptor
    {
		public CustomMethodDescriptor()
		{
		}

		public CustomMethodDescriptor(MethodInfo methodInfo, IExpressionDescriptor[] args)
		{
			MethodInfo = methodInfo;
			Args = args;
		}

		public MethodInfo MethodInfo { get; set; }
		public IExpressionDescriptor[] Args { get; set; }
    }
}