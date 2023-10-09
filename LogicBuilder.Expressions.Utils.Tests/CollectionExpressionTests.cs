using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Cacnonical;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Collection;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Operand;
using LogicBuilder.Expressions.Utils.Tests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace LogicBuilder.Expressions.Utils.Tests
{
    public class CollectionExpressionTests
    {
        public CollectionExpressionTests()
        {
            parameters = new Dictionary<string, ParameterExpression>();
        }

        private static readonly string parameterName = "$it";
        private readonly IDictionary<string, ParameterExpression> parameters;

        [Fact]
        public void ConcatOperatorWorks()
        {
            //act
            var expression = CreateExpression<Product>();

            var result = RunExpression
            (
                expression,
                new Product { AlternateAddresses = new[] { new Address { City = "Redmond" }, new Address { City = "Seattle" } } }
            );

            AssertExpressionStringIsCorrect(expression, "$it => $it.AlternateAddresses.Concat(LogicBuilder.Expressions.Utils.Tests.Data.Address[])");
            Assert.Equal(4, result.Count());

            Expression<Func<T, IEnumerable<Address>>> CreateExpression<T>()
                => LinqHelpers.GetExpression<T, IEnumerable<Address>>
                (
                    new ConcatOperator
                    (
                        new MemberSelectorOperator("AlternateAddresses", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator
                        (
                            new Address[] { new Address { City = "Seattle" }, new Address { City = "Portland" } }
                        )
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void ExceptOperatorWorks()
        {
            //act
            var expression = CreateExpression<Product>();

            var result = RunExpression
            (
                expression,
                new Product { AlternateAddresses = new[] { new Address { City = "Redmond" }, new Address { City = "Seattle" } } }
            );

            AssertExpressionStringIsCorrect(expression, "$it => $it.AlternateAddresses.Except(LogicBuilder.Expressions.Utils.Tests.Data.Address[])");
            Assert.Single(result);
            Assert.Equal(new Address { City = "Redmond" }, result.Single());

            Expression<Func<T, IEnumerable<Address>>> CreateExpression<T>()
                => LinqHelpers.GetExpression<T, IEnumerable<Address>>
                (
                    new ExceptOperator
                    (
                        new MemberSelectorOperator("AlternateAddresses", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator
                        (
                            new Address[] { new Address { City = "Seattle" }, new Address { City = "Portland" } }
                        )
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void UnionOperatorWorks()
        {
            //act
            var expression = CreateExpression<Product>();

            var result = RunExpression
            (
                expression,
                new Product { AlternateAddresses = new[] { new Address { City = "Redmond" }, new Address { City = "Seattle" } } }
            );

            AssertExpressionStringIsCorrect(expression, "$it => $it.AlternateAddresses.Union(LogicBuilder.Expressions.Utils.Tests.Data.Address[])");
            Assert.Equal(3, result.Count());

            Expression<Func<T, IEnumerable<Address>>> CreateExpression<T>()
                => LinqHelpers.GetExpression<T, IEnumerable<Address>>
                (
                    new UnionOperator
                    (
                        new MemberSelectorOperator("AlternateAddresses", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator
                        (
                            new Address[] { new Address { City = "Seattle" }, new Address { City = "Portland" } }
                        )
                    ),
                    parameters,
                    parameterName
                );
        }

        private static void AssertExpressionStringIsCorrect(Expression expression, string expected)
        {
            string resultExpression = ExpressionStringBuilder.ToString(expression);
            Assert.True(expected == resultExpression, string.Format("Expected expression '{0}' but the deserializer produced '{1}'", expected, resultExpression));
        }

        private static TResult RunExpression<TModel, TResult>(Expression<Func<TModel, TResult>> expression, TModel instance)
            => expression.Compile().Invoke(instance);
    }
}
