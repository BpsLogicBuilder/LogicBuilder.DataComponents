using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    public class CustomMethodOperator : FilterPart
    {
        public CustomMethodOperator(MethodInfo methodInfo, FilterPart[] args)
        {
            MethodInfo = methodInfo;
            Args = args;
        }

        public MethodInfo MethodInfo { get; }
        public FilterPart[] Args { get; }

        public override Expression Build() => Build(Args.Select(arg => arg.Build()));

        private Expression Build(IEnumerable<Expression> arguments) 
            => MethodInfo.IsStatic
                ? Expression.Call(MethodInfo, arguments.ToArray())
                : Expression.Call(arguments.First(), MethodInfo, arguments.Skip(1).ToArray());
    }
}
