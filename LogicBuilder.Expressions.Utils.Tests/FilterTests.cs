using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Operand;
using LogicBuilder.Expressions.Utils.Tests.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace LogicBuilder.Expressions.Utils.Tests
{
    public class FilterTests
    {
        private static readonly string parameterName = "$it";

        #region Inequalities
        [Theory]
        [InlineData(null, true)]
        [InlineData("", false)]
        [InlineData("Doritos", false)]
        public void EqualityOperatorWithNull(string productName, bool expected)
        {
            //arrange
            var parameters = GetParameters();

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName == null)");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>() 
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand(null)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("Doritos", true)]
        public void EqualityOperator(string productName, bool expected)
        {
            //arrange
            var parameters = GetParameters();

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName == \"Doritos\")");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>() 
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand("Doritos", typeof(string))
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("Doritos", false)]
        public void NotEqualOperator(string productName, bool expected)
        {
            //arrange
            var parameters = GetParameters();

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName != \"Doritos\")");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>() 
                => GetFilter<T>
                (
                    new NotEqualsBinaryOperator
                    (
                        new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand("Doritos", typeof(string))
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(5.01, true)]
        [InlineData(4.99, false)]
        public void GreaterThanOperator(object unitPrice, bool expected)
        {
            //arrange
            var parameters = GetParameters();

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = ToNullable<decimal>(unitPrice) });
            
            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => ($it.UnitPrice > Convert({0:0.00}))", 5.0));
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>() 
                => GetFilter<T>
                (
                    new GreaterThanBinaryOperator
                    (
                        new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand(5.00m, typeof(decimal))
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(5.0, true)]
        [InlineData(4.99, false)]
        public void GreaterThanEqualOperator(object unitPrice, bool expected)
        {
            //arrange
            var parameters = GetParameters();

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = ToNullable<decimal>(unitPrice) });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => ($it.UnitPrice >= Convert({0:0.00}))", 5.0));
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>() 
                => GetFilter<T>
                (
                    new GreaterThanOrEqualsBinaryOperator
                    (
                        new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand(5.00m, typeof(decimal))
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(4.99, true)]
        [InlineData(5.01, false)]
        public void LessThanOperator(object unitPrice, bool expected)
        {
            //arrange
            var parameters = GetParameters();

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = ToNullable<decimal>(unitPrice) });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => ($it.UnitPrice < Convert({0:0.00}))", 5.0));
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>() 
                => GetFilter<T>
                (
                    new LessThanBinaryOperator
                    (
                        new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand(5.00m, typeof(decimal))
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(5.0, true)]
        [InlineData(5.01, false)]
        public void LessThanOrEqualOperator(object unitPrice, bool expected)
        {
            //arrange
            var parameters = GetParameters();

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = ToNullable<decimal>(unitPrice) });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => ($it.UnitPrice <= Convert({0:0.00}))", 5.0));
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>() 
                => GetFilter<T>
                (
                    new LessThanOrEqualsBinaryOperator
                    (
                        new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand(5.00m, typeof(decimal))
                    ),
                    parameters
                );
        }

        [Fact]
        public void NegativeNumbers()
        {
            //arrange
            var parameters = GetParameters();

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = ToNullable<decimal>(44m) });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => ($it.UnitPrice <= Convert({0:0.00}))", -5.0));
            Assert.False(result);

            Expression<Func<T, bool>> CreateFilter<T>() 
                => GetFilter<T>
                (
                    new LessThanOrEqualsBinaryOperator
                    (
                        new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand(-5.00m, typeof(decimal))
                    ),
                    parameters
                );
        }

        private class DateTimeOffsetInequalities_Class : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object []
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp == $it.DateTimeOffsetProp)",
                        parameters
                    },
                    new object []
                    {
                        new NotEqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp != $it.DateTimeOffsetProp)",
                        parameters
                    },
                    new object []
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp >= $it.DateTimeOffsetProp)",
                        parameters
                    },
                    new object []
                    {
                        new LessThanOrEqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp <= $it.DateTimeOffsetProp)",
                        parameters
                    }
                }.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public static List<object[]> DateTimeOffsetInequalities_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object []
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp == $it.DateTimeOffsetProp)",
                        parameters
                    },
                    new object []
                    {
                        new NotEqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp != $it.DateTimeOffsetProp)",
                        parameters
                    },
                    new object []
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp >= $it.DateTimeOffsetProp)",
                        parameters
                    },
                    new object []
                    {
                        new LessThanOrEqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp <= $it.DateTimeOffsetProp)",
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateTimeOffsetInequalities_Data))]
        public void DateTimeOffsetInequalities(IExpressionPart filterBody, string expectedExpression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter, expectedExpression);

            Expression<Func<T, bool>> CreateFilter<T>()
            {
                return GetFilter<T>
                (
                    filterBody,
                    parameters
                );
            }
        }
        #endregion Inequalities

        private T? ToNullable<T>(object value) where T : struct => 
            value == null ? null : (T?)Convert.ChangeType(value, typeof(T));

        private static IDictionary<string, ParameterExpression> GetParameters()
            => new Dictionary<string, ParameterExpression>();

        private Expression<Func<T, bool>> GetFilter<T>(IExpressionPart filterBody, IDictionary<string, ParameterExpression> parameters, string parameterName = "$it") 
            => filterBody.GetFilter<T>(parameters, parameterName);

        private void AssertFilterStringIsCorrect(Expression expression, string expected)
        {
            string resultExpression = ExpressionStringBuilder.ToString(expression);
            Assert.True(expected == resultExpression, string.Format("Expected expression '{0}' but the deserializer produced '{1}'", expected, resultExpression));
        }

        private bool RunFilter<TModel>(Expression<Func<TModel, bool>> filter, TModel instance)
            => filter.Compile().Invoke(instance);
    }
}
