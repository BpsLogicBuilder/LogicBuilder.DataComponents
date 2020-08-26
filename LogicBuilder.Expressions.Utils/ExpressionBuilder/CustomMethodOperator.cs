using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder
{
    public class CustomMethodOperator : IExpressionPart
    {
        public CustomMethodOperator(MethodInfo methodInfo, IExpressionPart[] args)
        {
            MethodInfo = methodInfo;
            Args = args;
        }

        public MethodInfo MethodInfo { get; }
        public IExpressionPart[] Args { get; }

        public Expression Build() => Build(Args.Select(arg => arg.Build()));

        private Expression Build(IEnumerable<Expression> arguments) 
            => MethodInfo.IsStatic
                ? Expression.Call(MethodInfo, arguments.ToArray())
                : Expression.Call(arguments.First(), MethodInfo, arguments.Skip(1).ToArray());
    }
}
