using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Cacnonical;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Conversions;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.DateTimeOperators;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Operand;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.StringOperators;
using LogicBuilder.Expressions.Utils.Tests.Data;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace LogicBuilder.Expressions.Utils.Tests
{
    public class FilterTests
    {
        public FilterTests()
        {
            parameters = GetParameters();
        }

        private static readonly string parameterName = "$it";
        private IDictionary<string, ParameterExpression> parameters;

        #region Inequalities
        [Theory]
        [InlineData(null, true)]
        [InlineData("", false)]
        [InlineData("Doritos", false)]
        public void EqualityOperatorWithNull(string productName, bool expected)
        {
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

        public static List<object[]> DateTimeOffsetInequalities_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp == $it.DateTimeOffsetProp)",
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp != $it.DateTimeOffsetProp)",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp >= $it.DateTimeOffsetProp)",
                        parameters
                    },
                    new object[]
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

        public static List<object[]> DateInEqualities_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeProp == $it.DateTimeProp)",
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeProp != $it.DateTimeProp)",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeProp >= $it.DateTimeProp)",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanOrEqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("DateTimeProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeProp <= $it.DateTimeProp)",
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateInEqualities_Data))]
        public void DateInEqualities(IExpressionPart filterBody, string expectedExpression, IDictionary<string, ParameterExpression> parameters)
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

        #region Logical Operators
        [Fact]
        public void BooleanOperatorNullableTypes()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => (($it.UnitPrice == Convert(5.00)) OrElse ($it.CategoryID == 0))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new OrBinaryOperator
                    (
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(5.00m, typeof(decimal))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("CategoryID", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(0, typeof(int))
                        )
                    ),
                    parameters
                );
        }

        [Fact]
        public void BooleanComparisonOnNullableAndNonNullableType()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.Discontinued == Convert(True))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelector("Discontinued", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand(true, typeof(bool))
                    ),
                    parameters
                );
        }

        [Fact]
        public void BooleanComparisonOnNullableType()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.Discontinued == $it.Discontinued)");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelector("Discontinued", new ParameterOperator(parameters, parameterName)),
                        new MemberSelector("Discontinued", new ParameterOperator(parameters, parameterName))
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData(5.0, 0, true)]
        [InlineData(null, 1, false)]
        public void OrOperator(object unitPrice, object unitsInStock, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = ToNullable<decimal>(unitPrice), UnitsInStock = ToNullable<short>(unitsInStock) });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => (($it.UnitPrice == Convert({0:0.00})) OrElse (Convert($it.UnitsInStock) == Convert({1})))", 5.0, 0));
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new OrBinaryOperator
                    (
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(5.00m, typeof(decimal))
                        ),
                        new EqualsBinaryOperator
                        (
                            new ConvertOperand(new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)), typeof(int?)),
                            new ConstantOperand(0, typeof(int))
                        )
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData(5.0, 10, true)]
        [InlineData(null, 1, false)]
        public void AndOperator(object unitPrice, object unitsInStock, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = ToNullable<decimal>(unitPrice), UnitsInStock = ToNullable<short>(unitsInStock) });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => (($it.UnitPrice == Convert({0:0.00})) AndAlso (Convert($it.UnitsInStock) == Convert({1:0.00})))", 5.0, 10.0));
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AndBinaryOperator
                    (
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(5.00m, typeof(decimal))
                        ),
                        new EqualsBinaryOperator
                        (
                            new ConvertOperand(new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)), typeof(decimal?)),
                            new ConstantOperand(10.00m, typeof(decimal))
                        )
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(5.0, false)]
        [InlineData(5.5, true)]
        public void Negation(object unitPrice, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = ToNullable<decimal>(unitPrice) });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => Not(($it.UnitPrice == Convert({0:0.00})))", 5.0));
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new NotOperator
                    (
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(5.00m, typeof(decimal))
                        )
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void BoolNegation(bool discontinued, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { Discontinued = discontinued });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => Convert(Not($it.Discontinued))");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new NotOperator
                    (
                        new MemberSelector("Discontinued", new ParameterOperator(parameters, parameterName))
                    ),
                    parameters
                );
        }

        [Fact]
        public void NestedNegation()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => Convert(Not(Not(Not($it.Discontinued))))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new NotOperator
                    (
                        new NotOperator
                        (
                            new NotOperator
                            (
                                new MemberSelector("Discontinued", new ParameterOperator(parameters, parameterName))
                            )
                        )
                    ),
                    parameters
                );
        }
        #endregion Logical Operators

        #region Arithmetic Operators
        [Theory]
        [InlineData(null, false)]
        [InlineData(5.0, true)]
        [InlineData(15.01, false)]
        public void Subtraction(object unitPrice, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = ToNullable<decimal>(unitPrice) });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => (($it.UnitPrice - Convert({0:0.00})) < Convert({1:0.00}))", 1.0, 5.0));
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new LessThanBinaryOperator
                    (
                        new SubtractBinaryOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(1.00m, typeof(decimal))
                        ),
                        new ConstantOperand(5.00m, typeof(decimal))
                    ),
                    parameters
                );
        }

        [Fact]
        public void Addition()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => (($it.UnitPrice + Convert({0:0.00})) < Convert({1:0.00}))", 1.0, 5.0));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new LessThanBinaryOperator
                    (
                        new AddBinaryOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(1.00m, typeof(decimal))
                        ),
                        new ConstantOperand(5.00m, typeof(decimal))
                    ),
                    parameters
                );
        }

        [Fact]
        public void Multiplication()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => (($it.UnitPrice * Convert({0:0.00})) < Convert({1:0.00}))", 1.0, 5.0));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new LessThanBinaryOperator
                    (
                        new MultiplyBinaryOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(1.00m, typeof(decimal))
                        ),
                        new ConstantOperand(5.00m, typeof(decimal))
                    ),
                    parameters
                );
        }

        [Fact]
        public void Division()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => (($it.UnitPrice / Convert({0:0.00})) < Convert({1:0.00}))", 1.0, 5.0));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new LessThanBinaryOperator
                    (
                        new DivideBinaryOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(1.00m, typeof(decimal))
                        ),
                        new ConstantOperand(5.00m, typeof(decimal))
                    ),
                    parameters
                );
        }

        [Fact]
        public void Modulo()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => (($it.UnitPrice % Convert({0:0.00})) < Convert({1:0.00}))", 1.0, 5.0));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new LessThanBinaryOperator
                    (
                        new ModuloBinaryOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(1.00m, typeof(decimal))
                        ),
                        new ConstantOperand(5.00m, typeof(decimal))
                    ),
                    parameters
                );
        }
        #endregion Arithmetic Operators

        #region NULL  handling
        public static List<object[]> NullHandling_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                        ),
                        null,
                        null,
                        true,
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                        ),
                        null,
                        null,
                        false,
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanBinaryOperator
                        (
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                        ),
                        null,
                        null,
                        false,
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                        ),
                        null,
                        null,
                        false,
                        parameters
                    },
                    new object[]
                    {
                        new LessThanBinaryOperator
                        (
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                        ),
                        null,
                        null,
                        false,
                        parameters
                    },
                    new object[]
                    {
                        new LessThanOrEqualsBinaryOperator
                        (
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                        ),
                        null,
                        null,
                        false,
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new AddBinaryOperator
                            (
                                new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                                new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                            ),
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName))
                        ),
                        null,
                        null,
                        true,
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new SubtractBinaryOperator
                            (
                                new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                                new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                            ),
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName))
                        ),
                        null,
                        null,
                        true,
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MultiplyBinaryOperator
                            (
                                new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                                new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                            ),
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName))
                        ),
                        null,
                        null,
                        true,
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new DivideBinaryOperator
                            (
                                new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                                new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                            ),
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName))
                        ),
                        null,
                        null,
                        true,
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new ModuloBinaryOperator
                            (
                                new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                                new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                            ),
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName))
                        ),
                        null,
                        null,
                        true,
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                        ),
                        1,
                        null,
                        false,
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelector("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                        ),
                        1,
                        1,
                        true,
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(NullHandling_Data))]
        public void NullHandling(IExpressionPart filterBody, object unitsInStock, object unitsOnOrder, bool expected, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitsInStock = ToNullable<short>(unitsInStock), UnitsOnOrder = ToNullable<short>(unitsOnOrder) });

            //assert
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> NullHandling_LiteralNull_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(null)
                        ),
                        null,
                        true,
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new MemberSelector("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(null)
                        ),
                        null,
                        false,
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(NullHandling_LiteralNull_Data))]
        public void NullHandling_LiteralNull(IExpressionPart filterBody, object unitsInStock, bool expected, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitsInStock = ToNullable<short>(unitsInStock) });

            //assert
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }
        #endregion NULL  handling

        public static List<object[]> ComparisonsInvolvingCastsAndNullableValues_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new GreaterThanBinaryOperator
                        (
                            new IndexOfOperator
                            (
                                new ConstantOperand("hello"),
                                new MemberSelector("StringProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertOperand
                            (
                                new MemberSelector("UIntProp", new ParameterOperator(parameters, parameterName)),
                                typeof(int?)
                            )
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanBinaryOperator
                        (
                            new IndexOfOperator
                            (
                                new ConstantOperand("hello"),
                                new MemberSelector("StringProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertOperand
                            (
                                new MemberSelector("ULongProp", new ParameterOperator(parameters, parameterName)),
                                typeof(int?)
                            )
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanBinaryOperator
                        (
                            new IndexOfOperator
                            (
                                new ConstantOperand("hello"),
                                new MemberSelector("StringProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertOperand
                            (
                                new MemberSelector("UShortProp", new ParameterOperator(parameters, parameterName)),
                                typeof(int?)
                            )
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanBinaryOperator
                        (
                            new IndexOfOperator
                            (
                                new ConstantOperand("hello"),
                                new MemberSelector("StringProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertOperand
                            (
                                new MemberSelector("NullableUShortProp", new ParameterOperator(parameters, parameterName)),
                                typeof(int?)
                            )
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanBinaryOperator
                        (
                            new IndexOfOperator
                            (
                                new ConstantOperand("hello"),
                                new MemberSelector("StringProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertOperand
                            (
                                new MemberSelector("NullableUIntProp", new ParameterOperator(parameters, parameterName)),
                                typeof(int?)
                            )
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanBinaryOperator
                        (
                            new IndexOfOperator
                            (
                                new ConstantOperand("hello"),
                                new MemberSelector("StringProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertOperand
                            (
                                new MemberSelector("NullableULongProp", new ParameterOperator(parameters, parameterName)),
                                typeof(int?)
                            )
                        ),
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(ComparisonsInvolvingCastsAndNullableValues_Data))]
        public void ComparisonsInvolvingCastsAndNullableValues(IExpressionPart filterBody, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<DataTypes>();

            //assert
            Assert.Throws<ArgumentNullException>(() => RunFilter(filter, new DataTypes()));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        [Theory]
        [InlineData(null, null, true)]
        [InlineData("not doritos", 0, true)]
        [InlineData("Doritos", 1, false)]
        public void Grouping(string productName, object unitsInStock, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName, UnitsInStock = ToNullable<short>(unitsInStock) });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => (($it.ProductName != \"Doritos\") OrElse ($it.UnitPrice < Convert({0:0.00})))", 5.0));
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new OrBinaryOperator
                    (
                        new NotEqualsBinaryOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand("Doritos")
                        ),
                        new LessThanBinaryOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(5.00m, typeof(decimal))
                        )
                    ),
                    parameters
                );
        }

        [Fact]
        public void MemberExpressions()
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { Category = new Category { CategoryName = "Snacks" } });

            //assert
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product { }));
            AssertFilterStringIsCorrect(filter, "$it => ($it.Category.CategoryName == \"Snacks\")");
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelector
                        (
                            "CategoryName",
                            new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand("Snacks")
                    ),
                    parameters
                );
        }

        [Fact]
        public void MemberExpressionsRecursive()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product { }));
            AssertFilterStringIsCorrect(filter, "$it => ($it.Category.Product.Category.CategoryName == \"Snacks\")");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelector
                        (
                            "CategoryName",
                            new MemberSelector
                            (
                                "Category",
                                new MemberSelector
                                (
                                    "Product",
                                    new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                                )
                            )
                        ),
                        new ConstantOperand("Snacks")
                    ),
                    parameters
                );
        }

        [Fact]
        public void ComplexPropertyNavigation()
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { SupplierAddress = new Address { City = "Redmond" } });

            //assert
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product { }));
            AssertFilterStringIsCorrect(filter, "$it => ($it.SupplierAddress.City == \"Redmond\")");
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelector
                        (
                            "City",
                            new MemberSelector("SupplierAddress", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand("Redmond")
                    ),
                    parameters
                );
        }

        #region Any/All
        [Fact]
        public void AnyOnNavigationEnumerableCollections()
        {
            //act
            var filter = CreateFilter<Product>();

            bool result1 = RunFilter
            (
                filter,
                new Product
                {
                    Category = new Category
                    {
                        EnumerableProducts = new Product[]
                        {
                            new Product { ProductName = "Snacks" },
                            new Product { ProductName = "NonSnacks" }
                        }
                    }
                }
            );

            bool result2 = RunFilter
            (
                filter,
                new Product
                {
                    Category = new Category
                    {
                        EnumerableProducts = new Product[]
                        {
                            new Product { ProductName = "NonSnacks" }
                        }
                    }
                }
            );

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.Category.EnumerableProducts.Any(P => (P.ProductName == \"Snacks\"))");
            Assert.True(result1);
            Assert.False(result2);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AnyOperator
                    (
                        parameters,
                        new MemberSelector
                        (
                            "EnumerableProducts",
                            new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, "P")),
                            new ConstantOperand("Snacks")
                        ),
                        "P"
                    ),
                    parameters
                );
        }

        [Fact]
        public void AnyOnNavigationQueryableCollections()
        {
            //act
            var filter = CreateFilter<Product>();

            bool result1 = RunFilter
            (
                filter,
                new Product
                {
                    Category = new Category
                    {
                        QueryableProducts = new Product[]
                        {
                            new Product { ProductName = "Snacks" },
                            new Product { ProductName = "NonSnacks" }
                        }.AsQueryable()
                    }
                }
            );

            bool result2 = RunFilter
            (
                filter,
                new Product
                {
                    Category = new Category
                    {
                        QueryableProducts = new Product[]
                        {
                            new Product { ProductName = "NonSnacks" }
                        }.AsQueryable()
                    }
                }
            );

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.Category.QueryableProducts.Any(P => (P.ProductName == \"Snacks\"))");
            Assert.True(result1);
            Assert.False(result2);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AnyOperator
                    (
                        parameters,
                        new MemberSelector
                        (
                            "QueryableProducts",
                            new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, "P")),
                            new ConstantOperand("Snacks")
                        ),
                        "P"
                    ),
                    parameters
                );
        }

        public static List<object[]> AnyInOnNavigation_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "QueryableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelector("ProductID", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperand
                                (
                                    typeof(int),
                                    new List<object>{ 1 }
                                )
                            ),
                            "P"
                        ),
                        "$it => $it.Category.QueryableProducts.Any(P => System.Collections.Generic.List`1[System.Int32].Contains(P.ProductID))",
                        parameters
                    },
                    new object[]
                    {
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "EnumerableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelector("ProductID", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperand
                                (
                                    typeof(int),
                                    new List<object>{ 1 }
                                )
                            ),
                            "P"
                        ),
                        "$it => $it.Category.EnumerableProducts.Any(P => System.Collections.Generic.List`1[System.Int32].Contains(P.ProductID))",
                        parameters
                    },
                    new object[]
                    {
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "QueryableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelector("GuidProperty", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperand
                                (
                                    typeof(Guid),
                                    new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") }
                                )
                            ),
                            "P"
                        ),
                        "$it => $it.Category.QueryableProducts.Any(P => System.Collections.Generic.List`1[System.Guid].Contains(P.GuidProperty))",
                        parameters
                    },
                    new object[]
                    {
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "EnumerableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelector("GuidProperty", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperand
                                (
                                    typeof(Guid),
                                    new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") }
                                )
                            ),
                            "P"
                        ),
                       "$it => $it.Category.EnumerableProducts.Any(P => System.Collections.Generic.List`1[System.Guid].Contains(P.GuidProperty))",
                        parameters
                    },
                    new object[]
                    {
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "QueryableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelector("NullableGuidProperty", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperand
                                (
                                    typeof(Guid?),
                                    new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") }
                                )
                            ),
                            "P"
                        ),
                        "$it => $it.Category.QueryableProducts.Any(P => System.Collections.Generic.List`1[System.Nullable`1[System.Guid]].Contains(P.NullableGuidProperty))",
                        parameters
                    },
                    new object[]
                    {
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "EnumerableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelector("NullableGuidProperty", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperand
                                (
                                    typeof(Guid?),
                                    new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") }
                                )
                            ),
                            "P"
                        ),
                       "$it => $it.Category.EnumerableProducts.Any(P => System.Collections.Generic.List`1[System.Nullable`1[System.Guid]].Contains(P.NullableGuidProperty))",
                        parameters
                    },
                    new object[]
                    {
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "QueryableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelector("Discontinued", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperand
                                (
                                    typeof(bool?),
                                    new List<object>{ false, null }
                                )
                            ),
                            "P"
                        ),
                        "$it => $it.Category.QueryableProducts.Any(P => System.Collections.Generic.List`1[System.Nullable`1[System.Boolean]].Contains(P.Discontinued))",
                        parameters
                    },
                    new object[]
                    {
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "EnumerableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelector("Discontinued", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperand
                                (
                                    typeof(bool?),
                                    new List<object>{ false, null }
                                )
                            ),
                            "P"
                        ),
                       "$it => $it.Category.EnumerableProducts.Any(P => System.Collections.Generic.List`1[System.Nullable`1[System.Boolean]].Contains(P.Discontinued))",
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(AnyInOnNavigation_Data))]
        public void AnyInOnNavigation(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> AnyOnNavigation_Contradiction_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "QueryableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(false),
                            "P"
                        ),
                       "$it => $it.Category.QueryableProducts.Any(P => False)",
                        parameters
                    },
                    new object[]
                    {
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "QueryableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new AndBinaryOperator
                            (
                                new ConstantOperand(false),
                                new EqualsBinaryOperator
                                (
                                    new MemberSelector("ProductName", new ParameterOperator(parameters, "P")),
                                    new ConstantOperand("Snacks")
                                )
                            ),
                            "P"
                        ),
                       "$it => $it.Category.QueryableProducts.Any(P => (False AndAlso (P.ProductName == \"Snacks\")))",
                        parameters
                    },
                    new object[]
                    {
                        new AnyOperator
                        (
                            new MemberSelector
                            (
                                "QueryableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                       "$it => $it.Category.QueryableProducts.Any()",
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(AnyOnNavigation_Contradiction_Data))]
        public void AnyOnNavigation_Contradiction(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        [Fact]
        public void AnyOnNavigation_NullCollection()
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter
            (
                filter,
                new Product
                {
                    Category = new Category
                    {
                        EnumerableProducts = new Product[]
                        {
                            new Product { ProductName = "Snacks" }
                        }
                    }
                }
            );

            //assert
            Assert.Throws<ArgumentNullException>(() => RunFilter(filter, new Product { Category = new Category { } }));
            AssertFilterStringIsCorrect(filter, "$it => $it.Category.EnumerableProducts.Any(P => (P.ProductName == \"Snacks\"))");
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AnyOperator
                    (
                        parameters,
                        new MemberSelector
                        (
                            "EnumerableProducts",
                            new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, "P")),
                            new ConstantOperand("Snacks")
                        ),
                        "P"
                    ),
                    parameters
                );
        }


        [Fact]
        public void AllOnNavigation_NullCollection()
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter
            (
                filter,
                new Product
                {
                    Category = new Category
                    {
                        EnumerableProducts = new Product[]
                        {
                            new Product { ProductName = "Snacks" }
                        }
                    }
                }
            );

            //assert
            Assert.Throws<ArgumentNullException>(() => RunFilter(filter, new Product { Category = new Category { } }));
            AssertFilterStringIsCorrect(filter, "$it => $it.Category.EnumerableProducts.All(P => (P.ProductName == \"Snacks\"))");
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AllOperator
                    (
                        parameters,
                        new MemberSelector
                        (
                            "EnumerableProducts",
                            new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, "P")),
                            new ConstantOperand("Snacks")
                        ),
                        "P"
                    ),
                    parameters
                );
        }

        [Fact]
        public void MultipleAnys_WithSameRangeVariableName()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.AlternateIDs.Any(n => (n == 42)) AndAlso $it.AlternateAddresses.Any(n => (n.City == \"Redmond\")))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AndBinaryOperator
                    (
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector("AlternateIDs", new ParameterOperator(parameters, parameterName)),
                            new EqualsBinaryOperator
                            (
                                new ParameterOperator(parameters, "n"),
                                new ConstantOperand(42)
                            ),
                            "n"
                        ),
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector("AlternateAddresses", new ParameterOperator(parameters, parameterName)),
                            new EqualsBinaryOperator
                            (
                                new MemberSelector("City", new ParameterOperator(parameters, "n")),
                                new ConstantOperand("Redmond")
                            ),
                            "n"
                        )
                    ),
                    parameters
                );
        }

        [Fact]
        public void MultipleAlls_WithSameRangeVariableName()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.AlternateIDs.All(n => (n == 42)) AndAlso $it.AlternateAddresses.All(n => (n.City == \"Redmond\")))");
            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AndBinaryOperator
                    (
                        new AllOperator
                        (
                            parameters,
                            new MemberSelector("AlternateIDs", new ParameterOperator(parameters, parameterName)),
                            new EqualsBinaryOperator
                            (
                                new ParameterOperator(parameters, "n"),
                                new ConstantOperand(42)
                            ),
                            "n"
                        ),
                        new AllOperator
                        (
                            parameters,
                            new MemberSelector("AlternateAddresses", new ParameterOperator(parameters, parameterName)),
                            new EqualsBinaryOperator
                            (
                                new MemberSelector("City", new ParameterOperator(parameters, "n")),
                                new ConstantOperand("Redmond")
                            ),
                            "n"
                        )
                    ),
                    parameters
                );
        }

        [Fact]
        public void AnyOnNavigationEnumerableCollections_EmptyFilter()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.Category.EnumerableProducts.Any()");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AnyOperator
                    (
                        new MemberSelector
                        (
                            "EnumerableProducts",
                            new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                        )
                    ),
                    parameters
                );
        }

        [Fact]
        public void AnyOnNavigationQueryableCollections_EmptyFilter()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.Category.QueryableProducts.Any()");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AnyOperator
                    (
                        new MemberSelector
                        (
                            "QueryableProducts",
                            new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                        )
                    ),
                    parameters
                );
        }

        [Fact]
        public void AllOnNavigationEnumerableCollections()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.Category.EnumerableProducts.All(P => (P.ProductName == \"Snacks\"))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AllOperator
                    (
                        parameters,
                        new MemberSelector
                        (
                            "EnumerableProducts",
                            new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, "P")),
                            new ConstantOperand("Snacks")
                        ),
                        "P"
                    ),
                    parameters
                );
        }

        [Fact]
        public void AllOnNavigationQueryableCollections()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.Category.QueryableProducts.All(P => (P.ProductName == \"Snacks\"))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AllOperator
                    (
                        parameters,
                        new MemberSelector
                        (
                            "QueryableProducts",
                            new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, "P")),
                            new ConstantOperand("Snacks")
                        ),
                        "P"
                    ),
                    parameters
                );
        }

        [Fact]
        public void AnyInSequenceNotNested()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.Category.QueryableProducts.Any(P => (P.ProductName == \"Snacks\")) OrElse $it.Category.QueryableProducts.Any(P2 => (P2.ProductName == \"Snacks\")))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new OrBinaryOperator
                    (
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "QueryableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new EqualsBinaryOperator
                            (
                                new MemberSelector("ProductName", new ParameterOperator(parameters, "P")),
                                new ConstantOperand("Snacks")
                            ),
                            "P"
                        ),
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "QueryableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new EqualsBinaryOperator
                            (
                                new MemberSelector("ProductName", new ParameterOperator(parameters, "P2")),
                                new ConstantOperand("Snacks")
                            ),
                            "P2"
                        )
                    ),
                    parameters
                );
        }

        [Fact]
        public void AllInSequenceNotNested()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.Category.QueryableProducts.All(P => (P.ProductName == \"Snacks\")) OrElse $it.Category.QueryableProducts.All(P2 => (P2.ProductName == \"Snacks\")))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new OrBinaryOperator
                    (
                        new AllOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "QueryableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new EqualsBinaryOperator
                            (
                                new MemberSelector("ProductName", new ParameterOperator(parameters, "P")),
                                new ConstantOperand("Snacks")
                            ),
                            "P"
                        ),
                        new AllOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "QueryableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new EqualsBinaryOperator
                            (
                                new MemberSelector("ProductName", new ParameterOperator(parameters, "P2")),
                                new ConstantOperand("Snacks")
                            ),
                            "P2"
                        )
                    ),
                    parameters
                );
        }

        [Fact]
        public void AnyOnPrimitiveCollection()
        {
            //act
            var filter = CreateFilter<Product>();

            bool result1 = RunFilter
            (
                filter,
                new Product { AlternateIDs = new[] { 1, 2, 42 } }
            );

            bool result2 = RunFilter
            (
                filter,
                new Product { AlternateIDs = new[] { 1, 2 } }
            );

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.AlternateIDs.Any(id => (id == 42))");
            Assert.True(result1);
            Assert.False(result2);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AnyOperator
                    (
                        parameters,
                        new MemberSelector("AlternateIDs", new ParameterOperator(parameters, parameterName)),
                        new EqualsBinaryOperator
                        (
                            new ParameterOperator(parameters, "id"),
                            new ConstantOperand(42)
                        ),
                        "id"
                    ),
                    parameters
                );
        }

        [Fact]
        public void AllOnPrimitiveCollection()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.AlternateIDs.All(id => (id == 42))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AllOperator
                    (
                        parameters,
                        new MemberSelector("AlternateIDs", new ParameterOperator(parameters, parameterName)),
                        new EqualsBinaryOperator
                        (
                            new ParameterOperator(parameters, "id"),
                            new ConstantOperand(42)
                        ),
                        "id"
                    ),
                    parameters
                );
        }

        [Fact]
        public void AnyOnComplexCollection()
        {
            //act
            var filter = CreateFilter<Product>();

            bool result = RunFilter
            (
                filter,
                new Product { AlternateAddresses = new[] { new Address { City = "Redmond" } } }
            );

            //assert
            Assert.Throws<ArgumentNullException>(() => RunFilter(filter, new Product { }));
            AssertFilterStringIsCorrect(filter, "$it => $it.AlternateAddresses.Any(address => (address.City == \"Redmond\"))");
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AnyOperator
                    (
                        parameters,
                        new MemberSelector("AlternateAddresses", new ParameterOperator(parameters, parameterName)),
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("City", new ParameterOperator(parameters, "address")),
                            new ConstantOperand("Redmond")
                        ),
                        "address"
                    ),
                    parameters
                );
        }

        [Fact]
        public void AllOnComplexCollection()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.AlternateAddresses.All(address => (address.City == \"Redmond\"))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AllOperator
                    (
                        parameters,
                        new MemberSelector("AlternateAddresses", new ParameterOperator(parameters, parameterName)),
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("City", new ParameterOperator(parameters, "address")),
                            new ConstantOperand("Redmond")
                        ),
                        "address"
                    ),
                    parameters
                );
        }

        [Fact]
        public void RecursiveAllAny()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.Category.QueryableProducts.All(P => P.Category.EnumerableProducts.Any(PP => (PP.ProductName == \"Snacks\")))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AllOperator
                    (
                        parameters,
                        new MemberSelector
                        (
                            "QueryableProducts",
                            new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelector
                            (
                                "EnumerableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, "P"))
                            ),
                            new EqualsBinaryOperator
                            (
                                new MemberSelector("ProductName", new ParameterOperator(parameters, "PP")),
                                new ConstantOperand("Snacks")
                            ),
                            "PP"
                        ),
                        "P"
                    ),
                    parameters
                );
        }
        #endregion Any/All

        #region String Functions
        [Theory]
        [InlineData("Abcd", 0, "Abcd", true)]
        [InlineData("Abcd", 1, "bcd", true)]
        [InlineData("Abcd", 3, "d", true)]
        [InlineData("Abcd", 4, "", true)]
        public void StringSubstringStart(string productName, int startIndex, string compareString, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter
            (
                filter,
                new Product { ProductName = productName }
            );

            //assert
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new SubstringOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(startIndex)
                        ),
                        new ConstantOperand(compareString)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData("Abcd", -1, "Abcd")]
        [InlineData("Abcd", 5, "")]
        public void StringSubstringStartOutOfRange(string productName, int startIndex, string compareString)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            Assert.Throws<ArgumentOutOfRangeException>(() => RunFilter(filter, new Product { ProductName = productName }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new SubstringOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(startIndex)
                        ),
                        new ConstantOperand(compareString)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData("Abcd", 0, 1, "A", true)]
        [InlineData("Abcd", 0, 4, "Abcd", true)]
        [InlineData("Abcd", 0, 3, "Abc", true)]
        [InlineData("Abcd", 1, 3, "bcd", true)]
        [InlineData("Abcd", 2, 1, "c", true)]
        [InlineData("Abcd", 3, 1, "d", true)]
        public void StringSubstringStartAndLength(string productName, int startIndex, int length, string compareString, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter
            (
                filter,
                new Product { ProductName = productName }
            );

            //assert
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new SubstringOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(startIndex),
                            new ConstantOperand(length)
                        ),
                        new ConstantOperand(compareString)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData("Abcd", -1, 4, "Abcd")]
        [InlineData("Abcd", -1, 3, "Abc")]
        [InlineData("Abcd", 0, 5, "Abcd")]
        [InlineData("Abcd", 1, 5, "bcd")]
        [InlineData("Abcd", 4, 1, "")]
        [InlineData("Abcd", 0, -1, "")]
        [InlineData("Abcd", 5, -1, "")]
        public void StringSubstringStartAndLengthOutOfRange(string productName, int startIndex, int length, string compareString)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            Assert.Throws<ArgumentOutOfRangeException>(() => RunFilter(filter, new Product { ProductName = productName }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new SubstringOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(startIndex),
                            new ConstantOperand(length)
                        ),
                        new ConstantOperand(compareString)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData("Abcd", true)]
        [InlineData("Abd", false)]
        public void StringContains(string productName, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.ProductName.Contains(\"Abc\")");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new ContainsOperator
                    (
                        new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand("Abc")
                    ),
                    parameters
                );
        }

        [Fact]
        public void StringContainsNullReferenceException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.ProductName.Contains(\"Abc\")");
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new ContainsOperator
                    (
                        new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand("Abc")
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData("Abcd", true)]
        [InlineData("Abd", false)]
        public void StringStartsWith(string productName, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.ProductName.StartsWith(\"Abc\")");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new StartsWithOperator
                    (
                        new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand("Abc")
                    ),
                    parameters
                );
        }

        [Fact]
        public void StringStartsWithNullReferenceException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.ProductName.StartsWith(\"Abc\")");
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new StartsWithOperator
                    (
                        new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand("Abc")
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData("AAbc", true)]
        [InlineData("Abcd", false)]
        public void StringEndsWith(string productName, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.ProductName.EndsWith(\"Abc\")");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EndsWithOperator
                    (
                        new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand("Abc")
                    ),
                    parameters
                );
        }

        [Fact]
        public void StringEndsWithNullReferenceException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.ProductName.EndsWith(\"Abc\")");
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EndsWithOperator
                    (
                        new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand("Abc")
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData("AAbc", true)]
        [InlineData("", false)]
        public void StringLength(string productName, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName.Length > 0)");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new GreaterThanBinaryOperator
                    (
                        new LengthOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(0)
                    ),
                    parameters
                );
        }

        [Fact]
        public void StringLengthNullReferenceException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName.Length > 0)");
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new GreaterThanBinaryOperator
                    (
                        new LengthOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(0)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData("12345Abc", true)]
        [InlineData("1234Abc", false)]
        public void StringIndexOf(string productName, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName.IndexOf(\"Abc\") == 5)");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new IndexOfOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand("Abc")
                        ),
                        new ConstantOperand(5)
                    ),
                    parameters
                );
        }

        [Fact]
        public void StringIndexOfNullReferenceException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName.IndexOf(\"Abc\") == 5)");
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new IndexOfOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand("Abc")
                        ),
                        new ConstantOperand(5)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData("123uctName", true)]
        [InlineData("1234Abc", false)]
        public void StringSubstring(string productName, bool expected)
        {
            //act
            var filter1 = CreateFilter1<Product>();
            var filter2 = CreateFilter2<Product>();
            bool result = RunFilter(filter1, new Product { ProductName = productName });

            //assert
            AssertFilterStringIsCorrect(filter1, "$it => ($it.ProductName.Substring(3) == \"uctName\")");
            AssertFilterStringIsCorrect(filter2, "$it => ($it.ProductName.Substring(3, 4) == \"uctN\")");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter1<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new SubstringOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(3)
                        ),
                        new ConstantOperand("uctName")
                    ),
                    parameters
                );

            Expression<Func<T, bool>> CreateFilter2<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new SubstringOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(3),
                            new ConstantOperand(4)
                        ),
                        new ConstantOperand("uctN")
                    ),
                    parameters
                );
        }

        [Fact]
        public void StringSubstringNullReferenceException()
        {
            //act
            var filter1 = CreateFilter1<Product>();
            var filter2 = CreateFilter2<Product>();

            //assert
            AssertFilterStringIsCorrect(filter1, "$it => ($it.ProductName.Substring(3) == \"uctName\")");
            AssertFilterStringIsCorrect(filter2, "$it => ($it.ProductName.Substring(3, 4) == \"uctN\")");
            Assert.Throws<NullReferenceException>(() => RunFilter(filter1, new Product { }));

            Expression<Func<T, bool>> CreateFilter1<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new SubstringOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(3)
                        ),
                        new ConstantOperand("uctName")
                    ),
                    parameters
                );

            Expression<Func<T, bool>> CreateFilter2<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new SubstringOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(3),
                            new ConstantOperand(4)
                        ),
                        new ConstantOperand("uctN")
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData("Tasty Treats", true)]
        [InlineData("Tasty Treatss", false)]
        public void StringToLower(string productName, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName.ToLower() == \"tasty treats\")");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new ToLowerOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand("tasty treats")
                    ),
                    parameters
                );
        }

        [Fact]
        public void StringToLowerNullReferenceException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName.ToLower() == \"tasty treats\")");
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new ToLowerOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand("tasty treats")
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData("Tasty Treats", true)]
        [InlineData("Tasty Treatss", false)]
        public void StringToUpper(string productName, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName.ToUpper() == \"TASTY TREATS\")");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new ToUpperOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand("TASTY TREATS")
                    ),
                    parameters
                );
        }

        [Fact]
        public void StringToUpperNullReferenceException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName.ToUpper() == \"TASTY TREATS\")");
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new ToUpperOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand("TASTY TREATS")
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(" Tasty Treats  ", true)]
        [InlineData(" Tasty Treatss  ", false)]
        public void StringTrim(string productName, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName.Trim() == \"Tasty Treats\")");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new TrimOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand("Tasty Treats")
                    ),
                    parameters
                );
        }

        [Fact]
        public void StringTrimNullReferenceException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName.Trim() == \"Tasty Treats\")");
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new TrimOperator
                        (
                            new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand("Tasty Treats")
                    ),
                    parameters
                );
        }

        [Fact]
        public void StringConcat()
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => (\"Food\".Concat(\"Bar\") == \"FoodBar\")");
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new ConcatOperator
                        (
                            new ConstantOperand("Food"),
                            new ConstantOperand("Bar")
                        ),
                        new ConstantOperand("FoodBar")
                    ),
                    parameters
                );
        }
        #endregion String Functions

        #region Date Functions
        [Fact]
        public void DateDay()
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { DiscontinuedDate = new DateTime(2000, 10, 8) });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.DiscontinuedDate.Value.Day == 8)");
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new Product { }));
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new DayOperator
                        (
                            new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(8)
                    ),
                    parameters
                );
        }

        [Fact]
        public void DateDayNonNullable()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.NonNullableDiscontinuedDate.Day == 8)");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new DayOperator
                        (
                            new MemberSelector("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(8)
                    ),
                    parameters
                );
        }

        [Fact]
        public void DateMonth()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.DiscontinuedDate.Value.Month == 8)");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MonthOperator
                        (
                            new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(8)
                    ),
                    parameters
                );
        }

        [Fact]
        public void DateYear()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.DiscontinuedDate.Value.Year == 1974)");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new YearOperator
                        (
                            new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(1974)
                    ),
                    parameters
                );
        }

        [Fact]
        public void DateHour()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.DiscontinuedDate.Value.Hour == 8)");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new HourOperator
                        (
                            new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(8)
                    ),
                    parameters
                );
        }

        [Fact]
        public void DateMinute()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.DiscontinuedDate.Value.Minute == 12)");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MinuteOperator
                        (
                            new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(12)
                    ),
                    parameters
                );
        }

        [Fact]
        public void DateSecond()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.DiscontinuedDate.Value.Second == 33)");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new SecondOperator
                        (
                            new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(33)
                    ),
                    parameters
                );
        }

        public static List<object[]> DateTimeOffsetFunctions_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new YearOperator
                            (
                                new MemberSelector("DiscontinuedOffset", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(100)
                        ),
                       "$it => ($it.DiscontinuedOffset.Year == 100)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MonthOperator
                            (
                                new MemberSelector("DiscontinuedOffset", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(100)
                        ),
                       "$it => ($it.DiscontinuedOffset.Month == 100)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new DayOperator
                            (
                                new MemberSelector("DiscontinuedOffset", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(100)
                        ),
                       "$it => ($it.DiscontinuedOffset.Day == 100)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new HourOperator
                            (
                                new MemberSelector("DiscontinuedOffset", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(100)
                        ),
                       "$it => ($it.DiscontinuedOffset.Hour == 100)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MinuteOperator
                            (
                                new MemberSelector("DiscontinuedOffset", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(100)
                        ),
                       "$it => ($it.DiscontinuedOffset.Minute == 100)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new SecondOperator
                            (
                                new MemberSelector("DiscontinuedOffset", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(100)
                        ),
                       "$it => ($it.DiscontinuedOffset.Second == 100)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new NowDateTimeOperator(),
                            new ConstantOperand(new DateTimeOffset(new DateTime(2016, 11, 8), new TimeSpan(0)))
                        ),
                       "$it => (DateTimeOffset.UtcNow == 11/08/2016 00:00:00 +00:00)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateTimeOffsetFunctions_Data))]
        public void DateTimeOffsetFunctions(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> DateTimeFunctions_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new YearOperator
                            (
                                new MemberSelector("Birthday", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(100)
                        ),
                       "$it => ({0}.Year == 100)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MonthOperator
                            (
                                new MemberSelector("Birthday", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(100)
                        ),
                       "$it => ({0}.Month == 100)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new DayOperator
                            (
                                new MemberSelector("Birthday", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(100)
                        ),
                       "$it => ({0}.Day == 100)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new HourOperator
                            (
                                new MemberSelector("Birthday", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(100)
                        ),
                       "$it => ({0}.Hour == 100)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MinuteOperator
                            (
                                new MemberSelector("Birthday", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(100)
                        ),
                       "$it => ({0}.Minute == 100)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new SecondOperator
                            (
                                new MemberSelector("Birthday", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(100)
                        ),
                       "$it => ({0}.Second == 100)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateTimeFunctions_Data))]
        public void DateTimeFunctions(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, String.Format(expression, "$it.Birthday"));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> DateFunctions_Nullable_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new YearOperator
                            (
                                new MemberSelector("NullableDateProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(2015)
                        ),
                       "$it => ($it.NullableDateProperty.Value.Year == 2015)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MonthOperator
                            (
                                new MemberSelector("NullableDateProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(12)
                        ),
                       "$it => ($it.NullableDateProperty.Value.Month == 12)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new DayOperator
                            (
                                new MemberSelector("NullableDateProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(23)
                        ),
                       "$it => ($it.NullableDateProperty.Value.Day == 23)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateFunctions_Nullable_Data))]
        public void DateFunctions_Nullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> DateFunctions_NonNullable_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new YearOperator
                            (
                                new MemberSelector("DateProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(2015)
                        ),
                        "$it => ($it.DateProperty.Year == 2015)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MonthOperator
                            (
                                new MemberSelector("DateProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(12)
                        ),
                       "$it => ($it.DateProperty.Month == 12)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new DayOperator
                            (
                                new MemberSelector("DateProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(23)
                        ),
                       "$it => ($it.DateProperty.Day == 23)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateFunctions_NonNullable_Data))]
        public void DateFunctions_NonNullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> TimeOfDayFunctions_Nullable_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new HourOperator
                            (
                                new MemberSelector("NullableTimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(10)
                        ),
                        "$it => ($it.NullableTimeOfDayProperty.Value.Hours == 10)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MinuteOperator
                            (
                                new MemberSelector("NullableTimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(20)
                        ),
                       "$it => ($it.NullableTimeOfDayProperty.Value.Minutes == 20)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new SecondOperator
                            (
                                new MemberSelector("NullableTimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(30)
                        ),
                       "$it => ($it.NullableTimeOfDayProperty.Value.Seconds == 30)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(TimeOfDayFunctions_Nullable_Data))]
        public void TimeOfDayFunctions_Nullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> TimeOfDayFunctions_NonNullable_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new HourOperator
                            (
                                new MemberSelector("TimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(10)
                        ),
                        "$it => ($it.TimeOfDayProperty.Hours == 10)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MinuteOperator
                            (
                                new MemberSelector("TimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(20)
                        ),
                       "$it => ($it.TimeOfDayProperty.Minutes == 20)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new SecondOperator
                            (
                                new MemberSelector("TimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(30)
                        ),
                       "$it => ($it.TimeOfDayProperty.Seconds == 30)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(TimeOfDayFunctions_NonNullable_Data))]
        public void TimeOfDayFunctions_NonNullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> FractionalsecondsFunction_Nullable_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new FractionalSecondsOperator
                            (
                                new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(0.2m)
                        ),
                       "$it => ((Convert($it.DiscontinuedDate.Value.Millisecond) / 1000) == 0.2)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new FractionalSecondsOperator
                            (
                                new MemberSelector("NullableTimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(0.2m)
                        ),
                       "$it => ((Convert($it.NullableTimeOfDayProperty.Value.Milliseconds) / 1000) == 0.2)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(FractionalsecondsFunction_Nullable_Data))]
        public void FractionalsecondsFunction_Nullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> FractionalsecondsFunction_NonNullable_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new FractionalSecondsOperator
                            (
                                new MemberSelector("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(0.2m)
                        ),
                       "$it => ((Convert($it.NonNullableDiscontinuedDate.Millisecond) / 1000) == 0.2)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new FractionalSecondsOperator
                            (
                                new MemberSelector("TimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperand(0.2m)
                        ),
                       "$it => ((Convert($it.TimeOfDayProperty.Milliseconds) / 1000) == 0.2)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(FractionalsecondsFunction_NonNullable_Data))]
        public void FractionalsecondsFunction_NonNullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> DateFunction_Nullable_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new ConvertToNumericDate
                            (
                                new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericDate
                            (
                                new ConstantOperand(new Date(2015, 2, 26))
                            )
                        ),
                        "$it => (((($it.DiscontinuedDate.Value.Year * 10000) + ($it.DiscontinuedDate.Value.Month * 100)) + $it.DiscontinuedDate.Value.Day) == (((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanBinaryOperator
                        (
                            new ConvertToNumericDate
                            (
                                new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericDate
                            (
                                new ConstantOperand(new Date(2016, 2, 26))
                            )
                        ),
                        "$it => (((($it.DiscontinuedDate.Value.Year * 10000) + ($it.DiscontinuedDate.Value.Month * 100)) + $it.DiscontinuedDate.Value.Day) < (((2016-02-26.Year * 10000) + (2016-02-26.Month * 100)) + 2016-02-26.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericDate
                            (
                                new ConstantOperand(new Date(2015, 2, 26))
                            ),
                            new ConvertToNumericDate
                            (
                                new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        "$it => ((((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day) >= ((($it.DiscontinuedDate.Value.Year * 10000) + ($it.DiscontinuedDate.Value.Month * 100)) + $it.DiscontinuedDate.Value.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new ConstantOperand(null),
                            new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => (null != $it.DiscontinuedDate)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(null)
                        ),
                        "$it => ($it.DiscontinuedDate == null)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateFunction_Nullable_Data))]
        public void DateFunction_Nullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> DateFunction_NonNullable_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new ConvertToNumericDate
                            (
                                new MemberSelector("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericDate
                            (
                                new ConstantOperand(new Date(2015, 2, 26))
                            )
                        ),
                        "$it => (((($it.NonNullableDiscontinuedDate.Year * 10000) + ($it.NonNullableDiscontinuedDate.Month * 100)) + $it.NonNullableDiscontinuedDate.Day) == (((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanBinaryOperator
                        (
                            new ConvertToNumericDate
                            (
                                new MemberSelector("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericDate
                            (
                                new ConstantOperand(new Date(2016, 2, 26))
                            )
                        ),
                        "$it => (((($it.NonNullableDiscontinuedDate.Year * 10000) + ($it.NonNullableDiscontinuedDate.Month * 100)) + $it.NonNullableDiscontinuedDate.Day) < (((2016-02-26.Year * 10000) + (2016-02-26.Month * 100)) + 2016-02-26.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericDate
                            (
                                new ConstantOperand(new Date(2015, 2, 26))
                            ),
                            new ConvertToNumericDate
                            (
                                new MemberSelector("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        "$it => ((((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day) >= ((($it.NonNullableDiscontinuedDate.Year * 10000) + ($it.NonNullableDiscontinuedDate.Month * 100)) + $it.NonNullableDiscontinuedDate.Day))",
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateFunction_NonNullable_Data))]
        public void DateFunction_NonNullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> TimeFunction_Nullable_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new ConvertToNumericTime
                            (
                                new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericTime
                            (
                                new ConstantOperand(new TimeOfDay(1, 2, 3, 4))
                            )
                        ),
                        "$it => (((Convert($it.DiscontinuedDate.Value.Hour) * 36000000000) + ((Convert($it.DiscontinuedDate.Value.Minute) * 600000000) + ((Convert($it.DiscontinuedDate.Value.Second) * 10000000) + Convert($it.DiscontinuedDate.Value.Millisecond)))) == ((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))))",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericTime
                            (
                                new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericTime
                            (
                                new ConstantOperand(new TimeOfDay(1, 2, 3, 4))
                            )
                        ),
                        "$it => (((Convert($it.DiscontinuedDate.Value.Hour) * 36000000000) + ((Convert($it.DiscontinuedDate.Value.Minute) * 600000000) + ((Convert($it.DiscontinuedDate.Value.Second) * 10000000) + Convert($it.DiscontinuedDate.Value.Millisecond)))) >= ((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))))",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericTime
                            (
                                new ConstantOperand(new TimeOfDay(1, 2, 3, 4))
                            ),
                            new ConvertToNumericTime
                            (
                                new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        "$it => (((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))) <= ((Convert($it.DiscontinuedDate.Value.Hour) * 36000000000) + ((Convert($it.DiscontinuedDate.Value.Minute) * 600000000) + ((Convert($it.DiscontinuedDate.Value.Second) * 10000000) + Convert($it.DiscontinuedDate.Value.Millisecond)))))",
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new ConstantOperand(null),
                            new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => (null != $it.DiscontinuedDate)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("DiscontinuedDate", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(null)
                        ),
                        "$it => ($it.DiscontinuedDate == null)",
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TimeFunction_Nullable_Data))]
        public void TimeFunction_Nullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> TimeFunction_NonNullable_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new ConvertToNumericTime
                            (
                                new MemberSelector("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericTime
                            (
                                new ConstantOperand(new TimeOfDay(1, 2, 3, 4))
                            )
                        ),
                        "$it => (((Convert($it.NonNullableDiscontinuedDate.Hour) * 36000000000) + ((Convert($it.NonNullableDiscontinuedDate.Minute) * 600000000) + ((Convert($it.NonNullableDiscontinuedDate.Second) * 10000000) + Convert($it.NonNullableDiscontinuedDate.Millisecond)))) == ((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))))",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericTime
                            (
                                new MemberSelector("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericTime
                            (
                                new ConstantOperand(new TimeOfDay(1, 2, 3, 4))
                            )
                        ),
                        "$it => (((Convert($it.NonNullableDiscontinuedDate.Hour) * 36000000000) + ((Convert($it.NonNullableDiscontinuedDate.Minute) * 600000000) + ((Convert($it.NonNullableDiscontinuedDate.Second) * 10000000) + Convert($it.NonNullableDiscontinuedDate.Millisecond)))) >= ((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))))",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericTime
                            (
                                new ConstantOperand(new TimeOfDay(1, 2, 3, 4))
                            ),
                            new ConvertToNumericTime
                            (
                                new MemberSelector("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        "$it => (((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))) <= ((Convert($it.NonNullableDiscontinuedDate.Hour) * 36000000000) + ((Convert($it.NonNullableDiscontinuedDate.Minute) * 600000000) + ((Convert($it.NonNullableDiscontinuedDate.Second) * 10000000) + Convert($it.NonNullableDiscontinuedDate.Millisecond)))))",
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TimeFunction_NonNullable_Data))]
        public void TimeFunction_NonNullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }
        #endregion Date Functions

        #region Math Functions
        [Fact]
        public void RecursiveMethodCall()
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = 123.3m });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.UnitPrice.Value.Floor().Floor() == 123)");
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new FloorOperator
                        (
                            new FloorOperator
                            (
                                new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        new ConstantOperand(123m)
                    ),
                    parameters
                );
        }

        [Fact]
        public void RecursiveMethodCallInvalidOperationException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.UnitPrice.Value.Floor().Floor() == 123)");
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new FloorOperator
                        (
                            new FloorOperator
                            (
                                new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        new ConstantOperand(123m)
                    ),
                    parameters
                );
        }

        [Fact]
        public void MathRoundDecimalInvalidOperationException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => ($it.UnitPrice.Value.Round() > {0:0.00})", 5.0));
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new GreaterThanBinaryOperator
                    (
                        new RoundOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(5.00m)
                    ),
                    parameters
                );
        }

        public static IEnumerable<object[]> MathRoundDecimal_DataSet
            => new List<object[]>
                {
                    new object[] { 5.9m, true },
                    new object[] { 5.4m, false }
                };

        [Theory, MemberData(nameof(MathRoundDecimal_DataSet))]
        public void MathRoundDecimal(decimal? unitPrice, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = ToNullable<decimal>(unitPrice) });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => ($it.UnitPrice.Value.Round() > {0:0.00})", 5.0));
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new GreaterThanBinaryOperator
                    (
                        new RoundOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(5.00m)
                    ),
                    parameters
                );
        }

        [Fact]
        public void MathRoundDoubleInvalidOperationException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => ($it.Weight.Value.Round() > {0})", 5));
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new GreaterThanBinaryOperator
                    (
                        new RoundOperator
                        (
                            new MemberSelector("Weight", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(5d)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(5.9d, true)]
        [InlineData(5.4d, false)]
        public void MathRoundDouble(double? weight, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { Weight = ToNullable<double>(weight) });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => ($it.Weight.Value.Round() > {0})", 5));
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new GreaterThanBinaryOperator
                    (
                        new RoundOperator
                        (
                            new MemberSelector("Weight", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(5d)
                    ),
                    parameters
                );
        }

        [Fact]
        public void MathRoundFloatInvalidOperationException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => (Convert($it.Width).Value.Round() > {0})", 5));
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new GreaterThanBinaryOperator
                    (
                        new RoundOperator
                        (
                            new ConvertOperand(new MemberSelector("Width", new ParameterOperator(parameters, parameterName)), typeof(double?))
                        ),
                        new ConstantOperand(5d)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(5.9f, true)]
        [InlineData(5.4f, false)]
        public void MathRoundFloat(float? width, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { Width = ToNullable<float>(width) });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, "$it => (Convert($it.Width).Value.Round() > {0})", 5));
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new GreaterThanBinaryOperator
                    (
                        new RoundOperator
                        (
                            new ConvertOperand(new MemberSelector("Width", new ParameterOperator(parameters, parameterName)), typeof(double?))
                        ),
                        new ConstantOperand(5d)
                    ),
                    parameters
                );
        }

        [Fact]
        public void MathFloorDecimalInvalidOperationException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.UnitPrice.Value.Floor() == 5)");
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new FloorOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(5m)
                    ),
                    parameters
                );
        }

        public static IEnumerable<object[]> MathFloorDecimal_DataSet
            => new List<object[]>
                {
                    new object[] { 5.4m, true },
                    new object[] { 4.4m, false }
                };

        [Theory, MemberData(nameof(MathFloorDecimal_DataSet))]
        public void MathFloorDecimal(decimal? unitPrice, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = ToNullable<decimal>(unitPrice) });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.UnitPrice.Value.Floor() == 5)");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new FloorOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(5m)
                    ),
                    parameters
                );
        }

        [Fact]
        public void MathFloorDoubleInvalidOperationException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.Weight.Value.Floor() == 5)");
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new FloorOperator
                        (
                            new MemberSelector("Weight", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(5d)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(5.4d, true)]
        [InlineData(4.4d, false)]
        public void MathFloorDouble(double? weight, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { Weight = ToNullable<double>(weight) });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.Weight.Value.Floor() == 5)");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new FloorOperator
                        (
                            new MemberSelector("Weight", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(5d)
                    ),
                    parameters
                );
        }

        [Fact]
        public void MathFloorFloatInvalidOperationException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => (Convert($it.Width).Value.Floor() == 5)");
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new FloorOperator
                        (
                            new ConvertOperand(new MemberSelector("Width", new ParameterOperator(parameters, parameterName)), typeof(double?))
                        ),
                        new ConstantOperand(5d)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(5.4f, true)]
        [InlineData(4.4f, false)]
        public void MathFloorFloat(float? width, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { Width = ToNullable<float>(width) });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => (Convert($it.Width).Value.Floor() == 5)");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new FloorOperator
                        (
                            new ConvertOperand(new MemberSelector("Width", new ParameterOperator(parameters, parameterName)), typeof(double?))
                        ),
                        new ConstantOperand(5d)
                    ),
                    parameters
                );
        }

        [Fact]
        public void MathCeilingDecimalInvalidOperationException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.UnitPrice.Value.Ceiling() == 5)");
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new CeilingOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(5m)
                    ),
                    parameters
                );
        }

        public static IEnumerable<object[]> MathCeilingDecimal_DataSet
            => new List<object[]>
                {
                    new object[] { 4.1m, true },
                    new object[] { 5.9m, false }
                };

        [Theory, MemberData(nameof(MathCeilingDecimal_DataSet))]
        public void MathCeilingDecimal(object unitPrice, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitPrice = ToNullable<decimal>(unitPrice) });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.UnitPrice.Value.Ceiling() == 5)");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new CeilingOperator
                        (
                            new MemberSelector("UnitPrice", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(5m)
                    ),
                    parameters
                );
        }

        [Fact]
        public void MathCeilingDoubleInvalidOperationException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.Weight.Value.Ceiling() == 5)");
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new CeilingOperator
                        (
                            new MemberSelector("Weight", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(5d)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(4.1d, true)]
        [InlineData(5.9d, false)]
        public void MathCeilingDouble(double? weight, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { Weight = ToNullable<double>(weight) });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.Weight.Value.Ceiling() == 5)");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new CeilingOperator
                        (
                            new MemberSelector("Weight", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperand(5d)
                    ),
                    parameters
                );
        }

        [Fact]
        public void MathCeilingFloatInvalidOperationException()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => (Convert($it.Width).Value.Ceiling() == 5)");
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new Product { }));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new CeilingOperator
                        (
                            new ConvertOperand(new MemberSelector("Width", new ParameterOperator(parameters, parameterName)), typeof(double?))
                        ),
                        new ConstantOperand(5d)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData(4.1f, true)]
        [InlineData(5.9f, false)]
        public void MathCeilingFloat(float? width, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { Width = ToNullable<float>(width) });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => (Convert($it.Width).Value.Ceiling() == 5)");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new CeilingOperator
                        (
                            new ConvertOperand(new MemberSelector("Width", new ParameterOperator(parameters, parameterName)), typeof(double?))
                        ),
                        new ConstantOperand(5d)
                    ),
                    parameters
                );
        }

        public static List<object[]> MathFunctions_VariousTypes_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new FloorOperator
                            (
                                new ConvertOperand(new MemberSelector("FloatProp", new ParameterOperator(parameters, parameterName)), typeof(double))
                            ),
                            new FloorOperator
                            (
                                new ConvertOperand(new MemberSelector("FloatProp", new ParameterOperator(parameters, parameterName)), typeof(double))
                            )
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new RoundOperator
                            (
                                new ConvertOperand(new MemberSelector("FloatProp", new ParameterOperator(parameters, parameterName)), typeof(double))
                            ),
                            new RoundOperator
                            (
                                new ConvertOperand(new MemberSelector("FloatProp", new ParameterOperator(parameters, parameterName)), typeof(double))
                            )
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new CeilingOperator
                            (
                                new ConvertOperand(new MemberSelector("FloatProp", new ParameterOperator(parameters, parameterName)), typeof(double))
                            ),
                            new CeilingOperator
                            (
                                new ConvertOperand(new MemberSelector("FloatProp", new ParameterOperator(parameters, parameterName)), typeof(double))
                            )
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new FloorOperator
                            (
                                new MemberSelector("DoubleProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new FloorOperator
                            (
                                new MemberSelector("DoubleProp", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new RoundOperator
                            (
                                new MemberSelector("DoubleProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new RoundOperator
                            (
                                new MemberSelector("DoubleProp", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new CeilingOperator
                            (
                                new MemberSelector("DoubleProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new CeilingOperator
                            (
                                new MemberSelector("DoubleProp", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        parameters
                    },
                     new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new FloorOperator
                            (
                                new MemberSelector("DecimalProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new FloorOperator
                            (
                                new MemberSelector("DecimalProp", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new RoundOperator
                            (
                                new MemberSelector("DecimalProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new RoundOperator
                            (
                                new MemberSelector("DecimalProp", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new CeilingOperator
                            (
                                new MemberSelector("DecimalProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new CeilingOperator
                            (
                                new MemberSelector("DecimalProp", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(MathFunctions_VariousTypes_Data))]
        public void MathFunctions_VariousTypes(IExpressionPart filterBody, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<DataTypes>();
            bool result = RunFilter(filter, new DataTypes { });

            //assert
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }
        #endregion Math Functions

        #region Custom Functions
        [Fact]
        public void CustomMethod_InstanceMethodOfDeclaringType()
        {
            //arrange
            const string productName = "Abcd";
            const int totalWidth = 5;
            const string expectedProductName = "Abcd ";

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new CustomMethodOperator
                        (
                            typeof(string).GetMethod("PadRight", new Type[] { typeof(int) }),
                            new IExpressionPart[]
                            {
                                new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperand(totalWidth)
                            }
                        ),
                        new ConstantOperand(expectedProductName)
                    ),
                    parameters
                );
        }

        [Fact]
        public void CustomMethod_StaticExtensionMethod()
        {
            //arrange
            const string productName = "Abcd";
            const int totalWidth = 5;
            const string expectedProductName = "Abcd ";

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new CustomMethodOperator
                        (
                            typeof(StringExtender).GetMethod("PadRightExStatic", BindingFlags.Public | BindingFlags.Static),
                            new IExpressionPart[]
                            {
                                new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperand(totalWidth)
                            }
                        ),
                        new ConstantOperand(expectedProductName)
                    ),
                    parameters
                );
        }

        [Fact]
        public void CustomMethod_StaticMethodNotOfDeclaringType()
        {
            //arrange
            const string productName = "Abcd";
            const int totalWidth = 5;
            const string expectedProductName = "Abcd ";

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = productName });

            //assert
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new CustomMethodOperator
                        (
                            typeof(FilterTests).GetMethod("PadRightStatic", BindingFlags.NonPublic | BindingFlags.Static),
                            new IExpressionPart[]
                            {
                                new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperand(totalWidth)
                            }
                        ),
                        new ConstantOperand(expectedProductName)
                    ),
                    parameters
                );
        }
        #endregion Custom Functions

        #region Data Types
        [Fact]
        public void GuidExpression()
        {
            //act
            var filter1 = CreateFilter1<DataTypes>();
            var filter2 = CreateFilter2<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter1, "$it => ($it.GuidProp == 0efdaecf-a9f0-42f3-a384-1295917af95e)");
            AssertFilterStringIsCorrect(filter2, "$it => ($it.GuidProp == 0efdaecf-a9f0-42f3-a384-1295917af95e)");

            Expression<Func<T, bool>> CreateFilter1<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelector("GuidProp", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand(new Guid("0EFDAECF-A9F0-42F3-A384-1295917AF95E"))
                    ),
                    parameters
                );

            Expression<Func<T, bool>> CreateFilter2<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelector("GuidProp", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand(new Guid("0efdaecf-a9f0-42f3-a384-1295917af95e"))
                    ),
                    parameters
                );
        }

        public static List<object[]> DateTimeExpression_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector("DateTimeProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(new DateTimeOffset(new DateTime(2000, 12, 12, 12, 0, 0), TimeSpan.Zero))
                        ),
                       "$it => ($it.DateTimeProp == {0})",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanBinaryOperator
                        (
                            new MemberSelector("DateTimeProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(new DateTimeOffset(new DateTime(2000, 12, 12, 12, 0, 0), TimeSpan.Zero))
                        ),
                       "$it => ($it.DateTimeProp < {0})",
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateTimeExpression_Data))]
        public void DateTimeExpression(IExpressionPart filterBody, string expectedExpression, IDictionary<string, ParameterExpression> parameters)
        {
            //arrange
            var dateTime = new DateTimeOffset(new DateTime(2000, 12, 12, 12, 0, 0), TimeSpan.Zero);

            //act
            var filter = CreateFilter<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter, string.Format(CultureInfo.InvariantCulture, expectedExpression, dateTime));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        [Fact]
        public void IntegerLiteralSuffix()
        {
            //act
            var filter1 = CreateFilter1<DataTypes>();
            var filter2 = CreateFilter2<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter1, "$it => (($it.LongProp < 987654321) AndAlso ($it.LongProp > 123456789))");
            AssertFilterStringIsCorrect(filter2, "$it => (($it.LongProp < -987654321) AndAlso ($it.LongProp > -123456789))");

            Expression<Func<T, bool>> CreateFilter1<T>()
                => GetFilter<T>
                (
                    new AndBinaryOperator
                    (
                        new LessThanBinaryOperator
                        (
                            new MemberSelector("LongProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand((long)987654321, typeof(long))
                        ),
                        new GreaterThanBinaryOperator
                        (
                            new MemberSelector("LongProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand((long)123456789, typeof(long))
                        )
                    ),
                    parameters
                );

            Expression<Func<T, bool>> CreateFilter2<T>()
                => GetFilter<T>
                (
                    new AndBinaryOperator
                    (
                        new LessThanBinaryOperator
                        (
                            new MemberSelector("LongProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand((long)-987654321, typeof(long))
                        ),
                        new GreaterThanBinaryOperator
                        (
                            new MemberSelector("LongProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand((long)-123456789, typeof(long))
                        )
                    ),
                    parameters
                );
        }

        [Fact]
        public void EnumInExpression()
        {
            //act
            var filter = CreateFilter<DataTypes>();
            var constant = (ConstantExpression)((MethodCallExpression)filter.Body).Arguments[0];
            var values = (IList<SimpleEnum>)constant.Value;

            //assert
            AssertFilterStringIsCorrect(filter, "$it => System.Collections.Generic.List`1[LogicBuilder.Expressions.Utils.Tests.Data.SimpleEnum].Contains($it.SimpleEnumProp)");
            Assert.Equal(new[] { SimpleEnum.First, SimpleEnum.Second }, values);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new InOperator
                    (
                        new MemberSelector("SimpleEnumProp", new ParameterOperator(parameters, parameterName)),
                        new CollectionConstantOperand(typeof(SimpleEnum), new List<object> { SimpleEnum.First, SimpleEnum.Second })
                    ),
                    parameters
                );
        }

        [Fact]
        public void EnumInExpression_NullableEnum_WithNullable()
        {
            //act
            var filter = CreateFilter<DataTypes>();
            var constant = (ConstantExpression)((MethodCallExpression)filter.Body).Arguments[0];
            var values = (IList<SimpleEnum?>)constant.Value;

            //assert
            AssertFilterStringIsCorrect(filter, "$it => System.Collections.Generic.List`1[System.Nullable`1[LogicBuilder.Expressions.Utils.Tests.Data.SimpleEnum]].Contains($it.NullableSimpleEnumProp)");
            Assert.Equal(new SimpleEnum?[] { SimpleEnum.First, SimpleEnum.Second }, values);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new InOperator
                    (
                        new MemberSelector("NullableSimpleEnumProp", new ParameterOperator(parameters, parameterName)),
                        new CollectionConstantOperand(typeof(SimpleEnum?), new List<object> { SimpleEnum.First, SimpleEnum.Second })
                    ),
                    parameters
                );
        }

        [Fact]
        public void EnumInExpression_NullableEnum_WithNullValue()
        {
            //act
            var filter = CreateFilter<DataTypes>();
            var constant = (ConstantExpression)((MethodCallExpression)filter.Body).Arguments[0];
            var values = (IList<SimpleEnum?>)constant.Value;

            //assert
            AssertFilterStringIsCorrect(filter, "$it => System.Collections.Generic.List`1[System.Nullable`1[LogicBuilder.Expressions.Utils.Tests.Data.SimpleEnum]].Contains($it.NullableSimpleEnumProp)");
            Assert.Equal(new SimpleEnum?[] { SimpleEnum.First, null }, values);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new InOperator
                    (
                        new MemberSelector("NullableSimpleEnumProp", new ParameterOperator(parameters, parameterName)),
                        new CollectionConstantOperand(typeof(SimpleEnum?), new List<object> { SimpleEnum.First, null })
                    ),
                    parameters
                );
        }

        [Fact]
        public void RealLiteralSuffixes()
        {
            //act
            var filter1 = CreateFilter1<DataTypes>();
            var filter2 = CreateFilter2<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter1, string.Format(CultureInfo.InvariantCulture, "$it => (($it.FloatProp < {0:0.00}) AndAlso ($it.FloatProp > {1:0.00}))", 4321.56, 1234.56));
            AssertFilterStringIsCorrect(filter2, string.Format(CultureInfo.InvariantCulture, "$it => (($it.DecimalProp < {0:0.00}) AndAlso ($it.DecimalProp > {1:0.00}))", 4321.56, 1234.56));

            Expression<Func<T, bool>> CreateFilter1<T>()
                => GetFilter<T>
                (
                    new AndBinaryOperator
                    (
                        new LessThanBinaryOperator
                        (
                            new MemberSelector("FloatProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(4321.56F)
                        ),
                        new GreaterThanBinaryOperator
                        (
                            new MemberSelector("FloatProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(1234.56f)
                        )
                    ),
                    parameters
                );

            Expression<Func<T, bool>> CreateFilter2<T>()
                => GetFilter<T>
                (
                    new AndBinaryOperator
                    (
                        new LessThanBinaryOperator
                        (
                            new MemberSelector("DecimalProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(4321.56M)
                        ),
                        new GreaterThanBinaryOperator
                        (
                            new MemberSelector("DecimalProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperand(1234.56m)
                        )
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData("hello,world", "hello,world")]
        [InlineData("'hello,world", "'hello,world")]
        [InlineData("hello,world'", "hello,world'")]
        [InlineData("hello,'wor'ld", "hello,'wor'ld")]
        [InlineData("hello,''world", "hello,''world")]
        [InlineData("\"hello,world\"", "\"hello,world\"")]
        [InlineData("\"hello,world", "\"hello,world")]
        [InlineData("hello,world\"", "hello,world\"")]
        [InlineData("hello,\"world", "hello,\"world")]
        [InlineData("México D.F.", "México D.F.")]
        [InlineData("æææøøøååå", "æææøøøååå")]
        [InlineData("いくつかのテキスト", "いくつかのテキスト")]
        public void StringLiterals(string literal, string expected)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, string.Format("$it => ($it.ProductName == \"{0}\")", expected));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand(literal)
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData('$')]
        [InlineData('&')]
        [InlineData('+')]
        [InlineData(',')]
        [InlineData('/')]
        [InlineData(':')]
        [InlineData(';')]
        [InlineData('=')]
        [InlineData('?')]
        [InlineData('@')]
        [InlineData(' ')]
        [InlineData('<')]
        [InlineData('>')]
        [InlineData('#')]
        [InlineData('%')]
        [InlineData('{')]
        [InlineData('}')]
        [InlineData('|')]
        [InlineData('\\')]
        [InlineData('^')]
        [InlineData('~')]
        [InlineData('[')]
        [InlineData(']')]
        [InlineData('`')]
        public void SpecialCharactersInStringLiteral(char c)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductName = c.ToString() });

            //assert
            AssertFilterStringIsCorrect(filter, string.Format("$it => ($it.ProductName == \"{0}\")", c));
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperand(c.ToString())
                    ),
                    parameters
                );
        }
        #endregion Data Types

        #region Casts
        [Fact]
        public void NSCast_OnEnumerableEntityCollection_GeneratesExpression_WithOfTypeOnEnumerable()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.Category.EnumerableProducts.OfType().Any(p => (p.ProductName == \"ProductName\"))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AnyOperator
                    (
                        parameters,
                        new CollectionCastOperator
                        (
                            new MemberSelector
                            (
                                "EnumerableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            typeof(DerivedProduct)
                        ),
                        new EqualsBinaryOperator
                        (
                             new MemberSelector("ProductName", new ParameterOperator(parameters, "p")),
                             new ConstantOperand("ProductName")
                        ),
                        "p"
                    ),
                    parameters
                );
        }

        [Fact]
        public void NSCast_OnQueryableEntityCollection_GeneratesExpression_WithOfTypeOnQueryable()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => $it.Category.QueryableProducts.OfType().Any(p => (p.ProductName == \"ProductName\"))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AnyOperator
                    (
                        parameters,
                        new CollectionCastOperator
                        (
                            new MemberSelector
                            (
                                "QueryableProducts",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            typeof(DerivedProduct)
                        ),
                        new EqualsBinaryOperator
                        (
                             new MemberSelector("ProductName", new ParameterOperator(parameters, "p")),
                             new ConstantOperand("ProductName")
                        ),
                        "p"
                    ),
                    parameters
                );
        }

        [Fact]
        public void NSCast_OnEntityCollection_CanAccessDerivedInstanceProperty()
        {
            //act
            var filter = CreateFilter<Product>();
            bool result1 = RunFilter(filter, new Product { Category = new Category { Products = new Product[] { new DerivedProduct { DerivedProductName = "DerivedProductName" } } } });
            bool result2 = RunFilter(filter, new Product { Category = new Category { Products = new Product[] { new DerivedProduct { DerivedProductName = "NotDerivedProductName" } } } });

            //assert
            Assert.True(result1);
            Assert.False(result2);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new AnyOperator
                    (
                        parameters,
                        new CollectionCastOperator
                        (
                            new MemberSelector
                            (
                                "Products",
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            typeof(DerivedProduct)
                        ),
                        new EqualsBinaryOperator
                        (
                             new MemberSelector("DerivedProductName", new ParameterOperator(parameters, "p")),
                             new ConstantOperand("DerivedProductName")
                        ),
                        "p"
                    ),
                    parameters
                );
        }

        [Fact]
        public void NSCast_OnSingleEntity_GeneratesExpression_WithAsOperator()
        {
            //act
            var filter = CreateFilter<DerivedProduct>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => (($it As Product).ProductName == \"ProductName\")");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelector
                        (
                            "ProductName",
                            new CastOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(Product)
                            )
                        ),
                        new ConstantOperand("ProductName")
                    ),
                    parameters
                );
        }

        public static List<object[]> Inheritance_WithDerivedInstance_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector
                            (
                                "ProductName",
                                new CastOperator
                                (
                                    new ParameterOperator(parameters, parameterName),
                                    typeof(Product)
                                )
                            ),
                            new ConstantOperand("ProductName")
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector
                            (
                                "DerivedProductName",
                                new CastOperator
                                (
                                    new ParameterOperator(parameters, parameterName),
                                    typeof(DerivedProduct)
                                )
                            ),
                            new ConstantOperand("DerivedProductName")
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector
                            (
                                "CategoryID",
                                new MemberSelector
                                (
                                    "Category",
                                    new CastOperator
                                    (
                                        new ParameterOperator(parameters, parameterName),
                                        typeof(DerivedProduct)
                                    )
                                )
                            ),
                            new ConstantOperand(123)
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector
                            (
                                "CategoryID",
                                new CastOperator
                                (
                                    new MemberSelector
                                    (
                                        "Category",
                                        new CastOperator
                                        (
                                            new ParameterOperator(parameters, parameterName),
                                            typeof(DerivedProduct)
                                        )
                                    ),
                                    typeof(DerivedCategory)
                                )
                            ),
                            new ConstantOperand(123)
                        ),
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(Inheritance_WithDerivedInstance_Data))]
        public void Inheritance_WithDerivedInstance(IExpressionPart filterBody, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<DerivedProduct>();
            bool result = RunFilter(filter, new DerivedProduct { Category = new DerivedCategory { CategoryID = 123 }, ProductName = "ProductName", DerivedProductName = "DerivedProductName" });

            //assert
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> Inheritance_WithBaseInstance_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector
                            (
                                "DerivedProductName",
                                new CastOperator
                                (
                                    new ParameterOperator(parameters, parameterName),
                                    typeof(DerivedProduct)
                                )
                            ),
                            new ConstantOperand("DerivedProductName")
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector
                            (
                                "CategoryID",
                                new MemberSelector
                                (
                                    "Category",
                                    new CastOperator
                                    (
                                        new ParameterOperator(parameters, parameterName),
                                        typeof(DerivedProduct)
                                    )
                                )
                            ),
                            new ConstantOperand(123)
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelector
                            (
                                "CategoryID",
                                new CastOperator
                                (
                                    new MemberSelector
                                    (
                                        "Category",
                                        new CastOperator
                                        (
                                            new ParameterOperator(parameters, parameterName),
                                            typeof(DerivedProduct)
                                        )
                                    ),
                                    typeof(DerivedCategory)
                                )
                            ),
                            new ConstantOperand(123)
                        ),
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(Inheritance_WithBaseInstance_Data))]
        public void Inheritance_WithBaseInstance(IExpressionPart filterBody, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product()));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> CastMethod_Succeeds_Data
        {
            get
            {
                //var parameters = GetParameters();

                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConstantOperand(null),
                                new ConstantOperand(null)
                            ),
                           "$it => (null == null)",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConstantOperand(null),
                                new ConstantOperand(123)
                            ),
                            "$it => (null == Convert(123))",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new NotEqualsBinaryOperator
                            (
                                new ConstantOperand(null),
                                new ConstantOperand(123)
                            ),
                            "$it => (null != Convert(123))",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new NotEqualsBinaryOperator
                            (
                                new ConstantOperand(null),
                                new ConstantOperand(true)
                            ),
                            "$it => (null != Convert(True))",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new NotEqualsBinaryOperator
                            (
                                new ConstantOperand(null),
                                new ConstantOperand(1)
                            ),
                            "$it => (null != Convert(1))",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConstantOperand(null),
                                new ConstantOperand(new Guid())
                            ),
                            "$it => (null == Convert(00000000-0000-0000-0000-000000000000))",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new NotEqualsBinaryOperator
                            (
                                new ConstantOperand(null),
                                new ConstantOperand("123")
                            ),
                            "$it => (null != \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConstantOperand(null),
                                new ConstantOperand(new DateTimeOffset(new DateTime(2001, 1, 1, 12, 0, 0), new TimeSpan(8, 0, 0)))
                            ),
                            "$it => (null == Convert(01/01/2001 12:00:00 +08:00))",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConstantOperand(null),
                                new ConstantOperand(new TimeSpan(7775999999000))
                            ),
                            "$it => (null == Convert(8.23:59:59.9999000))",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("IntProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => ($it.IntProp.ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("LongProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => ($it.LongProp.ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("SingleProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => ($it.SingleProp.ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("DoubleProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => ($it.DoubleProp.ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("DecimalProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => ($it.DecimalProp.ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("BoolProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => ($it.BoolProp.ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("ByteProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => ($it.ByteProp.ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("GuidProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => ($it.GuidProp.ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new MemberSelector("StringProp", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperand("123")
                            ),
                            "$it => ($it.StringProp == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => ($it.DateTimeOffsetProp.ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("TimeSpanProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => ($it.TimeSpanProp.ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("SimpleEnumProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (Convert($it.SimpleEnumProp).ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("FlagsEnumProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (Convert($it.FlagsEnumProp).ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("LongEnumProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (Convert($it.LongEnumProp).ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("NullableIntProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (IIF($it.NullableIntProp.HasValue, $it.NullableIntProp.Value.ToString(), null) == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("NullableLongProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (IIF($it.NullableLongProp.HasValue, $it.NullableLongProp.Value.ToString(), null) == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("NullableSingleProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (IIF($it.NullableSingleProp.HasValue, $it.NullableSingleProp.Value.ToString(), null) == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("NullableDoubleProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (IIF($it.NullableDoubleProp.HasValue, $it.NullableDoubleProp.Value.ToString(), null) == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("NullableDecimalProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (IIF($it.NullableDecimalProp.HasValue, $it.NullableDecimalProp.Value.ToString(), null) == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("NullableBoolProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (IIF($it.NullableBoolProp.HasValue, $it.NullableBoolProp.Value.ToString(), null) == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("NullableByteProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (IIF($it.NullableByteProp.HasValue, $it.NullableByteProp.Value.ToString(), null) == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("NullableGuidProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (IIF($it.NullableGuidProp.HasValue, $it.NullableGuidProp.Value.ToString(), null) == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("NullableDateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (IIF($it.NullableDateTimeOffsetProp.HasValue, $it.NullableDateTimeOffsetProp.Value.ToString(), null) == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("NullableTimeSpanProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (IIF($it.NullableTimeSpanProp.HasValue, $it.NullableTimeSpanProp.Value.ToString(), null) == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new MemberSelector("NullableSimpleEnumProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (IIF($it.NullableSimpleEnumProp.HasValue, Convert($it.NullableSimpleEnumProp.Value).ToString(), null) == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertOperand
                                (
                                    new MemberSelector("IntProp", new ParameterOperator(parameters, parameterName)),
                                    typeof(long)
                                ),
                                new ConstantOperand((long)123)
                            ),
                            "$it => (Convert($it.IntProp) == 123)",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertOperand
                                (
                                    new MemberSelector("NullableLongProp", new ParameterOperator(parameters, parameterName)),
                                    typeof(double)
                                ),
                                new ConstantOperand(1.23d)
                            ),
                            "$it => (Convert($it.NullableLongProp) == 1.23)",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new NotEqualsBinaryOperator
                            (
                                new ConvertOperand
                                (
                                    new ConstantOperand(2147483647),
                                    typeof(short)
                                ),
                                new ConstantOperand(null)
                            ),
                            "$it => (Convert(Convert(2147483647)) != null)",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new ConstantOperand(SimpleEnum.Second, typeof(SimpleEnum))
                                ),
                                new ConstantOperand("1")
                            ),
                            "$it => (Convert(Second).ToString() == \"1\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new ConvertOperand
                                    (
                                        new ConvertOperand
                                        (
                                            new MemberSelector("IntProp", new ParameterOperator(parameters, parameterName)),
                                            typeof(long)
                                        ),
                                        typeof(short)
                                    )
                                ),
                                new ConstantOperand("123")
                            ),
                            "$it => (Convert(Convert($it.IntProp)).ToString() == \"123\")",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new NotEqualsBinaryOperator
                            (
                                new ConvertToEnumOperator
                                (
                                    "123",
                                    typeof(SimpleEnum)
                                ),
                                new ConstantOperand(null)
                            ),
                            "$it => (Convert(123) != null)",
                            parameters
                        }
                    ),
                };
            }
        }

        [Theory]
        [MemberData(nameof(CastMethod_Succeeds_Data))]
        public void CastMethod_Succeeds(IExpressionPart filterBody, string expectedResult, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter, expectedResult);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }
        #endregion Casts

        #region 'isof' in query option
        public static List<object[]> IsofMethod_Succeeds_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(short)
                            ),
                            "$it => IIF(($it Is System.Int16), True, False)",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(Product)
                            ),
                            "$it => IIF(($it Is LogicBuilder.Expressions.Utils.Tests.Data.Product), True, False)",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new MemberSelector("ProductName", new ParameterOperator(parameters, parameterName)),
                                typeof(string)
                            ),
                            "$it => IIF(($it.ProductName Is System.String), True, False)",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName)),
                                typeof(Category)
                            ),
                            "$it => IIF(($it.Category Is LogicBuilder.Expressions.Utils.Tests.Data.Category), True, False)",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName)),
                                typeof(DerivedCategory)
                            ),
                            "$it => IIF(($it.Category Is LogicBuilder.Expressions.Utils.Tests.Data.DerivedCategory), True, False)",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new MemberSelector("Ranking", new ParameterOperator(parameters, parameterName)),
                                typeof(SimpleEnum)
                            ),
                            "$it => IIF(($it.Ranking Is LogicBuilder.Expressions.Utils.Tests.Data.SimpleEnum), True, False)",
                            parameters
                        }
                    ),
                };
            }
        }

        [Theory]
        [MemberData(nameof(IsofMethod_Succeeds_Data))]
        public void IsofMethod_Succeeds(IExpressionPart filterBody, string expectedExpression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expectedExpression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> IsOfPrimitiveType_Succeeds_WithFalse_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(byte[])
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(bool)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(byte)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(DateTimeOffset)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(Decimal)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(double)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(TimeSpan)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(Guid)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(Int16)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(Int32)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(Int64)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(sbyte)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(Single)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(System.IO.Stream)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(string)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(SimpleEnum)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(FlagsEnum)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new MemberSelector("ByteArrayProp", new ParameterOperator(parameters, parameterName)),
                                typeof(byte[])
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new MemberSelector("IntProp", new ParameterOperator(parameters, parameterName)),
                                typeof(SimpleEnum)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new MemberSelector("NullableShortProp", new ParameterOperator(parameters, parameterName)),
                                typeof(short)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(byte[])
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(bool)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(byte)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(DateTimeOffset)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(Decimal)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(double)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(TimeSpan)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(Guid)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(Int16)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(Int32)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(Int64)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(sbyte)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(Single)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(System.IO.Stream)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(string)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(SimpleEnum)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(FlagsEnum)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(23),
                                typeof(byte)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(23),
                                typeof(decimal)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(23),
                                typeof(double)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(23),
                                typeof(short)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(23),
                                typeof(long)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(23),
                                typeof(sbyte)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(23),
                                typeof(float)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand("hello"),
                                typeof(Stream)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(0),
                                typeof(FlagsEnum)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(0),
                                typeof(SimpleEnum)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand("2001-01-01T12:00:00.000+08:00"),
                                typeof(DateTimeOffset)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand("00000000-0000-0000-0000-000000000000"),
                                typeof(Guid)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand("23"),
                                typeof(byte)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand("23"),
                                typeof(short)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand("23"),
                                typeof(int)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand("false"),
                                typeof(bool)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand("OData"),
                                typeof(byte[])
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand("PT12H'"),
                                typeof(TimeSpan)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(23),
                                typeof(string)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand("0"),
                                typeof(FlagsEnum)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand("0"),
                                typeof(SimpleEnum)
                            ),
                            parameters
                        }
                    ),
                };
            }
        }

        [Theory]
        [MemberData(nameof(IsOfPrimitiveType_Succeeds_WithFalse_Data))]
        public void IsOfPrimitiveType_Succeeds_WithFalse(IExpressionPart filterBody, IDictionary<string, ParameterExpression> parameters)
        {
            //arrange
            var model = new DataTypes();

            //act
            var filter = CreateFilter<DataTypes>();
            bool result = RunFilter(filter, model);

            //assert
            Assert.False(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> IsOfQuotedNonPrimitiveType
        {
            get
            {
                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(DerivedProduct)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new MemberSelector("SupplierAddress", new ParameterOperator(parameters, parameterName)),
                                typeof(Address)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new MemberSelector("Category", new ParameterOperator(parameters, parameterName)),
                                typeof(DerivedCategory)
                            ),
                            parameters
                        }
                    ),
                };
            }
        }

        [Theory]
        [MemberData(nameof(IsOfQuotedNonPrimitiveType))]
        public void IsOfQuotedNonPrimitiveType_Succeeds(IExpressionPart filterBody, IDictionary<string, ParameterExpression> parameters)
        {
            //arrange
            var model = new DerivedProduct
            {
                SupplierAddress = new Address { City = "Redmond", },
                Category = new DerivedCategory { DerivedCategoryName = "DerivedCategory" }
            };

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, model);

            //assert
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> IsOfQuotedNonPrimitiveTypeWithNull_Succeeds_WithFalse_Data
        {
            get
            {
                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(Address)
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperand(null),
                                typeof(DerivedCategory)
                            ),
                            parameters
                        }
                    ),
                };
            }
        }
        [Theory]
        [MemberData(nameof(IsOfQuotedNonPrimitiveTypeWithNull_Succeeds_WithFalse_Data))]
        public void IsOfQuotedNonPrimitiveTypeWithNull_Succeeds_WithFalse(IExpressionPart filterBody, IDictionary<string, ParameterExpression> parameters)
        {
            //arrange
            var model = new DerivedProduct
            {
                SupplierAddress = new Address { City = "Redmond", },
                Category = new DerivedCategory { DerivedCategoryName = "DerivedCategory" }
            };

            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, model);

            //assert
            Assert.False(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }
        #endregion 'isof' in query option

        public static List<object[]> ByteArrayComparisons_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new EqualsBinaryOperator
                            (
                                new MemberSelector("ByteArrayProp", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperand(Convert.FromBase64String("I6v/"))
                            ),
                            "$it => ($it.ByteArrayProp == System.Byte[])",
                            true,
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new NotEqualsBinaryOperator
                            (
                                new MemberSelector("ByteArrayProp", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperand(Convert.FromBase64String("I6v/"))
                            ),
                            "$it => ($it.ByteArrayProp != System.Byte[])",
                            false,
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new EqualsBinaryOperator
                            (
                                new ConstantOperand(Convert.FromBase64String("I6v/")),
                                new ConstantOperand(Convert.FromBase64String("I6v/"))
                            ),
                            "$it => (System.Byte[] == System.Byte[])",
                            true,
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new NotEqualsBinaryOperator
                            (
                                new ConstantOperand(Convert.FromBase64String("I6v/")),
                                new ConstantOperand(Convert.FromBase64String("I6v/"))
                            ),
                            "$it => (System.Byte[] != System.Byte[])",
                            false,
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new NotEqualsBinaryOperator
                            (
                                new MemberSelector("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperand(Convert.FromBase64String("I6v/"))
                            ),
                            "$it => ($it.ByteArrayPropWithNullValue != System.Byte[])",
                            true,
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new NotEqualsBinaryOperator
                            (
                                new MemberSelector("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName)),
                                new MemberSelector("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName))
                            ),
                            "$it => ($it.ByteArrayPropWithNullValue != $it.ByteArrayPropWithNullValue)",
                            false,
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new NotEqualsBinaryOperator
                            (
                                new MemberSelector("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperand(null)
                            ),
                            "$it => ($it.ByteArrayPropWithNullValue != null)",
                            false,
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new EqualsBinaryOperator
                            (
                                new MemberSelector("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperand(null)
                            ),
                            "$it => ($it.ByteArrayPropWithNullValue == null)",
                            true,
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new NotEqualsBinaryOperator
                            (
                                new ConstantOperand(null),
                                new MemberSelector("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName))
                            ),
                            "$it => (null != $it.ByteArrayPropWithNullValue)",
                            false,
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new EqualsBinaryOperator
                            (
                                new ConstantOperand(null),
                                new MemberSelector("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName))
                            ),
                            "$it => (null == $it.ByteArrayPropWithNullValue)",
                            true,
                            parameters
                        }
                    ),
                };
            }
        }

        [Theory]
        [MemberData(nameof(ByteArrayComparisons_Data))]
        public void ByteArrayComparisons(IExpressionPart filterBody, string expectedExpression, bool expected, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<DataTypes>();
            bool result = RunFilter
            (
                filter,
                new DataTypes
                {
                    ByteArrayProp = new byte[] { 35, 171, 255 }
                }
            );

            //assert
            Assert.Equal(expected, result);
            AssertFilterStringIsCorrect(filter, expectedExpression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> DisAllowed_ByteArrayComparisons_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new GreaterThanOrEqualsBinaryOperator
                            (
                                new ConstantOperand(Convert.FromBase64String("AP8Q")),
                                new ConstantOperand(Convert.FromBase64String("AP8Q"))
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new LessThanOrEqualsBinaryOperator
                            (
                                new ConstantOperand(Convert.FromBase64String("AP8Q")),
                                new ConstantOperand(Convert.FromBase64String("AP8Q"))
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new LessThanBinaryOperator
                            (
                                new ConstantOperand(Convert.FromBase64String("AP8Q")),
                                new ConstantOperand(Convert.FromBase64String("AP8Q"))
                            ),
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new GreaterThanBinaryOperator
                            (
                                new ConstantOperand(Convert.FromBase64String("AP8Q")),
                                new ConstantOperand(Convert.FromBase64String("AP8Q"))
                            ),
                            parameters
                        }
                    ),
                };
            }
        }

        [Theory]
        [MemberData(nameof(DisAllowed_ByteArrayComparisons_Data))]
        public void DisAllowed_ByteArrayComparisons(IExpressionPart filterBody, IDictionary<string, ParameterExpression> parameters)
        {
            //assert
            Assert.Throws<InvalidOperationException>(() => CreateFilter<DataTypes>());
            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        public static List<object[]> Nullable_NonstandardEdmPrimitives_Data
        {
            get
            {
                var parameters = GetParameters();

                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertOperand
                                (
                                    new ConvertToNullableUnderlyingValueOperator
                                    (
                                        new MemberSelector("NullableUShortProp", new ParameterOperator(parameters, parameterName))
                                    ),
                                    typeof(int?)
                                ),
                                new ConstantOperand(12)
                            ),
                            "$it => (Convert($it.NullableUShortProp.Value) == Convert(12))",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertOperand
                                (
                                    new ConvertToNullableUnderlyingValueOperator
                                    (
                                        new MemberSelector("NullableULongProp", new ParameterOperator(parameters, parameterName))
                                    ),
                                    typeof(long?)
                                ),
                                new ConstantOperand(12l)
                            ),
                            "$it => (Convert($it.NullableULongProp.Value) == Convert(12))",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertOperand
                                (
                                    new ConvertToNullableUnderlyingValueOperator
                                    (
                                        new MemberSelector("NullableUIntProp", new ParameterOperator(parameters, parameterName))
                                    ),
                                    typeof(int?)
                                ),
                                new ConstantOperand(12)
                            ),
                            "$it => (Convert($it.NullableUIntProp.Value) == Convert(12))",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertToStringOperator
                                (
                                    new ConvertToNullableUnderlyingValueOperator
                                    (
                                        new MemberSelector("NullableCharProp", new ParameterOperator(parameters, parameterName))
                                    )
                                ),
                                new ConstantOperand("a")
                            ),
                            "$it => ($it.NullableCharProp.Value.ToString() == \"a\")",
                            parameters
                        }
                    ),
                };
            }
        }

        [Theory]
        [MemberData(nameof(Nullable_NonstandardEdmPrimitives_Data))]
        public void Nullable_NonstandardEdmPrimitives(IExpressionPart filterBody, string expectedExpression, IDictionary<string, ParameterExpression> parameters)
        {
            //act
            var filter = CreateFilter<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter, expectedExpression);
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new DataTypes()));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody,
                    parameters
                );
        }

        private string PadRightInstance(string str, int number)
        {
            return str.PadRight(number);
        }

        // Used by Custom Method binder tests - by reflection
        private static string PadRightStatic(string str, int number)
        {
            return str.PadRight(number);
        }

        private T? ToNullable<T>(object value) where T : struct => 
            value == null ? null : (T?)Convert.ChangeType(value, typeof(T));

        private static IDictionary<string, ParameterExpression> GetParameters()
            => new Dictionary<string, ParameterExpression>();

        static object[] GetArguments(IDictionary<string, ParameterExpression> parameters, Func<IDictionary<string, ParameterExpression>, object[]> getList) 
            => getList(parameters);

        static object[] GetArguments(Func<IDictionary<string, ParameterExpression>, object[]> getList) 
            => GetArguments(GetParameters(), getList);

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

    public static class StringExtender
    {
        public static string PadRightExStatic(this string str, int width)
        {
            return str.PadRight(width);
        }
    }
}
