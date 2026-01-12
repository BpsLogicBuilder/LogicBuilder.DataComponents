using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using LogicBuilder.EntityFrameworkCore.SqlServer.Mapping;
using LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Tests
{
    public class CollectionExpressionTests
    {
        public CollectionExpressionTests()
        {
            Initialize();
        }

        #region Fields
        private IServiceProvider serviceProvider;
        private static readonly string parameterName = "$it";
        #endregion Fields

        [Fact]
        public void ConcatDescriptorWorks()
        {
            //act
            var descriptor = new ConcatDescriptor
            (
                new MemberSelectorDescriptor("AlternateAddresses", new ParameterDescriptor(parameterName)),
                new ConstantDescriptor(new Address[] { new Address { City = "Seattle" }, new Address { City = "Portland" } })
            );
            var expression = GetExpression<Product, IEnumerable<Address>>(descriptor);
            var result = RunExpression
            (
                expression,
                new Product { AlternateAddresses = new[] { new Address { City = "Redmond" }, new Address { City = "Seattle" } } }
            );

            AssertExpressionStringIsCorrect(expression, "$it => $it.AlternateAddresses.Concat(LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data.Address[])");
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void ExceptDescriptorWorks()
        {
            //act
            var descriptor = new ExceptDescriptor
            (
                new MemberSelectorDescriptor("AlternateAddresses", new ParameterDescriptor(parameterName)),
                new ConstantDescriptor(new Address[] { new Address { City = "Seattle" }, new Address { City = "Portland" } })
            );
            var expression = GetExpression<Product, IEnumerable<Address>>(descriptor);
            var result = RunExpression
            (
                expression,
                new Product { AlternateAddresses = new[] { new Address { City = "Redmond" }, new Address { City = "Seattle" } } }
            );

            AssertExpressionStringIsCorrect(expression, "$it => $it.AlternateAddresses.Except(LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data.Address[])");
            Assert.Single(result);
            Assert.Equal(new Address { City = "Redmond" }, result.Single());
        }

        [Fact]
        public void UnionDescriptorWorks()
        {
            //act
            var descriptor = new UnionDescriptor
            (
                new MemberSelectorDescriptor("AlternateAddresses", new ParameterDescriptor(parameterName)),
                new ConstantDescriptor(new Address[] { new Address { City = "Seattle" }, new Address { City = "Portland" } })
            );
            var expression = GetExpression<Product, IEnumerable<Address>>(descriptor);
            var result = RunExpression
            (
                expression,
                new Product { AlternateAddresses = new[] { new Address { City = "Redmond" }, new Address { City = "Seattle" } } }
            );

            AssertExpressionStringIsCorrect(expression, "$it => $it.AlternateAddresses.Union(LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data.Address[])");
            Assert.Equal(3, result.Count());
        }

        static MapperConfiguration MapperConfiguration;
        private void Initialize()
        {
            MapperConfiguration ??= ConfigurationHelper.GetMapperConfiguration(cfg =>
            {
                cfg.AddExpressionMapping();
                cfg.AddProfile<ExpressionOperatorsMappingProfile>();
            });

            serviceProvider = new ServiceCollection()
                .AddSingleton<AutoMapper.IConfigurationProvider>
                (
                    MapperConfiguration
                )
                .AddTransient<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService))
                .BuildServiceProvider();
        }

        private static IDictionary<string, ParameterExpression> GetParameters()
            => new Dictionary<string, ParameterExpression>();

        private Expression<Func<T, TResult>> GetExpression<T, TResult>(IExpressionDescriptor filterBody, string parameterName = "$it")
        {
            IMapper mapper = serviceProvider.GetRequiredService<IMapper>();

            return (Expression<Func<T, TResult>>)mapper.Map<SelectorLambdaOperator>
            (
                new SelectorLambdaDescriptor
                (
                    filterBody,
                    typeof(T),
                    typeof(TResult),
                    parameterName
                ),
                opts => opts.Items["parameters"] = GetParameters()
            ).Build();
        }

        private static TResult RunExpression<T, TResult>(Expression<Func<T, TResult>> filter, T instance)
            => filter.Compile().Invoke(instance);

        private static void AssertExpressionStringIsCorrect(Expression expression, string expected)
        {
            AssertStringIsCorrect(ExpressionStringBuilder.ToString(expression));

            void AssertStringIsCorrect(string resultExpression)
                => Assert.True
                (
                    expected == resultExpression,
                    $"Expected expression '{expected}' but the deserializer produced '{resultExpression}'"
                );
        }
    }
}
