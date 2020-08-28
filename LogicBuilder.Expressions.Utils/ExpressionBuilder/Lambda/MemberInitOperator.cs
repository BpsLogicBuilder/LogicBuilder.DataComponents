using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class MemberInitOperator : IExpressionPart
    {
        public MemberInitOperator(IDictionary<string, IExpressionPart> memberBindings, Type newType)
        {
            this.MemberBindings = memberBindings;
            NewType = newType;
        }

        public MemberInitOperator(IDictionary<string, IExpressionPart> memberBindings)
        {
            this.MemberBindings = memberBindings;
        }

        public IDictionary<string, IExpressionPart> MemberBindings { get; }
        public Type NewType { get; private set; }

        public Expression Build() 
            => Build
            (
                MemberBindings.Select
                (
                    binding => new { Name = binding.Key, Expression = binding.Value.Build() }
                ).ToDictionary(k => k.Name, v => v.Expression)
            );

        private Expression Build(IDictionary<string, Expression> bindings)
        {
            if (NewType == null)
            {
                NewType = AnonymousTypeFactory.CreateAnonymousType
                (
                    bindings.ToDictionary(k => k.Key, v => v.Value.Type)
                );
            }

            return Expression.MemberInit
            (
                Expression.New(NewType),
                bindings.Select
                (
                    binding => Expression.Bind(NewType.GetMemberInfo(binding.Key), binding.Value)
                )
            );
        }
    }
}
