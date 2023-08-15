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
        private readonly IDictionary<string, ParameterExpression> parameters;

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
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator(null)
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
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator("Doritos", typeof(string))
                    ),
                    parameters
                );
        }

        [Theory]
        [InlineData((byte)1, false)]
        [InlineData((byte)2, false)]
        [InlineData((byte)3, true)]
        public void EqualityOperatorWithChangedType(byte id, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { ProductID = id });

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductID == 3)");
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelectorOperator("ProductID", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator((byte)3, typeof(int))
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
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator("Doritos", typeof(string))
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
                        new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator(5.00m, typeof(decimal))
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
                        new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator(5.00m, typeof(decimal))
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
                        new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator(5.00m, typeof(decimal))
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
                        new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator(5.00m, typeof(decimal))
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
                        new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator(-5.00m, typeof(decimal))
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
                            new MemberSelectorOperator("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp == $it.DateTimeOffsetProp)",
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new MemberSelectorOperator("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp != $it.DateTimeOffsetProp)",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new MemberSelectorOperator("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeOffsetProp >= $it.DateTimeOffsetProp)",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanOrEqualsBinaryOperator
                        (
                            new MemberSelectorOperator("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
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
                            new MemberSelectorOperator("DateTimeProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("DateTimeProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeProp == $it.DateTimeProp)",
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new MemberSelectorOperator("DateTimeProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("DateTimeProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeProp != $it.DateTimeProp)",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new MemberSelectorOperator("DateTimeProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("DateTimeProp", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => ($it.DateTimeProp >= $it.DateTimeProp)",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanOrEqualsBinaryOperator
                        (
                            new MemberSelectorOperator("DateTimeProp", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("DateTimeProp", new ParameterOperator(parameters, parameterName))
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(5.00m, typeof(decimal))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(0, typeof(int))
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
                        new MemberSelectorOperator("Discontinued", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator(true, typeof(bool))
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
                        new MemberSelectorOperator("Discontinued", new ParameterOperator(parameters, parameterName)),
                        new MemberSelectorOperator("Discontinued", new ParameterOperator(parameters, parameterName))
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(5.00m, typeof(decimal))
                        ),
                        new EqualsBinaryOperator
                        (
                            new ConvertOperator(new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)), typeof(int?)),
                            new ConstantOperator(0, typeof(int))
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(5.00m, typeof(decimal))
                        ),
                        new EqualsBinaryOperator
                        (
                            new ConvertOperator(new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)), typeof(decimal?)),
                            new ConstantOperator(10.00m, typeof(decimal))
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(5.00m, typeof(decimal))
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
                        new MemberSelectorOperator("Discontinued", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("Discontinued", new ParameterOperator(parameters, parameterName))
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(1.00m, typeof(decimal))
                        ),
                        new ConstantOperator(5.00m, typeof(decimal))
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(1.00m, typeof(decimal))
                        ),
                        new ConstantOperator(5.00m, typeof(decimal))
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(1.00m, typeof(decimal))
                        ),
                        new ConstantOperator(5.00m, typeof(decimal))
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(1.00m, typeof(decimal))
                        ),
                        new ConstantOperator(5.00m, typeof(decimal))
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(1.00m, typeof(decimal))
                        ),
                        new ConstantOperator(5.00m, typeof(decimal))
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
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
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
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
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
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
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
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
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
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
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
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                                new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                            ),
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                                new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                            ),
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                                new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                            ),
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                                new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                            ),
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                                new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
                            ),
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName))
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
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
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
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new MemberSelectorOperator("UnitsOnOrder", new ParameterOperator(parameters, parameterName))
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
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(null)
                        ),
                        null,
                        true,
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new MemberSelectorOperator("UnitsInStock", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(null)
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
                                new ConstantOperator("hello"),
                                new MemberSelectorOperator("StringProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertOperator
                            (
                                new MemberSelectorOperator("UIntProp", new ParameterOperator(parameters, parameterName)),
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
                                new ConstantOperator("hello"),
                                new MemberSelectorOperator("StringProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertOperator
                            (
                                new MemberSelectorOperator("ULongProp", new ParameterOperator(parameters, parameterName)),
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
                                new ConstantOperator("hello"),
                                new MemberSelectorOperator("StringProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertOperator
                            (
                                new MemberSelectorOperator("UShortProp", new ParameterOperator(parameters, parameterName)),
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
                                new ConstantOperator("hello"),
                                new MemberSelectorOperator("StringProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertOperator
                            (
                                new MemberSelectorOperator("NullableUShortProp", new ParameterOperator(parameters, parameterName)),
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
                                new ConstantOperator("hello"),
                                new MemberSelectorOperator("StringProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertOperator
                            (
                                new MemberSelectorOperator("NullableUIntProp", new ParameterOperator(parameters, parameterName)),
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
                                new ConstantOperator("hello"),
                                new MemberSelectorOperator("StringProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertOperator
                            (
                                new MemberSelectorOperator("NullableULongProp", new ParameterOperator(parameters, parameterName)),
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator("Doritos")
                        ),
                        new LessThanBinaryOperator
                        (
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(5.00m, typeof(decimal))
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
                        new MemberSelectorOperator
                        (
                            "CategoryName",
                            new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator("Snacks")
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
                        new MemberSelectorOperator
                        (
                            "CategoryName",
                            new MemberSelectorOperator
                            (
                                "Category",
                                new MemberSelectorOperator
                                (
                                    "Product",
                                    new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                                )
                            )
                        ),
                        new ConstantOperator("Snacks")
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
                        new MemberSelectorOperator
                        (
                            "City",
                            new MemberSelectorOperator("SupplierAddress", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator("Redmond")
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
                        new MemberSelectorOperator
                        (
                            "EnumerableProducts",
                            new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "P")),
                            new ConstantOperator("Snacks")
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
                        new MemberSelectorOperator
                        (
                            "QueryableProducts",
                            new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "P")),
                            new ConstantOperator("Snacks")
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
                            new MemberSelectorOperator
                            (
                                "QueryableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelectorOperator("ProductID", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperator
                                (
                                    new List<object>{ 1 },
                                    typeof(int)
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
                            new MemberSelectorOperator
                            (
                                "EnumerableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelectorOperator("ProductID", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperator
                                (
                                    new List<object>{ 1 },
                                    typeof(int)
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
                            new MemberSelectorOperator
                            (
                                "QueryableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelectorOperator("GuidProperty", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperator
                                (
                                    new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") },
                                    typeof(Guid)
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
                            new MemberSelectorOperator
                            (
                                "EnumerableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelectorOperator("GuidProperty", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperator
                                (
                                    new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") },
                                    typeof(Guid)
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
                            new MemberSelectorOperator
                            (
                                "QueryableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelectorOperator("NullableGuidProperty", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperator
                                (
                                    new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") },
                                    typeof(Guid?)
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
                            new MemberSelectorOperator
                            (
                                "EnumerableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelectorOperator("NullableGuidProperty", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperator
                                (
                                    new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") },
                                    typeof(Guid?)
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
                            new MemberSelectorOperator
                            (
                                "QueryableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelectorOperator("Discontinued", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperator
                                (
                                    new List<object>{ false, null },
                                    typeof(bool?)
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
                            new MemberSelectorOperator
                            (
                                "EnumerableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new InOperator
                            (
                                new MemberSelectorOperator("Discontinued", new ParameterOperator(parameters, "P")),
                                new CollectionConstantOperator
                                (
                                    new List<object>{ false, null },
                                    typeof(bool?)
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
                            new MemberSelectorOperator
                            (
                                "QueryableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(false),
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
                            new MemberSelectorOperator
                            (
                                "QueryableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new AndBinaryOperator
                            (
                                new ConstantOperator(false),
                                new EqualsBinaryOperator
                                (
                                    new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "P")),
                                    new ConstantOperator("Snacks")
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
                            new MemberSelectorOperator
                            (
                                "QueryableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
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
                        new MemberSelectorOperator
                        (
                            "EnumerableProducts",
                            new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "P")),
                            new ConstantOperator("Snacks")
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
                        new MemberSelectorOperator
                        (
                            "EnumerableProducts",
                            new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "P")),
                            new ConstantOperator("Snacks")
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
                            new MemberSelectorOperator("AlternateIDs", new ParameterOperator(parameters, parameterName)),
                            new EqualsBinaryOperator
                            (
                                new ParameterOperator(parameters, "n"),
                                new ConstantOperator(42)
                            ),
                            "n"
                        ),
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelectorOperator("AlternateAddresses", new ParameterOperator(parameters, parameterName)),
                            new EqualsBinaryOperator
                            (
                                new MemberSelectorOperator("City", new ParameterOperator(parameters, "n")),
                                new ConstantOperator("Redmond")
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
                            new MemberSelectorOperator("AlternateIDs", new ParameterOperator(parameters, parameterName)),
                            new EqualsBinaryOperator
                            (
                                new ParameterOperator(parameters, "n"),
                                new ConstantOperator(42)
                            ),
                            "n"
                        ),
                        new AllOperator
                        (
                            parameters,
                            new MemberSelectorOperator("AlternateAddresses", new ParameterOperator(parameters, parameterName)),
                            new EqualsBinaryOperator
                            (
                                new MemberSelectorOperator("City", new ParameterOperator(parameters, "n")),
                                new ConstantOperator("Redmond")
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
                        new MemberSelectorOperator
                        (
                            "EnumerableProducts",
                            new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
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
                        new MemberSelectorOperator
                        (
                            "QueryableProducts",
                            new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
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
                        new MemberSelectorOperator
                        (
                            "EnumerableProducts",
                            new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "P")),
                            new ConstantOperator("Snacks")
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
                        new MemberSelectorOperator
                        (
                            "QueryableProducts",
                            new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "P")),
                            new ConstantOperator("Snacks")
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
                            new MemberSelectorOperator
                            (
                                "QueryableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new EqualsBinaryOperator
                            (
                                new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "P")),
                                new ConstantOperator("Snacks")
                            ),
                            "P"
                        ),
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelectorOperator
                            (
                                "QueryableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new EqualsBinaryOperator
                            (
                                new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "P2")),
                                new ConstantOperator("Snacks")
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
                            new MemberSelectorOperator
                            (
                                "QueryableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new EqualsBinaryOperator
                            (
                                new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "P")),
                                new ConstantOperator("Snacks")
                            ),
                            "P"
                        ),
                        new AllOperator
                        (
                            parameters,
                            new MemberSelectorOperator
                            (
                                "QueryableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            new EqualsBinaryOperator
                            (
                                new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "P2")),
                                new ConstantOperator("Snacks")
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
                        new MemberSelectorOperator("AlternateIDs", new ParameterOperator(parameters, parameterName)),
                        new EqualsBinaryOperator
                        (
                            new ParameterOperator(parameters, "id"),
                            new ConstantOperator(42)
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
                        new MemberSelectorOperator("AlternateIDs", new ParameterOperator(parameters, parameterName)),
                        new EqualsBinaryOperator
                        (
                            new ParameterOperator(parameters, "id"),
                            new ConstantOperator(42)
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
                        new MemberSelectorOperator("AlternateAddresses", new ParameterOperator(parameters, parameterName)),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("City", new ParameterOperator(parameters, "address")),
                            new ConstantOperator("Redmond")
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
                        new MemberSelectorOperator("AlternateAddresses", new ParameterOperator(parameters, parameterName)),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("City", new ParameterOperator(parameters, "address")),
                            new ConstantOperator("Redmond")
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
                        new MemberSelectorOperator
                        (
                            "QueryableProducts",
                            new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                        ),
                        new AnyOperator
                        (
                            parameters,
                            new MemberSelectorOperator
                            (
                                "EnumerableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, "P"))
                            ),
                            new EqualsBinaryOperator
                            (
                                new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "PP")),
                                new ConstantOperator("Snacks")
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(startIndex)
                        ),
                        new ConstantOperator(compareString)
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(startIndex)
                        ),
                        new ConstantOperator(compareString)
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(startIndex),
                            new ConstantOperator(length)
                        ),
                        new ConstantOperator(compareString)
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(startIndex),
                            new ConstantOperator(length)
                        ),
                        new ConstantOperator(compareString)
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
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator("Abc")
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
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator("Abc")
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
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator("Abc")
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
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator("Abc")
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
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator("Abc")
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
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator("Abc")
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(0)
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(0)
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator("Abc")
                        ),
                        new ConstantOperator(5)
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator("Abc")
                        ),
                        new ConstantOperator(5)
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(3)
                        ),
                        new ConstantOperator("uctName")
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(3),
                            new ConstantOperator(4)
                        ),
                        new ConstantOperator("uctN")
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(3)
                        ),
                        new ConstantOperator("uctName")
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(3),
                            new ConstantOperator(4)
                        ),
                        new ConstantOperator("uctN")
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator("tasty treats")
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator("tasty treats")
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator("TASTY TREATS")
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator("TASTY TREATS")
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator("Tasty Treats")
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
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator("Tasty Treats")
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
                            new ConstantOperator("Food"),
                            new ConstantOperator("Bar")
                        ),
                        new ConstantOperator("FoodBar")
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
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(8)
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
                            new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(8)
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
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(8)
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
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(1974)
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
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(8)
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
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(12)
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
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(33)
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
                                new MemberSelectorOperator("DiscontinuedOffset", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(100)
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
                                new MemberSelectorOperator("DiscontinuedOffset", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(100)
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
                                new MemberSelectorOperator("DiscontinuedOffset", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(100)
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
                                new MemberSelectorOperator("DiscontinuedOffset", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(100)
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
                                new MemberSelectorOperator("DiscontinuedOffset", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(100)
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
                                new MemberSelectorOperator("DiscontinuedOffset", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(100)
                        ),
                       "$it => ($it.DiscontinuedOffset.Second == 100)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new NowDateTimeOperator(),
                            new ConstantOperator(new DateTimeOffset(new DateTime(2016, 11, 8), new TimeSpan(0)))
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
                                new MemberSelectorOperator("Birthday", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(100)
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
                                new MemberSelectorOperator("Birthday", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(100)
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
                                new MemberSelectorOperator("Birthday", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(100)
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
                                new MemberSelectorOperator("Birthday", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(100)
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
                                new MemberSelectorOperator("Birthday", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(100)
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
                                new MemberSelectorOperator("Birthday", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(100)
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
                                new MemberSelectorOperator("NullableDateProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(2015)
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
                                new MemberSelectorOperator("NullableDateProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(12)
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
                                new MemberSelectorOperator("NullableDateProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(23)
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

        public static List<object[]> DateOnlyFunctions_Nullable_Data
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
                                new MemberSelectorOperator("NullableDateOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(2015)
                        ),
                       "$it => ($it.NullableDateOnlyProperty.Value.Year == 2015)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MonthOperator
                            (
                                new MemberSelectorOperator("NullableDateOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(12)
                        ),
                       "$it => ($it.NullableDateOnlyProperty.Value.Month == 12)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new DayOperator
                            (
                                new MemberSelectorOperator("NullableDateOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(23)
                        ),
                       "$it => ($it.NullableDateOnlyProperty.Value.Day == 23)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateOnlyFunctions_Nullable_Data))]
        public void DateOnlyFunctions_Nullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
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
                                new MemberSelectorOperator("DateProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(2015)
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
                                new MemberSelectorOperator("DateProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(12)
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
                                new MemberSelectorOperator("DateProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(23)
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

        public static List<object[]> DateOnlyFunctions_NonNullable_Data
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
                                new MemberSelectorOperator("DateOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(2015)
                        ),
                        "$it => ($it.DateOnlyProperty.Year == 2015)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MonthOperator
                            (
                                new MemberSelectorOperator("DateOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(12)
                        ),
                       "$it => ($it.DateOnlyProperty.Month == 12)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new DayOperator
                            (
                                new MemberSelectorOperator("DateOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(23)
                        ),
                       "$it => ($it.DateOnlyProperty.Day == 23)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateOnlyFunctions_NonNullable_Data))]
        public void DateOnlyFunctions_NonNullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
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
                                new MemberSelectorOperator("NullableTimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(10)
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
                                new MemberSelectorOperator("NullableTimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(20)
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
                                new MemberSelectorOperator("NullableTimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(30)
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

        public static List<object[]> TimeOnlyFunctions_Nullable_Data
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
                                new MemberSelectorOperator("NullableTimeOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(10)
                        ),
                        "$it => ($it.NullableTimeOnlyProperty.Value.Hour == 10)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MinuteOperator
                            (
                                new MemberSelectorOperator("NullableTimeOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(20)
                        ),
                       "$it => ($it.NullableTimeOnlyProperty.Value.Minute == 20)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new SecondOperator
                            (
                                new MemberSelectorOperator("NullableTimeOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(30)
                        ),
                       "$it => ($it.NullableTimeOnlyProperty.Value.Second == 30)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(TimeOnlyFunctions_Nullable_Data))]
        public void TimeOnlyFunctions_Nullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
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
                                new MemberSelectorOperator("TimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(10)
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
                                new MemberSelectorOperator("TimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(20)
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
                                new MemberSelectorOperator("TimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(30)
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

        public static List<object[]> TimeOnlyFunctions_NonNullable_Data
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
                                new MemberSelectorOperator("TimeOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(10)
                        ),
                        "$it => ($it.TimeOnlyProperty.Hour == 10)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MinuteOperator
                            (
                                new MemberSelectorOperator("TimeOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(20)
                        ),
                       "$it => ($it.TimeOnlyProperty.Minute == 20)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new SecondOperator
                            (
                                new MemberSelectorOperator("TimeOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(30)
                        ),
                       "$it => ($it.TimeOnlyProperty.Second == 30)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(TimeOnlyFunctions_NonNullable_Data))]
        public void TimeOnlyFunctions_NonNullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
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
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(0.2m)
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
                                new MemberSelectorOperator("NullableTimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(0.2m)
                        ),
                       "$it => ((Convert($it.NullableTimeOfDayProperty.Value.Milliseconds) / 1000) == 0.2)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new FractionalSecondsOperator
                            (
                                new MemberSelectorOperator("NullableTimeOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(0.2m)
                        ),
                       "$it => ((Convert($it.NullableTimeOnlyProperty.Value.Millisecond) / 1000) == 0.2)",
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
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(0.2m)
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
                                new MemberSelectorOperator("TimeOfDayProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(0.2m)
                        ),
                       "$it => ((Convert($it.TimeOfDayProperty.Milliseconds) / 1000) == 0.2)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new FractionalSecondsOperator
                            (
                                new MemberSelectorOperator("TimeOnlyProperty", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConstantOperator(0.2m)
                        ),
                       "$it => ((Convert($it.TimeOnlyProperty.Millisecond) / 1000) == 0.2)",
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
                            new ConvertToNumericDateOperator
                            (
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericDateOperator
                            (
                                new ConstantOperator(new Date(2015, 2, 26))
                            )
                        ),
                        "$it => (((($it.DiscontinuedDate.Value.Year * 10000) + ($it.DiscontinuedDate.Value.Month * 100)) + $it.DiscontinuedDate.Value.Day) == (((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanBinaryOperator
                        (
                            new ConvertToNumericDateOperator
                            (
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericDateOperator
                            (
                                new ConstantOperator(new Date(2016, 2, 26))
                            )
                        ),
                        "$it => (((($it.DiscontinuedDate.Value.Year * 10000) + ($it.DiscontinuedDate.Value.Month * 100)) + $it.DiscontinuedDate.Value.Day) < (((2016-02-26.Year * 10000) + (2016-02-26.Month * 100)) + 2016-02-26.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericDateOperator
                            (
                                new ConstantOperator(new Date(2015, 2, 26))
                            ),
                            new ConvertToNumericDateOperator
                            (
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        "$it => ((((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day) >= ((($it.DiscontinuedDate.Value.Year * 10000) + ($it.DiscontinuedDate.Value.Month * 100)) + $it.DiscontinuedDate.Value.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new ConstantOperator(null),
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => (null != $it.DiscontinuedDate)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(null)
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

        public static List<object[]> DateOnlyFunction_Nullable_Data
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
                            new ConvertToNumericDateOperator
                            (
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericDateOperator
                            (
                                new ConstantOperator(new DateOnly(2015, 2, 26))
                            )
                        ),
                        "$it => (((($it.DiscontinuedDate.Value.Year * 10000) + ($it.DiscontinuedDate.Value.Month * 100)) + $it.DiscontinuedDate.Value.Day) == (((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanBinaryOperator
                        (
                            new ConvertToNumericDateOperator
                            (
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericDateOperator
                            (
                                new ConstantOperator(new DateOnly(2016, 2, 26))
                            )
                        ),
                        "$it => (((($it.DiscontinuedDate.Value.Year * 10000) + ($it.DiscontinuedDate.Value.Month * 100)) + $it.DiscontinuedDate.Value.Day) < (((2016-02-26.Year * 10000) + (2016-02-26.Month * 100)) + 2016-02-26.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericDateOperator
                            (
                                new ConstantOperator(new DateOnly(2015, 2, 26))
                            ),
                            new ConvertToNumericDateOperator
                            (
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        "$it => ((((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day) >= ((($it.DiscontinuedDate.Value.Year * 10000) + ($it.DiscontinuedDate.Value.Month * 100)) + $it.DiscontinuedDate.Value.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new ConstantOperator(null),
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => (null != $it.DiscontinuedDate)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(null)
                        ),
                        "$it => ($it.DiscontinuedDate == null)",
                        parameters
                    },
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateOnlyFunction_Nullable_Data))]
        public void DateOnlyFunction_Nullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
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
                            new ConvertToNumericDateOperator
                            (
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericDateOperator
                            (
                                new ConstantOperator(new Date(2015, 2, 26))
                            )
                        ),
                        "$it => (((($it.NonNullableDiscontinuedDate.Year * 10000) + ($it.NonNullableDiscontinuedDate.Month * 100)) + $it.NonNullableDiscontinuedDate.Day) == (((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanBinaryOperator
                        (
                            new ConvertToNumericDateOperator
                            (
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericDateOperator
                            (
                                new ConstantOperator(new Date(2016, 2, 26))
                            )
                        ),
                        "$it => (((($it.NonNullableDiscontinuedDate.Year * 10000) + ($it.NonNullableDiscontinuedDate.Month * 100)) + $it.NonNullableDiscontinuedDate.Day) < (((2016-02-26.Year * 10000) + (2016-02-26.Month * 100)) + 2016-02-26.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericDateOperator
                            (
                                new ConstantOperator(new Date(2015, 2, 26))
                            ),
                            new ConvertToNumericDateOperator
                            (
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
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

        public static List<object[]> DateOnlyFunction_NonNullable_Data
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
                            new ConvertToNumericDateOperator
                            (
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericDateOperator
                            (
                                new ConstantOperator(new DateOnly(2015, 2, 26))
                            )
                        ),
                        "$it => (((($it.NonNullableDiscontinuedDate.Year * 10000) + ($it.NonNullableDiscontinuedDate.Month * 100)) + $it.NonNullableDiscontinuedDate.Day) == (((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanBinaryOperator
                        (
                            new ConvertToNumericDateOperator
                            (
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericDateOperator
                            (
                                new ConstantOperator(new DateOnly(2016, 2, 26))
                            )
                        ),
                        "$it => (((($it.NonNullableDiscontinuedDate.Year * 10000) + ($it.NonNullableDiscontinuedDate.Month * 100)) + $it.NonNullableDiscontinuedDate.Day) < (((2016-02-26.Year * 10000) + (2016-02-26.Month * 100)) + 2016-02-26.Day))",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericDateOperator
                            (
                                new ConstantOperator(new DateOnly(2015, 2, 26))
                            ),
                            new ConvertToNumericDateOperator
                            (
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        "$it => ((((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day) >= ((($it.NonNullableDiscontinuedDate.Year * 10000) + ($it.NonNullableDiscontinuedDate.Month * 100)) + $it.NonNullableDiscontinuedDate.Day))",
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(DateOnlyFunction_NonNullable_Data))]
        public void DateOnlyFunction_NonNullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
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
                            new ConvertToNumericTimeOperator
                            (
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericTimeOperator
                            (
                                new ConstantOperator(new TimeOfDay(1, 2, 3, 4))
                            )
                        ),
                        "$it => (((Convert($it.DiscontinuedDate.Value.Hour) * 36000000000) + ((Convert($it.DiscontinuedDate.Value.Minute) * 600000000) + ((Convert($it.DiscontinuedDate.Value.Second) * 10000000) + Convert($it.DiscontinuedDate.Value.Millisecond)))) == ((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))))",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericTimeOperator
                            (
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericTimeOperator
                            (
                                new ConstantOperator(new TimeOfDay(1, 2, 3, 4))
                            )
                        ),
                        "$it => (((Convert($it.DiscontinuedDate.Value.Hour) * 36000000000) + ((Convert($it.DiscontinuedDate.Value.Minute) * 600000000) + ((Convert($it.DiscontinuedDate.Value.Second) * 10000000) + Convert($it.DiscontinuedDate.Value.Millisecond)))) >= ((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))))",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericTimeOperator
                            (
                                new ConstantOperator(new TimeOfDay(1, 2, 3, 4))
                            ),
                            new ConvertToNumericTimeOperator
                            (
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        "$it => (((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))) <= ((Convert($it.DiscontinuedDate.Value.Hour) * 36000000000) + ((Convert($it.DiscontinuedDate.Value.Minute) * 600000000) + ((Convert($it.DiscontinuedDate.Value.Second) * 10000000) + Convert($it.DiscontinuedDate.Value.Millisecond)))))",
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new ConstantOperator(null),
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => (null != $it.DiscontinuedDate)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(null)
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

        public static List<object[]> TimeOnlyFunction_Nullable_Data
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
                            new ConvertToNumericTimeOperator
                            (
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericTimeOperator
                            (
                                new ConstantOperator(new TimeOnly(1, 2, 3, 4))
                            )
                        ),
                        "$it => (((Convert($it.DiscontinuedDate.Value.Hour) * 36000000000) + ((Convert($it.DiscontinuedDate.Value.Minute) * 600000000) + ((Convert($it.DiscontinuedDate.Value.Second) * 10000000) + Convert($it.DiscontinuedDate.Value.Millisecond)))) == ((Convert(01:02:03.0040000.Hour) * 36000000000) + ((Convert(01:02:03.0040000.Minute) * 600000000) + ((Convert(01:02:03.0040000.Second) * 10000000) + Convert(01:02:03.0040000.Millisecond)))))",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericTimeOperator
                            (
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericTimeOperator
                            (
                                new ConstantOperator(new TimeOnly(1, 2, 3, 4))
                            )
                        ),
                        "$it => (((Convert($it.DiscontinuedDate.Value.Hour) * 36000000000) + ((Convert($it.DiscontinuedDate.Value.Minute) * 600000000) + ((Convert($it.DiscontinuedDate.Value.Second) * 10000000) + Convert($it.DiscontinuedDate.Value.Millisecond)))) >= ((Convert(01:02:03.0040000.Hour) * 36000000000) + ((Convert(01:02:03.0040000.Minute) * 600000000) + ((Convert(01:02:03.0040000.Second) * 10000000) + Convert(01:02:03.0040000.Millisecond)))))",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericTimeOperator
                            (
                                new ConstantOperator(new TimeOnly(1, 2, 3, 4))
                            ),
                            new ConvertToNumericTimeOperator
                            (
                                new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        "$it => (((Convert(01:02:03.0040000.Hour) * 36000000000) + ((Convert(01:02:03.0040000.Minute) * 600000000) + ((Convert(01:02:03.0040000.Second) * 10000000) + Convert(01:02:03.0040000.Millisecond)))) <= ((Convert($it.DiscontinuedDate.Value.Hour) * 36000000000) + ((Convert($it.DiscontinuedDate.Value.Minute) * 600000000) + ((Convert($it.DiscontinuedDate.Value.Second) * 10000000) + Convert($it.DiscontinuedDate.Value.Millisecond)))))",
                        parameters
                    },
                    new object[]
                    {
                        new NotEqualsBinaryOperator
                        (
                            new ConstantOperator(null),
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName))
                        ),
                        "$it => (null != $it.DiscontinuedDate)",
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("DiscontinuedDate", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(null)
                        ),
                        "$it => ($it.DiscontinuedDate == null)",
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TimeOnlyFunction_Nullable_Data))]
        public void TimeOnlyFunction_Nullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
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
                            new ConvertToNumericTimeOperator
                            (
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericTimeOperator
                            (
                                new ConstantOperator(new TimeOfDay(1, 2, 3, 4))
                            )
                        ),
                        "$it => (((Convert($it.NonNullableDiscontinuedDate.Hour) * 36000000000) + ((Convert($it.NonNullableDiscontinuedDate.Minute) * 600000000) + ((Convert($it.NonNullableDiscontinuedDate.Second) * 10000000) + Convert($it.NonNullableDiscontinuedDate.Millisecond)))) == ((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))))",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericTimeOperator
                            (
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericTimeOperator
                            (
                                new ConstantOperator(new TimeOfDay(1, 2, 3, 4))
                            )
                        ),
                        "$it => (((Convert($it.NonNullableDiscontinuedDate.Hour) * 36000000000) + ((Convert($it.NonNullableDiscontinuedDate.Minute) * 600000000) + ((Convert($it.NonNullableDiscontinuedDate.Second) * 10000000) + Convert($it.NonNullableDiscontinuedDate.Millisecond)))) >= ((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))))",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericTimeOperator
                            (
                                new ConstantOperator(new TimeOfDay(1, 2, 3, 4))
                            ),
                            new ConvertToNumericTimeOperator
                            (
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
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

        public static List<object[]> TimeOnlyFunction_NonNullable_Data
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
                            new ConvertToNumericTimeOperator
                            (
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericTimeOperator
                            (
                                new ConstantOperator(new TimeOnly(1, 2, 3, 4))
                            )
                        ),
                        "$it => (((Convert($it.NonNullableDiscontinuedDate.Hour) * 36000000000) + ((Convert($it.NonNullableDiscontinuedDate.Minute) * 600000000) + ((Convert($it.NonNullableDiscontinuedDate.Second) * 10000000) + Convert($it.NonNullableDiscontinuedDate.Millisecond)))) == ((Convert(01:02:03.0040000.Hour) * 36000000000) + ((Convert(01:02:03.0040000.Minute) * 600000000) + ((Convert(01:02:03.0040000.Second) * 10000000) + Convert(01:02:03.0040000.Millisecond)))))",
                        parameters
                    },
                    new object[]
                    {
                        new GreaterThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericTimeOperator
                            (
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            ),
                            new ConvertToNumericTimeOperator
                            (
                                new ConstantOperator(new TimeOnly(1, 2, 3, 4))
                            )
                        ),
                        "$it => (((Convert($it.NonNullableDiscontinuedDate.Hour) * 36000000000) + ((Convert($it.NonNullableDiscontinuedDate.Minute) * 600000000) + ((Convert($it.NonNullableDiscontinuedDate.Second) * 10000000) + Convert($it.NonNullableDiscontinuedDate.Millisecond)))) >= ((Convert(01:02:03.0040000.Hour) * 36000000000) + ((Convert(01:02:03.0040000.Minute) * 600000000) + ((Convert(01:02:03.0040000.Second) * 10000000) + Convert(01:02:03.0040000.Millisecond)))))",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanOrEqualsBinaryOperator
                        (
                            new ConvertToNumericTimeOperator
                            (
                                new ConstantOperator(new TimeOnly(1, 2, 3, 4))
                            ),
                            new ConvertToNumericTimeOperator
                            (
                                new MemberSelectorOperator("NonNullableDiscontinuedDate", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        "$it => (((Convert(01:02:03.0040000.Hour) * 36000000000) + ((Convert(01:02:03.0040000.Minute) * 600000000) + ((Convert(01:02:03.0040000.Second) * 10000000) + Convert(01:02:03.0040000.Millisecond)))) <= ((Convert($it.NonNullableDiscontinuedDate.Hour) * 36000000000) + ((Convert($it.NonNullableDiscontinuedDate.Minute) * 600000000) + ((Convert($it.NonNullableDiscontinuedDate.Second) * 10000000) + Convert($it.NonNullableDiscontinuedDate.Millisecond)))))",
                        parameters
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(TimeOnlyFunction_NonNullable_Data))]
        public void TimeOnlyFunction_NonNullable(IExpressionPart filterBody, string expression, IDictionary<string, ParameterExpression> parameters)
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
                                new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        new ConstantOperator(123m)
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
                                new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName))
                            )
                        ),
                        new ConstantOperator(123m)
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(5.00m)
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(5.00m)
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
                            new MemberSelectorOperator("Weight", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(5d)
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
                            new MemberSelectorOperator("Weight", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(5d)
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
                            new ConvertOperator(new MemberSelectorOperator("Width", new ParameterOperator(parameters, parameterName)), typeof(double?))
                        ),
                        new ConstantOperator(5d)
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
                            new ConvertOperator(new MemberSelectorOperator("Width", new ParameterOperator(parameters, parameterName)), typeof(double?))
                        ),
                        new ConstantOperator(5d)
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(5m)
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(5m)
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
                            new MemberSelectorOperator("Weight", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(5d)
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
                            new MemberSelectorOperator("Weight", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(5d)
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
                            new ConvertOperator(new MemberSelectorOperator("Width", new ParameterOperator(parameters, parameterName)), typeof(double?))
                        ),
                        new ConstantOperator(5d)
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
                            new ConvertOperator(new MemberSelectorOperator("Width", new ParameterOperator(parameters, parameterName)), typeof(double?))
                        ),
                        new ConstantOperator(5d)
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(5m)
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
                            new MemberSelectorOperator("UnitPrice", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(5m)
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
                            new MemberSelectorOperator("Weight", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(5d)
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
                            new MemberSelectorOperator("Weight", new ParameterOperator(parameters, parameterName))
                        ),
                        new ConstantOperator(5d)
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
                            new ConvertOperator(new MemberSelectorOperator("Width", new ParameterOperator(parameters, parameterName)), typeof(double?))
                        ),
                        new ConstantOperator(5d)
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
                            new ConvertOperator(new MemberSelectorOperator("Width", new ParameterOperator(parameters, parameterName)), typeof(double?))
                        ),
                        new ConstantOperator(5d)
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
                                new ConvertOperator(new MemberSelectorOperator("FloatProp", new ParameterOperator(parameters, parameterName)), typeof(double))
                            ),
                            new FloorOperator
                            (
                                new ConvertOperator(new MemberSelectorOperator("FloatProp", new ParameterOperator(parameters, parameterName)), typeof(double))
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
                                new ConvertOperator(new MemberSelectorOperator("FloatProp", new ParameterOperator(parameters, parameterName)), typeof(double))
                            ),
                            new RoundOperator
                            (
                                new ConvertOperator(new MemberSelectorOperator("FloatProp", new ParameterOperator(parameters, parameterName)), typeof(double))
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
                                new ConvertOperator(new MemberSelectorOperator("FloatProp", new ParameterOperator(parameters, parameterName)), typeof(double))
                            ),
                            new CeilingOperator
                            (
                                new ConvertOperator(new MemberSelectorOperator("FloatProp", new ParameterOperator(parameters, parameterName)), typeof(double))
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
                                new MemberSelectorOperator("DoubleProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new FloorOperator
                            (
                                new MemberSelectorOperator("DoubleProp", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("DoubleProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new RoundOperator
                            (
                                new MemberSelectorOperator("DoubleProp", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("DoubleProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new CeilingOperator
                            (
                                new MemberSelectorOperator("DoubleProp", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("DecimalProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new FloorOperator
                            (
                                new MemberSelectorOperator("DecimalProp", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("DecimalProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new RoundOperator
                            (
                                new MemberSelectorOperator("DecimalProp", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("DecimalProp", new ParameterOperator(parameters, parameterName))
                            ),
                            new CeilingOperator
                            (
                                new MemberSelectorOperator("DecimalProp", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperator(totalWidth)
                            }
                        ),
                        new ConstantOperator(expectedProductName)
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
                                new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperator(totalWidth)
                            }
                        ),
                        new ConstantOperator(expectedProductName)
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
                            typeof(FilterTests).GetMethod(nameof(PadRightStatic), BindingFlags.NonPublic | BindingFlags.Static),
                            new IExpressionPart[]
                            {
                                new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperator(totalWidth)
                            }
                        ),
                        new ConstantOperator(expectedProductName)
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
                        new MemberSelectorOperator("GuidProp", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator(new Guid("0EFDAECF-A9F0-42F3-A384-1295917AF95E"))
                    ),
                    parameters
                );

            Expression<Func<T, bool>> CreateFilter2<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelectorOperator("GuidProp", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator(new Guid("0efdaecf-a9f0-42f3-a384-1295917af95e"))
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
                            new MemberSelectorOperator("DateTimeProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(new DateTimeOffset(new DateTime(2000, 12, 12, 12, 0, 0), TimeSpan.Zero))
                        ),
                       "$it => ($it.DateTimeProp == {0})",
                        parameters
                    },
                    new object[]
                    {
                        new LessThanBinaryOperator
                        (
                            new MemberSelectorOperator("DateTimeProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(new DateTimeOffset(new DateTime(2000, 12, 12, 12, 0, 0), TimeSpan.Zero))
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
                            new MemberSelectorOperator("LongProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator((long)987654321, typeof(long))
                        ),
                        new GreaterThanBinaryOperator
                        (
                            new MemberSelectorOperator("LongProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator((long)123456789, typeof(long))
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
                            new MemberSelectorOperator("LongProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator((long)-987654321, typeof(long))
                        ),
                        new GreaterThanBinaryOperator
                        (
                            new MemberSelectorOperator("LongProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator((long)-123456789, typeof(long))
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
                        new MemberSelectorOperator("SimpleEnumProp", new ParameterOperator(parameters, parameterName)),
                        new CollectionConstantOperator(new List<object> { SimpleEnum.First, SimpleEnum.Second }, typeof(SimpleEnum))
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
                        new MemberSelectorOperator("NullableSimpleEnumProp", new ParameterOperator(parameters, parameterName)),
                        new CollectionConstantOperator(new List<object> { SimpleEnum.First, SimpleEnum.Second }, typeof(SimpleEnum?))
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
                        new MemberSelectorOperator("NullableSimpleEnumProp", new ParameterOperator(parameters, parameterName)),
                        new CollectionConstantOperator(new List<object> { SimpleEnum.First, null }, typeof(SimpleEnum?))
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
                            new MemberSelectorOperator("FloatProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(4321.56F)
                        ),
                        new GreaterThanBinaryOperator
                        (
                            new MemberSelectorOperator("FloatProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(1234.56f)
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
                            new MemberSelectorOperator("DecimalProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(4321.56M)
                        ),
                        new GreaterThanBinaryOperator
                        (
                            new MemberSelectorOperator("DecimalProp", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator(1234.56m)
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
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator(literal)
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
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator(c.ToString())
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
                            new MemberSelectorOperator
                            (
                                "EnumerableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            typeof(DerivedProduct)
                        ),
                        new EqualsBinaryOperator
                        (
                             new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "p")),
                             new ConstantOperator("ProductName")
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
                            new MemberSelectorOperator
                            (
                                "QueryableProducts",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            typeof(DerivedProduct)
                        ),
                        new EqualsBinaryOperator
                        (
                             new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, "p")),
                             new ConstantOperator("ProductName")
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
                            new MemberSelectorOperator
                            (
                                "Products",
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                            ),
                            typeof(DerivedProduct)
                        ),
                        new EqualsBinaryOperator
                        (
                             new MemberSelectorOperator("DerivedProductName", new ParameterOperator(parameters, "p")),
                             new ConstantOperator("DerivedProductName")
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
                        new MemberSelectorOperator
                        (
                            "ProductName",
                            new CastOperator
                            (
                                new ParameterOperator(parameters, parameterName),
                                typeof(Product)
                            )
                        ),
                        new ConstantOperator("ProductName")
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
                            new MemberSelectorOperator
                            (
                                "ProductName",
                                new CastOperator
                                (
                                    new ParameterOperator(parameters, parameterName),
                                    typeof(Product)
                                )
                            ),
                            new ConstantOperator("ProductName")
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator
                            (
                                "DerivedProductName",
                                new CastOperator
                                (
                                    new ParameterOperator(parameters, parameterName),
                                    typeof(DerivedProduct)
                                )
                            ),
                            new ConstantOperator("DerivedProductName")
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator
                            (
                                "CategoryID",
                                new MemberSelectorOperator
                                (
                                    "Category",
                                    new CastOperator
                                    (
                                        new ParameterOperator(parameters, parameterName),
                                        typeof(DerivedProduct)
                                    )
                                )
                            ),
                            new ConstantOperator(123)
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator
                            (
                                "CategoryID",
                                new CastOperator
                                (
                                    new MemberSelectorOperator
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
                            new ConstantOperator(123)
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
                            new MemberSelectorOperator
                            (
                                "DerivedProductName",
                                new CastOperator
                                (
                                    new ParameterOperator(parameters, parameterName),
                                    typeof(DerivedProduct)
                                )
                            ),
                            new ConstantOperator("DerivedProductName")
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator
                            (
                                "CategoryID",
                                new MemberSelectorOperator
                                (
                                    "Category",
                                    new CastOperator
                                    (
                                        new ParameterOperator(parameters, parameterName),
                                        typeof(DerivedProduct)
                                    )
                                )
                            ),
                            new ConstantOperator(123)
                        ),
                        parameters
                    },
                    new object[]
                    {
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator
                            (
                                "CategoryID",
                                new CastOperator
                                (
                                    new MemberSelectorOperator
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
                            new ConstantOperator(123)
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
                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object[]
                        {
                            new EqualsBinaryOperator
                            (
                                new ConstantOperator(null),
                                new ConstantOperator(null)
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
                                new ConstantOperator(null),
                                new ConstantOperator(123)
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
                                new ConstantOperator(null),
                                new ConstantOperator(123)
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
                                new ConstantOperator(null),
                                new ConstantOperator(true)
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
                                new ConstantOperator(null),
                                new ConstantOperator(1)
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
                                new ConstantOperator(null),
                                new ConstantOperator(new Guid())
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
                                new ConstantOperator(null),
                                new ConstantOperator("123")
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
                                new ConstantOperator(null),
                                new ConstantOperator(new DateTimeOffset(new DateTime(2001, 1, 1, 12, 0, 0), new TimeSpan(8, 0, 0)))
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
                                new ConstantOperator(null),
                                new ConstantOperator(new TimeSpan(7775999999000))
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
                                    new MemberSelectorOperator("IntProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("LongProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("SingleProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("DoubleProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("DecimalProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("BoolProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("ByteProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("GuidProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                new MemberSelectorOperator("StringProp", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("DateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("TimeSpanProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("SimpleEnumProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("FlagsEnumProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("LongEnumProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("NullableIntProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("NullableLongProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("NullableSingleProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("NullableDoubleProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("NullableDecimalProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("NullableBoolProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("NullableByteProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("NullableGuidProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("NullableDateTimeOffsetProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("NullableTimeSpanProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                    new MemberSelectorOperator("NullableSimpleEnumProp", new ParameterOperator(parameters, parameterName))
                                ),
                                new ConstantOperator("123")
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
                                new ConvertOperator
                                (
                                    new MemberSelectorOperator("IntProp", new ParameterOperator(parameters, parameterName)),
                                    typeof(long)
                                ),
                                new ConstantOperator((long)123)
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
                                new ConvertOperator
                                (
                                    new MemberSelectorOperator("NullableLongProp", new ParameterOperator(parameters, parameterName)),
                                    typeof(double)
                                ),
                                new ConstantOperator(1.23d)
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
                                new ConvertOperator
                                (
                                    new ConstantOperator(2147483647),
                                    typeof(short)
                                ),
                                new ConstantOperator(null)
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
                                    new ConstantOperator(SimpleEnum.Second, typeof(SimpleEnum))
                                ),
                                new ConstantOperator("1")
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
                                    new ConvertOperator
                                    (
                                        new ConvertOperator
                                        (
                                            new MemberSelectorOperator("IntProp", new ParameterOperator(parameters, parameterName)),
                                            typeof(long)
                                        ),
                                        typeof(short)
                                    )
                                ),
                                new ConstantOperator("123")
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
                                new ConstantOperator(null)
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
                                new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
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
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName)),
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
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName)),
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
                                new MemberSelectorOperator("Ranking", new ParameterOperator(parameters, parameterName)),
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
                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new IsOfOperator
                            (
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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
                                new MemberSelectorOperator("ByteArrayProp", new ParameterOperator(parameters, parameterName)),
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
                                new MemberSelectorOperator("IntProp", new ParameterOperator(parameters, parameterName)),
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
                                new MemberSelectorOperator("NullableShortProp", new ParameterOperator(parameters, parameterName)),
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
                                new ConstantOperator(23),
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
                                new ConstantOperator(23),
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
                                new ConstantOperator(23),
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
                                new ConstantOperator(23),
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
                                new ConstantOperator(23),
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
                                new ConstantOperator(23),
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
                                new ConstantOperator(23),
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
                                new ConstantOperator("hello"),
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
                                new ConstantOperator(0),
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
                                new ConstantOperator(0),
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
                                new ConstantOperator("2001-01-01T12:00:00.000+08:00"),
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
                                new ConstantOperator("00000000-0000-0000-0000-000000000000"),
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
                                new ConstantOperator("23"),
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
                                new ConstantOperator("23"),
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
                                new ConstantOperator("23"),
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
                                new ConstantOperator("false"),
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
                                new ConstantOperator("OData"),
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
                                new ConstantOperator("PT12H'"),
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
                                new ConstantOperator(23),
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
                                new ConstantOperator("0"),
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
                                new ConstantOperator("0"),
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
                                new MemberSelectorOperator("SupplierAddress", new ParameterOperator(parameters, parameterName)),
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
                                new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName)),
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
                                new ConstantOperator(null),
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
                                new ConstantOperator(null),
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

        #region
        public static List<object[]> ByteArrayComparisons_Data
        {
            get
            {
                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new EqualsBinaryOperator
                            (
                                new MemberSelectorOperator("ByteArrayProp", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperator(Convert.FromBase64String("I6v/"))
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
                                new MemberSelectorOperator("ByteArrayProp", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperator(Convert.FromBase64String("I6v/"))
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
                                new ConstantOperator(Convert.FromBase64String("I6v/")),
                                new ConstantOperator(Convert.FromBase64String("I6v/"))
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
                                new ConstantOperator(Convert.FromBase64String("I6v/")),
                                new ConstantOperator(Convert.FromBase64String("I6v/"))
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
                                new MemberSelectorOperator("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperator(Convert.FromBase64String("I6v/"))
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
                                new MemberSelectorOperator("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName)),
                                new MemberSelectorOperator("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName))
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
                                new MemberSelectorOperator("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperator(null)
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
                                new MemberSelectorOperator("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperator(null)
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
                                new ConstantOperator(null),
                                new MemberSelectorOperator("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName))
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
                                new ConstantOperator(null),
                                new MemberSelectorOperator("ByteArrayPropWithNullValue", new ParameterOperator(parameters, parameterName))
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
                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new GreaterThanOrEqualsBinaryOperator
                            (
                                new ConstantOperator(Convert.FromBase64String("AP8Q")),
                                new ConstantOperator(Convert.FromBase64String("AP8Q"))
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
                                new ConstantOperator(Convert.FromBase64String("AP8Q")),
                                new ConstantOperator(Convert.FromBase64String("AP8Q"))
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
                                new ConstantOperator(Convert.FromBase64String("AP8Q")),
                                new ConstantOperator(Convert.FromBase64String("AP8Q"))
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
                                new ConstantOperator(Convert.FromBase64String("AP8Q")),
                                new ConstantOperator(Convert.FromBase64String("AP8Q"))
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
                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new EqualsBinaryOperator
                            (
                                new ConvertOperator
                                (
                                    new ConvertToNullableUnderlyingValueOperator
                                    (
                                        new MemberSelectorOperator("NullableUShortProp", new ParameterOperator(parameters, parameterName))
                                    ),
                                    typeof(int?)
                                ),
                                new ConstantOperator(12)
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
                                new ConvertOperator
                                (
                                    new ConvertToNullableUnderlyingValueOperator
                                    (
                                        new MemberSelectorOperator("NullableULongProp", new ParameterOperator(parameters, parameterName))
                                    ),
                                    typeof(long?)
                                ),
                                new ConstantOperator(12L)
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
                                new ConvertOperator
                                (
                                    new ConvertToNullableUnderlyingValueOperator
                                    (
                                        new MemberSelectorOperator("NullableUIntProp", new ParameterOperator(parameters, parameterName))
                                    ),
                                    typeof(int?)
                                ),
                                new ConstantOperator(12)
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
                                        new MemberSelectorOperator("NullableCharProp", new ParameterOperator(parameters, parameterName))
                                    )
                                ),
                                new ConstantOperator("a")
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

        public static List<object[]> InOnNavigation_Data
        {
            get
            {
                return new List<object[]>
                {
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new InOperator
                            (
                                new MemberSelectorOperator
                                (
                                    "ProductID",
                                    new MemberSelectorOperator
                                    (
                                        "Product",
                                        new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                                    )
                                ),
                                new CollectionConstantOperator
                                (
                                    new List<object>{ 1 },
                                    typeof(int)
                                )
                            ),
                            "$it => System.Collections.Generic.List`1[System.Int32].Contains($it.Category.Product.ProductID)",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new InOperator
                            (
                                new MemberSelectorOperator("Category.Product.ProductID", new ParameterOperator(parameters, parameterName)),
                                new CollectionConstantOperator
                                (
                                    new List<object>{ 1 },
                                    typeof(int)
                                )
                            ),
                            "$it => System.Collections.Generic.List`1[System.Int32].Contains($it.Category.Product.ProductID)",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new InOperator
                            (
                                new MemberSelectorOperator
                                (
                                    "GuidProperty",
                                    new MemberSelectorOperator
                                    (
                                        "Product",
                                        new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                                    )
                                ),
                                new CollectionConstantOperator
                                (
                                    new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") },
                                    typeof(Guid)
                                )
                            ),
                            "$it => System.Collections.Generic.List`1[System.Guid].Contains($it.Category.Product.GuidProperty)",
                            parameters
                        }
                    ),
                    GetArguments
                    (
                        parameters => new object []
                        {
                            new InOperator
                            (
                                new MemberSelectorOperator
                                (
                                    "NullableGuidProperty",
                                    new MemberSelectorOperator
                                    (
                                        "Product",
                                        new MemberSelectorOperator("Category", new ParameterOperator(parameters, parameterName))
                                    )
                                ),
                                new CollectionConstantOperator
                                (
                                    new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") },
                                    typeof(Guid?)
                                )
                            ),
                            "$it => System.Collections.Generic.List`1[System.Nullable`1[System.Guid]].Contains($it.Category.Product.NullableGuidProperty)",
                            parameters
                        }
                    ),
                };
            }
        }

        [Theory]
        [MemberData(nameof(InOnNavigation_Data))]
        public void InOnNavigation(IExpressionPart filterBody, string expectedExpression, IDictionary<string, ParameterExpression> parameters)
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

        [Fact]
        public void MultipleConstants_Are_Parameterized()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => (((($it.ProductName == \"1\") OrElse ($it.ProductName == \"2\")) OrElse ($it.ProductName == \"3\")) OrElse ($it.ProductName == \"4\"))");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new OrBinaryOperator
                    (
                        new OrBinaryOperator
                        (
                            new OrBinaryOperator
                            (
                                new EqualsBinaryOperator
                                (
                                    new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                                    new ConstantOperator("1")
                                ),
                                new EqualsBinaryOperator
                                (
                                    new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                                    new ConstantOperator("2")
                                )
                            ),
                            new EqualsBinaryOperator
                            (
                                new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                                new ConstantOperator("3")
                            )
                        ),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                            new ConstantOperator("4")
                        )
                    ),
                    parameters
                );
        }

        [Fact]
        public void Constants_Are_Not_Parameterized_IfDisabled()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => ($it.ProductName == \"1\")");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryOperator
                    (
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new ConstantOperator("1")
                    ),
                    parameters
                );
        }

        [Fact]
        public void CollectionConstants_Are_Parameterized()
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => System.Collections.Generic.List`1[System.String].Contains($it.ProductName)");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new InOperator
                    (
                        new MemberSelectorOperator("ProductName", new ParameterOperator(parameters, parameterName)),
                        new CollectionConstantOperator
                        (
                            new List<object> { "Prod1", "Prod2" },
                            typeof(string)
                        )
                    ),
                    parameters
                );
        }

        [Fact]
        public void CollectionConstants_OfEnums_Are_Not_Parameterized_If_Disabled()
        {
            //act
            var filter = CreateFilter<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => System.Collections.Generic.List`1[LogicBuilder.Expressions.Utils.Tests.Data.SimpleEnum].Contains($it.SimpleEnumProp)");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new InOperator
                    (
                        new MemberSelectorOperator("SimpleEnumProp", new ParameterOperator(parameters, parameterName)),
                        new CollectionConstantOperator
                        (
                            new List<object> { SimpleEnum.First, SimpleEnum.Second },
                            typeof(SimpleEnum)
                        )
                    ),
                    parameters
                );
        }
        #endregion

        //private string PadRightInstance(string str, int number)
        //{
        //    return str.PadRight(number);
        //}

        // Used by Custom Method binder tests - by reflection
        private static string PadRightStatic(string str, int number)
        {
            return str.PadRight(number);
        }

        private static T? ToNullable<T>(object value) where T : struct => 
            value == null ? null : (T?)Convert.ChangeType(value, typeof(T));

        private static IDictionary<string, ParameterExpression> GetParameters()
            => new Dictionary<string, ParameterExpression>();

        static object[] GetArguments(IDictionary<string, ParameterExpression> parameters, Func<IDictionary<string, ParameterExpression>, object[]> getList) 
            => getList(parameters);

        static object[] GetArguments(Func<IDictionary<string, ParameterExpression>, object[]> getList) 
            => GetArguments(GetParameters(), getList);

        private static Expression<Func<T, bool>> GetFilter<T>(IExpressionPart filterBody, IDictionary<string, ParameterExpression> parameters, string parameterName = "$it") 
            => filterBody.GetFilter<T>(parameters, parameterName);

        private static void AssertFilterStringIsCorrect(Expression expression, string expected)
        {
            string resultExpression = ExpressionStringBuilder.ToString(expression);
            Assert.True(expected == resultExpression, string.Format("Expected expression '{0}' but the deserializer produced '{1}'", expected, resultExpression));
        }

        private static bool RunFilter<TModel>(Expression<Func<TModel, bool>> filter, TModel instance)
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
