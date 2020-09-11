using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Tests
{
    public class FilterDescriptorTests
    {
        public FilterDescriptorTests()
        {
            Initialize();
        }

        #region Fields
        private IServiceProvider serviceProvider;
        private static readonly string parameterName = "$it";
        #endregion Fields

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
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(null)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor("Doritos", typeof(string))
                    )
                );
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("Doritos", false)]
        public void NotEqualDescriptor(string productName, bool expected)
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
                    new NotEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor("Doritos", typeof(string))
                    )
                );
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(5.01, true)]
        [InlineData(4.99, false)]
        public void GreaterThanDescriptor(object unitPrice, bool expected)
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
                    new GreaterThanBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(5.00m, typeof(decimal))
                    )
                );
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(5.0, true)]
        [InlineData(4.99, false)]
        public void GreaterThanEqualDescriptor(object unitPrice, bool expected)
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
                    new GreaterThanOrEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(5.00m, typeof(decimal))
                    )
                );
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(4.99, true)]
        [InlineData(5.01, false)]
        public void LessThanDescriptor(object unitPrice, bool expected)
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
                    new LessThanBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(5.00m, typeof(decimal))
                    )
                );
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(5.0, true)]
        [InlineData(5.01, false)]
        public void LessThanOrEqualDescriptor(object unitPrice, bool expected)
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
                    new LessThanOrEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(5.00m, typeof(decimal))
                    )
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
                    new LessThanOrEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(-5.00m, typeof(decimal))
                    )
                );
        }

        public static List<object[]> DateTimeOffsetInequalities_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("DateTimeOffsetProp", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("DateTimeOffsetProp", new ParameterDescriptor(parameterName))
                    ),
                    "$it => ($it.DateTimeOffsetProp == $it.DateTimeOffsetProp)"
                },
                new object[]
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("DateTimeOffsetProp", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("DateTimeOffsetProp", new ParameterDescriptor(parameterName))
                    ),
                    "$it => ($it.DateTimeOffsetProp != $it.DateTimeOffsetProp)"
                },
                new object[]
                {
                    new GreaterThanOrEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("DateTimeOffsetProp", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("DateTimeOffsetProp", new ParameterDescriptor(parameterName))
                    ),
                    "$it => ($it.DateTimeOffsetProp >= $it.DateTimeOffsetProp)"
                },
                new object[]
                {
                    new LessThanOrEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("DateTimeOffsetProp", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("DateTimeOffsetProp", new ParameterDescriptor(parameterName))
                    ),
                    "$it => ($it.DateTimeOffsetProp <= $it.DateTimeOffsetProp)"
                }
            };

        [Theory]
        [MemberData(nameof(DateTimeOffsetInequalities_Data))]
        public void DateTimeOffsetInequalities(IExpressionDescriptor filterBody, string expectedExpression)
        {
            //act
            var filter = CreateFilter<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter, expectedExpression);

            Expression<Func<T, bool>> CreateFilter<T>()
            {
                return GetFilter<T>
                (
                    filterBody
                );
            }
        }

        public static List<object[]> DateInEqualities_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("DateTimeProp", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("DateTimeProp", new ParameterDescriptor(parameterName))
                    ),
                    "$it => ($it.DateTimeProp == $it.DateTimeProp)"
                },
                new object[]
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("DateTimeProp", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("DateTimeProp", new ParameterDescriptor(parameterName))
                    ),
                    "$it => ($it.DateTimeProp != $it.DateTimeProp)"
                },
                new object[]
                {
                    new GreaterThanOrEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("DateTimeProp", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("DateTimeProp", new ParameterDescriptor(parameterName))
                    ),
                    "$it => ($it.DateTimeProp >= $it.DateTimeProp)"
                },
                new object[]
                {
                    new LessThanOrEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("DateTimeProp", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("DateTimeProp", new ParameterDescriptor(parameterName))
                    ),
                    "$it => ($it.DateTimeProp <= $it.DateTimeProp)"
                }
            };

        [Theory]
        [MemberData(nameof(DateInEqualities_Data))]
        public void DateInEqualities(IExpressionDescriptor filterBody, string expectedExpression)
        {
            //act
            var filter = CreateFilter<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter, expectedExpression);

            Expression<Func<T, bool>> CreateFilter<T>()
            {
                return GetFilter<T>
                (
                    filterBody
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
                    new OrBinaryDescriptor
                    (
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(5.00m, typeof(decimal))
                        ),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(0, typeof(int))
                        )
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("Discontinued", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(true, typeof(bool))
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("Discontinued", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("Discontinued", new ParameterDescriptor(parameterName))
                    )
                );
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData(5.0, 0, true)]
        [InlineData(null, 1, false)]
        public void OrDescriptor(object unitPrice, object unitsInStock, bool expected)
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
                    new OrBinaryDescriptor
                    (
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(5.00m, typeof(decimal))
                        ),
                        new EqualsBinaryDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)), typeof(int?)),
                            new ConstantDescriptor(0, typeof(int))
                        )
                    )
                );
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData(5.0, 10, true)]
        [InlineData(null, 1, false)]
        public void AndDescriptor(object unitPrice, object unitsInStock, bool expected)
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
                    new AndBinaryDescriptor
                    (
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(5.00m, typeof(decimal))
                        ),
                        new EqualsBinaryDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)), typeof(decimal?)),
                            new ConstantDescriptor(10.00m, typeof(decimal))
                        )
                    )
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
                    new NotDescriptor
                    (
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(5.00m, typeof(decimal))
                        )
                    )
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
                    new NotDescriptor
                    (
                        new MemberSelectorDescriptor("Discontinued", new ParameterDescriptor(parameterName))
                    )
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
                    new NotDescriptor
                    (
                        new NotDescriptor
                        (
                            new NotDescriptor
                            (
                                new MemberSelectorDescriptor("Discontinued", new ParameterDescriptor(parameterName))
                            )
                        )
                    )
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
                    new LessThanBinaryDescriptor
                    (
                        new SubtractBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(1.00m, typeof(decimal))
                        ),
                        new ConstantDescriptor(5.00m, typeof(decimal))
                    )
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
                    new LessThanBinaryDescriptor
                    (
                        new AddBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(1.00m, typeof(decimal))
                        ),
                        new ConstantDescriptor(5.00m, typeof(decimal))
                    )
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
                    new LessThanBinaryDescriptor
                    (
                        new MultiplyBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(1.00m, typeof(decimal))
                        ),
                        new ConstantDescriptor(5.00m, typeof(decimal))
                    )
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
                    new LessThanBinaryDescriptor
                    (
                        new DivideBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(1.00m, typeof(decimal))
                        ),
                        new ConstantDescriptor(5.00m, typeof(decimal))
                    )
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
                    new LessThanBinaryDescriptor
                    (
                        new ModuloBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(1.00m, typeof(decimal))
                        ),
                        new ConstantDescriptor(5.00m, typeof(decimal))
                    )
                );
        }
        #endregion Arithmetic Operators

        #region NULL handling
        public static List<object[]> NullHandling_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                    ),
                    null,
                    null,
                    true
                },
                new object[]
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                    ),
                    null,
                    null,
                    false
                },
                new object[]
                {
                    new GreaterThanBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                    ),
                    null,
                    null,
                    false
                },
                new object[]
                {
                    new GreaterThanOrEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                    ),
                    null,
                    null,
                    false
                },
                new object[]
                {
                    new LessThanBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                    ),
                    null,
                    null,
                    false
                },
                new object[]
                {
                    new LessThanOrEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                    ),
                    null,
                    null,
                    false
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new AddBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                            new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                        ),
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName))
                    ),
                    null,
                    null,
                    true
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new SubtractBinaryDescriptor

                        (
                            new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                            new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                        ),
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName))
                    ),
                    null,
                    null,
                    true
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MultiplyBinaryDescriptor

                        (
                            new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                            new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                        ),
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName))
                    ),
                    null,
                    null,
                    true
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new DivideBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                            new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                        ),
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName))
                    ),
                    null,
                    null,
                    true
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ModuloBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                            new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                        ),
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName))
                    ),
                    null,
                    null,
                    true
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                    ),
                    1,
                    null,
                    false
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("UnitsOnOrder", new ParameterDescriptor(parameterName))
                    ),
                    1,
                    1,
                    true
                }
            };

        [Theory]
        [MemberData(nameof(NullHandling_Data))]
        public void NullHandling(IExpressionDescriptor filterBody, object unitsInStock, object unitsOnOrder, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitsInStock = ToNullable<short>(unitsInStock), UnitsOnOrder = ToNullable<short>(unitsOnOrder) });

            //assert
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> NullHandling_LiteralNull_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(null)
                    ),
                    null,
                    true
                },
                new object[]
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("UnitsInStock", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(null)
                    ),
                    null,
                    false
                }
            };

        [Theory]
        [MemberData(nameof(NullHandling_LiteralNull_Data))]
        public void NullHandling_LiteralNull(IExpressionDescriptor filterBody, object unitsInStock, bool expected)
        {
            //act
            var filter = CreateFilter<Product>();
            bool result = RunFilter(filter, new Product { UnitsInStock = ToNullable<short>(unitsInStock) });

            //assert
            Assert.Equal(expected, result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }
        #endregion NULL handling

        public static List<object[]> ComparisonsInvolvingCastsAndNullableValues_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new GreaterThanBinaryDescriptor
                    (
                        new IndexOfDescriptor
                        (
                            new ConstantDescriptor("hello"),
                            new MemberSelectorDescriptor("StringProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertDescriptor

                        (
                            new MemberSelectorDescriptor("UIntProp", new ParameterDescriptor(parameterName)),
                            typeof(int?)
                        )
                    )
                },
                new object[]
                {
                    new GreaterThanBinaryDescriptor
                    (
                        new IndexOfDescriptor
                        (
                            new ConstantDescriptor("hello"),
                            new MemberSelectorDescriptor("StringProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertDescriptor
                        (
                            new MemberSelectorDescriptor("ULongProp", new ParameterDescriptor(parameterName)),
                            typeof(int?)
                        )
                    )
                },
                new object[]
                {
                    new GreaterThanBinaryDescriptor
                    (
                        new IndexOfDescriptor
                        (
                            new ConstantDescriptor("hello"),
                            new MemberSelectorDescriptor("StringProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertDescriptor
                        (
                            new MemberSelectorDescriptor("UShortProp", new ParameterDescriptor(parameterName)),
                            typeof(int?)
                        )
                    )
                },
                new object[]
                {
                    new GreaterThanBinaryDescriptor
                    (
                        new IndexOfDescriptor
                        (
                            new ConstantDescriptor("hello"),
                            new MemberSelectorDescriptor("StringProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertDescriptor
                        (
                            new MemberSelectorDescriptor("NullableUShortProp", new ParameterDescriptor(parameterName)),
                            typeof(int?)
                        )
                    )
                },
                new object[]
                {
                    new GreaterThanBinaryDescriptor
                    (
                        new IndexOfDescriptor
                        (
                            new ConstantDescriptor("hello"),
                            new MemberSelectorDescriptor("StringProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertDescriptor
                        (
                            new MemberSelectorDescriptor("NullableUIntProp", new ParameterDescriptor(parameterName)),
                            typeof(int?)
                        )
                    )
                },
                new object[]
                {
                    new GreaterThanBinaryDescriptor
                    (
                        new IndexOfDescriptor
                        (
                            new ConstantDescriptor("hello"),
                            new MemberSelectorDescriptor("StringProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertDescriptor
                        (
                            new MemberSelectorDescriptor("NullableULongProp", new ParameterDescriptor(parameterName)),
                            typeof(int?)
                        )
                    )
                }
            };

        [Theory]
        [MemberData(nameof(ComparisonsInvolvingCastsAndNullableValues_Data))]
        public void ComparisonsInvolvingCastsAndNullableValues(IExpressionDescriptor filterBody)
        {
            //act
            var filter = CreateFilter<DataTypes>();

            //assert
            Assert.Throws<ArgumentNullException>(() => RunFilter(filter, new DataTypes()));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
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
                    new OrBinaryDescriptor
                    (
                        new NotEqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor("Doritos")
                        ),
                        new LessThanBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(5.00m, typeof(decimal))
                        )
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "CategoryName",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("Snacks")
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "CategoryName",
                            new MemberSelectorDescriptor
                            (
                                "Category",
                                new MemberSelectorDescriptor

                                (
                                    "Product",
                                    new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                                )
                            )
                        ),
                        new ConstantDescriptor("Snacks")
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "City",
                            new MemberSelectorDescriptor("SupplierAddress", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("Redmond")
                    )
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
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "EnumerableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("P")),
                            new ConstantDescriptor("Snacks")
                        ),
                        "P"
                    )
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
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "QueryableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("P")),
                            new ConstantDescriptor("Snacks")
                        ),
                        "P"
                    )
                );
        }

        public static List<object[]> AnyInOnNavigation_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "QueryableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new InDescriptor
                        (
                            new MemberSelectorDescriptor("ProductID", new ParameterDescriptor("P")),
                            new CollectionConstantDescriptor
                            (
                                new List<object>{ 1 },
                                typeof(int)
                            )
                        ),
                        "P"
                    ),
                    "$it => $it.Category.QueryableProducts.Any(P => System.Collections.Generic.List`1[System.Int32].Contains(P.ProductID))"
                },
                new object[]
                {
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "EnumerableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new InDescriptor
                        (
                            new MemberSelectorDescriptor("ProductID", new ParameterDescriptor("P")),
                            new CollectionConstantDescriptor
                            (
                                new List<object>{ 1 },
                                typeof(int)
                            )
                        ),
                        "P"
                    ),
                    "$it => $it.Category.EnumerableProducts.Any(P => System.Collections.Generic.List`1[System.Int32].Contains(P.ProductID))"
                },
                new object[]
                {
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "QueryableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new InDescriptor
                        (
                            new MemberSelectorDescriptor("GuidProperty", new ParameterDescriptor("P")),
                            new CollectionConstantDescriptor
                            (
                                new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") },
                                typeof(Guid)
                            )
                        ),
                        "P"
                    ),
                    "$it => $it.Category.QueryableProducts.Any(P => System.Collections.Generic.List`1[System.Guid].Contains(P.GuidProperty))"
                },
                new object[]
                {
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "EnumerableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new InDescriptor
                        (
                            new MemberSelectorDescriptor("GuidProperty", new ParameterDescriptor("P")),
                            new CollectionConstantDescriptor
                            (
                                new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") },
                                typeof(Guid)
                            )
                        ),
                        "P"
                    ),
                    "$it => $it.Category.EnumerableProducts.Any(P => System.Collections.Generic.List`1[System.Guid].Contains(P.GuidProperty))"
                },
                new object[]
                {
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "QueryableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new InDescriptor
                        (
                            new MemberSelectorDescriptor("NullableGuidProperty", new ParameterDescriptor("P")),
                            new CollectionConstantDescriptor
                            (
                                new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") },
                                typeof(Guid?)
                            )
                        ),
                        "P"
                    ),
                    "$it => $it.Category.QueryableProducts.Any(P => System.Collections.Generic.List`1[System.Nullable`1[System.Guid]].Contains(P.NullableGuidProperty))"
                },
                new object[]
                {
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "EnumerableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new InDescriptor
                        (
                            new MemberSelectorDescriptor("NullableGuidProperty", new ParameterDescriptor("P")),
                            new CollectionConstantDescriptor
                            (
                                new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") },
                                typeof(Guid?)
                            )
                        ),
                        "P"
                    ),
                    "$it => $it.Category.EnumerableProducts.Any(P => System.Collections.Generic.List`1[System.Nullable`1[System.Guid]].Contains(P.NullableGuidProperty))"
                },
                new object[]
                {
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "QueryableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new InDescriptor
                        (
                            new MemberSelectorDescriptor("Discontinued", new ParameterDescriptor("P")),
                            new CollectionConstantDescriptor
                            (
                                new List<object>{ false, null },
                                typeof(bool?)
                            )
                        ),
                        "P"
                    ),
                    "$it => $it.Category.QueryableProducts.Any(P => System.Collections.Generic.List`1[System.Nullable`1[System.Boolean]].Contains(P.Discontinued))"
                },
                new object[]
                {
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "EnumerableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new InDescriptor
                        (
                            new MemberSelectorDescriptor("Discontinued", new ParameterDescriptor("P")),
                            new CollectionConstantDescriptor
                            (
                                new List<object>{ false, null },
                                typeof(bool?)
                            )
                        ),
                        "P"
                    ),
                    "$it => $it.Category.EnumerableProducts.Any(P => System.Collections.Generic.List`1[System.Nullable`1[System.Boolean]].Contains(P.Discontinued))"
                }
            };

        [Theory]
        [MemberData(nameof(AnyInOnNavigation_Data))]
        public void AnyInOnNavigation(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> AnyOnNavigation_Contradiction_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "QueryableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(false),
                        "P"
                    ),
                    "$it => $it.Category.QueryableProducts.Any(P => False)"
                },
                new object[]
                {
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "QueryableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new AndBinaryDescriptor
                        (
                            new ConstantDescriptor(false),
                            new EqualsBinaryDescriptor
                            (
                                new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("P")),
                                new ConstantDescriptor("Snacks")
                            )
                        ),
                        "P"
                    ),
                    "$it => $it.Category.QueryableProducts.Any(P => (False AndAlso (P.ProductName == \"Snacks\")))"
                },
                new object[]
                {
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "QueryableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        )
                    ),
                    "$it => $it.Category.QueryableProducts.Any()"
                }
            };

        [Theory]
        [MemberData(nameof(AnyOnNavigation_Contradiction_Data))]
        public void AnyOnNavigation_Contradiction(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
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
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "EnumerableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("P")),
                            new ConstantDescriptor("Snacks")
                        ),
                        "P"
                    )
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
                    new AllDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "EnumerableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("P")),
                            new ConstantDescriptor("Snacks")
                        ),
                        "P"
                    )
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
                    new AndBinaryDescriptor
                    (
                        new AnyDescriptor
                        (
                            new MemberSelectorDescriptor("AlternateIDs", new ParameterDescriptor(parameterName)),
                            new EqualsBinaryDescriptor
                            (
                                new ParameterDescriptor("n"),
                                new ConstantDescriptor(42)
                            ),
                            "n"
                        ),
                        new AnyDescriptor
                        (
                            new MemberSelectorDescriptor("AlternateAddresses", new ParameterDescriptor(parameterName)),
                            new EqualsBinaryDescriptor
                            (
                                new MemberSelectorDescriptor("City", new ParameterDescriptor("n")),
                                new ConstantDescriptor("Redmond")
                            ),
                            "n"
                        )
                    )
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
                    new AndBinaryDescriptor
                    (
                        new AllDescriptor
                        (
                            new MemberSelectorDescriptor("AlternateIDs", new ParameterDescriptor(parameterName)),
                            new EqualsBinaryDescriptor
                            (
                                new ParameterDescriptor("n"),
                                new ConstantDescriptor(42)
                            ),
                            "n"
                        ),
                        new AllDescriptor
                        (
                            new MemberSelectorDescriptor("AlternateAddresses", new ParameterDescriptor(parameterName)),
                            new EqualsBinaryDescriptor
                            (
                                new MemberSelectorDescriptor("City", new ParameterDescriptor("n")),
                                new ConstantDescriptor("Redmond")
                            ),
                            "n"
                        )
                    )
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
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "EnumerableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        )
                    )
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
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "QueryableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        )
                    )
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
                    new AllDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "EnumerableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("P")),
                            new ConstantDescriptor("Snacks")
                        ),
                        "P"
                    )
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
                    new AllDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "QueryableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("P")),
                            new ConstantDescriptor("Snacks")
                        ),
                        "P"
                    )
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
                    new OrBinaryDescriptor
                    (
                        new AnyDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "QueryableProducts",
                                new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                            ),
                            new EqualsBinaryDescriptor
                            (
                                new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("P")),
                                new ConstantDescriptor("Snacks")
                            ),
                            "P"
                        ),
                        new AnyDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "QueryableProducts",
                                new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                            ),
                            new EqualsBinaryDescriptor
                            (
                                new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("P2")),
                                new ConstantDescriptor("Snacks")
                            ),
                            "P2"
                        )
                    )
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
                    new OrBinaryDescriptor
                    (
                        new AllDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "QueryableProducts",
                                new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                            ),
                            new EqualsBinaryDescriptor
                            (
                                new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("P")),
                                new ConstantDescriptor("Snacks")
                            ),
                            "P"
                        ),
                        new AllDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "QueryableProducts",
                                new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                            ),
                            new EqualsBinaryDescriptor
                            (
                                new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("P2")),
                                new ConstantDescriptor("Snacks")
                            ),
                            "P2"
                        )
                    )
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
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor("AlternateIDs", new ParameterDescriptor(parameterName)),
                        new EqualsBinaryDescriptor
                        (
                            new ParameterDescriptor("id"),
                            new ConstantDescriptor(42)
                        ),
                        "id"
                    )
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
                    new AllDescriptor
                    (
                        new MemberSelectorDescriptor("AlternateIDs", new ParameterDescriptor(parameterName)),
                        new EqualsBinaryDescriptor
                        (
                            new ParameterDescriptor("id"),
                            new ConstantDescriptor(42)
                        ),
                        "id"
                    )
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
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor("AlternateAddresses", new ParameterDescriptor(parameterName)),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("City", new ParameterDescriptor("address")),
                            new ConstantDescriptor("Redmond")
                        ),
                        "address"
                    )
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
                    new AllDescriptor
                    (
                        new MemberSelectorDescriptor("AlternateAddresses", new ParameterDescriptor(parameterName)),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("City", new ParameterDescriptor("address")),
                            new ConstantDescriptor("Redmond")
                        ),
                        "address"
                    )
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
                    new AllDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "QueryableProducts",
                            new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                        ),
                        new AnyDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "EnumerableProducts",
                                new MemberSelectorDescriptor("Category", new ParameterDescriptor("P"))
                            ),
                            new EqualsBinaryDescriptor
                            (
                                new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("PP")),
                                new ConstantDescriptor("Snacks")
                            ),
                            "PP"
                        ),
                        "P"
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new SubstringDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(startIndex)
                        ),
                        new ConstantDescriptor(compareString)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new SubstringDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(startIndex)
                        ),
                        new ConstantDescriptor(compareString)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new SubstringDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(startIndex),
                            new ConstantDescriptor(length)
                        ),
                        new ConstantDescriptor(compareString)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new SubstringDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(startIndex),
                            new ConstantDescriptor(length)
                        ),
                        new ConstantDescriptor(compareString)
                    )
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
                    new ContainsDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor("Abc")
                    )
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
                    new ContainsDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor("Abc")
                    )
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
                    new StartsWithDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor("Abc")
                    )
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
                    new StartsWithDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor("Abc")
                    )
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
                    new EndsWithDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor("Abc")
                    )
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
                    new EndsWithDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor("Abc")
                    )
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
                    new GreaterThanBinaryDescriptor
                    (
                        new LengthDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(0)
                    )
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
                    new GreaterThanBinaryDescriptor
                    (
                        new LengthDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(0)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new IndexOfDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor("Abc")
                        ),
                        new ConstantDescriptor(5)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new IndexOfDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor("Abc")
                        ),
                        new ConstantDescriptor(5)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new SubstringDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(3)
                        ),
                        new ConstantDescriptor("uctName")
                    )
                );

            Expression<Func<T, bool>> CreateFilter2<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryDescriptor
                    (
                        new SubstringDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(3),
                            new ConstantDescriptor(4)
                        ),
                        new ConstantDescriptor("uctN")
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new SubstringDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(3)
                        ),
                        new ConstantDescriptor("uctName")
                    )
                );

            Expression<Func<T, bool>> CreateFilter2<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryDescriptor
                    (
                        new SubstringDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(3),
                            new ConstantDescriptor(4)
                        ),
                        new ConstantDescriptor("uctN")
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new ToLowerDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("tasty treats")
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new ToLowerDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("tasty treats")
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new ToUpperDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("TASTY TREATS")
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new ToUpperDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("TASTY TREATS")
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new TrimDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("Tasty Treats")
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new TrimDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("Tasty Treats")
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new ConcatDescriptor
                        (
                            new ConstantDescriptor("Food"),
                            new ConstantDescriptor("Bar")
                        ),
                        new ConstantDescriptor("FoodBar")
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new DayDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(8)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new DayDescriptor
                        (
                            new MemberSelectorDescriptor("NonNullableDiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(8)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new MonthDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(8)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new YearDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(1974)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new HourDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(8)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new MinuteDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(12)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new SecondDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(33)
                    )
                );
        }

        public static List<object[]> DateTimeOffsetFunctions_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new YearDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedOffset", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(100)
                    ),
                    "$it => ($it.DiscontinuedOffset.Year == 100)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MonthDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedOffset", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(100)
                    ),
                    "$it => ($it.DiscontinuedOffset.Month == 100)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new DayDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedOffset", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(100)
                    ),
                    "$it => ($it.DiscontinuedOffset.Day == 100)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new HourDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedOffset", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(100)
                    ),
                    "$it => ($it.DiscontinuedOffset.Hour == 100)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MinuteDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedOffset", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(100)
                    ),
                    "$it => ($it.DiscontinuedOffset.Minute == 100)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new SecondDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedOffset", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(100)
                    ),
                    "$it => ($it.DiscontinuedOffset.Second == 100)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new NowDateTimeDescriptor(),
                        new ConstantDescriptor(new DateTimeOffset(new DateTime(2016, 11, 8), new TimeSpan(0)))
                    ),
                    "$it => (DateTimeOffset.UtcNow == 11/08/2016 00:00:00 +00:00)"
                },
            };

        [Theory]
        [MemberData(nameof(DateTimeOffsetFunctions_Data))]
        public void DateTimeOffsetFunctions(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> DateTimeFunctions_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new YearDescriptor
                        (
                            new MemberSelectorDescriptor("Birthday", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(100)
                    ),
                    "$it => ({0}.Year == 100)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MonthDescriptor
                        (
                            new MemberSelectorDescriptor("Birthday", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(100)
                    ),
                    "$it => ({0}.Month == 100)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new DayDescriptor
                        (
                            new MemberSelectorDescriptor("Birthday", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(100)
                    ),
                    "$it => ({0}.Day == 100)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new HourDescriptor
                        (
                            new MemberSelectorDescriptor("Birthday", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(100)
                    ),
                    "$it => ({0}.Hour == 100)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MinuteDescriptor
                        (
                            new MemberSelectorDescriptor("Birthday", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(100)
                    ),
                    "$it => ({0}.Minute == 100)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new SecondDescriptor
                        (
                            new MemberSelectorDescriptor("Birthday", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(100)
                    ),
                    "$it => ({0}.Second == 100)"
                },
            };

        [Theory]
        [MemberData(nameof(DateTimeFunctions_Data))]
        public void DateTimeFunctions(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, String.Format(expression, "$it.Birthday"));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> DateFunctions_Nullable_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new YearDescriptor
                        (
                            new MemberSelectorDescriptor("NullableDateProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(2015)
                    ),
                    "$it => ($it.NullableDateProperty.Value.Year == 2015)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MonthDescriptor
                        (
                            new MemberSelectorDescriptor("NullableDateProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(12)
                    ),
                    "$it => ($it.NullableDateProperty.Value.Month == 12)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new DayDescriptor
                        (
                            new MemberSelectorDescriptor("NullableDateProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(23)
                    ),
                    "$it => ($it.NullableDateProperty.Value.Day == 23)"
                },
            };

        [Theory]
        [MemberData(nameof(DateFunctions_Nullable_Data))]
        public void DateFunctions_Nullable(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> DateFunctions_NonNullable_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new YearDescriptor
                        (
                            new MemberSelectorDescriptor("DateProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(2015)
                    ),
                    "$it => ($it.DateProperty.Year == 2015)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MonthDescriptor
                        (
                            new MemberSelectorDescriptor("DateProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(12)
                    ),
                    "$it => ($it.DateProperty.Month == 12)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new DayDescriptor
                        (
                            new MemberSelectorDescriptor("DateProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(23)
                    ),
                    "$it => ($it.DateProperty.Day == 23)"
                },
            };

        [Theory]
        [MemberData(nameof(DateFunctions_NonNullable_Data))]
        public void DateFunctions_NonNullable(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> TimeOfDayFunctions_Nullable_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new HourDescriptor
                        (
                            new MemberSelectorDescriptor("NullableTimeOfDayProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(10)
                    ),
                    "$it => ($it.NullableTimeOfDayProperty.Value.Hours == 10)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MinuteDescriptor
                        (
                            new MemberSelectorDescriptor("NullableTimeOfDayProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(20)
                    ),
                    "$it => ($it.NullableTimeOfDayProperty.Value.Minutes == 20)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new SecondDescriptor
                        (
                            new MemberSelectorDescriptor("NullableTimeOfDayProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(30)
                    ),
                    "$it => ($it.NullableTimeOfDayProperty.Value.Seconds == 30)"
                },
            };

        [Theory]
        [MemberData(nameof(TimeOfDayFunctions_Nullable_Data))]
        public void TimeOfDayFunctions_Nullable(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> TimeOfDayFunctions_NonNullable_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new HourDescriptor
                        (
                            new MemberSelectorDescriptor("TimeOfDayProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(10)
                    ),
                    "$it => ($it.TimeOfDayProperty.Hours == 10)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MinuteDescriptor
                        (
                            new MemberSelectorDescriptor("TimeOfDayProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(20)
                    ),
                    "$it => ($it.TimeOfDayProperty.Minutes == 20)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new SecondDescriptor
                        (
                            new MemberSelectorDescriptor("TimeOfDayProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(30)
                    ),
                    "$it => ($it.TimeOfDayProperty.Seconds == 30)"
                },
            };

        [Theory]
        [MemberData(nameof(TimeOfDayFunctions_NonNullable_Data))]
        public void TimeOfDayFunctions_NonNullable(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> FractionalsecondsFunction_Nullable_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new FractionalSecondsDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(0.2m)
                    ),
                    "$it => ((Convert($it.DiscontinuedDate.Value.Millisecond) / 1000) == 0.2)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new FractionalSecondsDescriptor
                        (
                            new MemberSelectorDescriptor("NullableTimeOfDayProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(0.2m)
                    ),
                    "$it => ((Convert($it.NullableTimeOfDayProperty.Value.Milliseconds) / 1000) == 0.2)"
                },
            };

        [Theory]
        [MemberData(nameof(FractionalsecondsFunction_Nullable_Data))]
        public void FractionalsecondsFunction_Nullable(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> FractionalsecondsFunction_NonNullable_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new FractionalSecondsDescriptor
                        (
                            new MemberSelectorDescriptor("NonNullableDiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(0.2m)
                    ),
                    "$it => ((Convert($it.NonNullableDiscontinuedDate.Millisecond) / 1000) == 0.2)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new FractionalSecondsDescriptor
                        (
                            new MemberSelectorDescriptor("TimeOfDayProperty", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(0.2m)
                    ),
                    "$it => ((Convert($it.TimeOfDayProperty.Milliseconds) / 1000) == 0.2)"
                },
            };

        [Theory]
        [MemberData(nameof(FractionalsecondsFunction_NonNullable_Data))]
        public void FractionalsecondsFunction_NonNullable(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> DateFunction_Nullable_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToNumericDateDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertToNumericDateDescriptor
                        (
                            new ConstantDescriptor(new Date(2015, 2, 26))
                        )
                    ),
                    "$it => (((($it.DiscontinuedDate.Value.Year * 10000) + ($it.DiscontinuedDate.Value.Month * 100)) + $it.DiscontinuedDate.Value.Day) == (((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day))"
                },
                new object[]
                {
                    new LessThanBinaryDescriptor
                    (
                        new ConvertToNumericDateDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertToNumericDateDescriptor
                        (
                            new ConstantDescriptor(new Date(2016, 2, 26))
                        )
                    ),
                    "$it => (((($it.DiscontinuedDate.Value.Year * 10000) + ($it.DiscontinuedDate.Value.Month * 100)) + $it.DiscontinuedDate.Value.Day) < (((2016-02-26.Year * 10000) + (2016-02-26.Month * 100)) + 2016-02-26.Day))"
                },
                new object[]
                {
                    new GreaterThanOrEqualsBinaryDescriptor
                    (
                        new ConvertToNumericDateDescriptor
                        (
                            new ConstantDescriptor(new Date(2015, 2, 26))
                        ),
                        new ConvertToNumericDateDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        )
                    ),
                    "$it => ((((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day) >= ((($it.DiscontinuedDate.Value.Year * 10000) + ($it.DiscontinuedDate.Value.Month * 100)) + $it.DiscontinuedDate.Value.Day))"
                },
                new object[]
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                    ),
                    "$it => (null != $it.DiscontinuedDate)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(null)
                    ),
                    "$it => ($it.DiscontinuedDate == null)"
                },
            };

        [Theory]
        [MemberData(nameof(DateFunction_Nullable_Data))]
        public void DateFunction_Nullable(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> DateFunction_NonNullable_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToNumericDateDescriptor
                        (
                            new MemberSelectorDescriptor("NonNullableDiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertToNumericDateDescriptor
                        (
                            new ConstantDescriptor(new Date(2015, 2, 26))
                        )
                    ),
                    "$it => (((($it.NonNullableDiscontinuedDate.Year * 10000) + ($it.NonNullableDiscontinuedDate.Month * 100)) + $it.NonNullableDiscontinuedDate.Day) == (((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day))"
                },
                new object[]
                {
                    new LessThanBinaryDescriptor
                    (
                        new ConvertToNumericDateDescriptor
                        (
                            new MemberSelectorDescriptor("NonNullableDiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertToNumericDateDescriptor
                        (
                            new ConstantDescriptor(new Date(2016, 2, 26))
                        )
                    ),
                    "$it => (((($it.NonNullableDiscontinuedDate.Year * 10000) + ($it.NonNullableDiscontinuedDate.Month * 100)) + $it.NonNullableDiscontinuedDate.Day) < (((2016-02-26.Year * 10000) + (2016-02-26.Month * 100)) + 2016-02-26.Day))"
                },
                new object[]
                {
                    new GreaterThanOrEqualsBinaryDescriptor
                    (
                        new ConvertToNumericDateDescriptor
                        (
                            new ConstantDescriptor(new Date(2015, 2, 26))
                        ),
                        new ConvertToNumericDateDescriptor
                        (
                            new MemberSelectorDescriptor("NonNullableDiscontinuedDate", new ParameterDescriptor(parameterName))
                        )
                    ),
                    "$it => ((((2015-02-26.Year * 10000) + (2015-02-26.Month * 100)) + 2015-02-26.Day) >= ((($it.NonNullableDiscontinuedDate.Year * 10000) + ($it.NonNullableDiscontinuedDate.Month * 100)) + $it.NonNullableDiscontinuedDate.Day))"
                }
            };

        [Theory]
        [MemberData(nameof(DateFunction_NonNullable_Data))]
        public void DateFunction_NonNullable(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> TimeFunction_Nullable_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToNumericTimeDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertToNumericTimeDescriptor
                        (
                            new ConstantDescriptor(new TimeOfDay(1, 2, 3, 4))
                        )
                    ),
                    "$it => (((Convert($it.DiscontinuedDate.Value.Hour) * 36000000000) + ((Convert($it.DiscontinuedDate.Value.Minute) * 600000000) + ((Convert($it.DiscontinuedDate.Value.Second) * 10000000) + Convert($it.DiscontinuedDate.Value.Millisecond)))) == ((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))))"
                },
                new object[]
                {
                    new GreaterThanOrEqualsBinaryDescriptor
                    (
                        new ConvertToNumericTimeDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertToNumericTimeDescriptor
                        (
                            new ConstantDescriptor(new TimeOfDay(1, 2, 3, 4))
                        )
                    ),
                    "$it => (((Convert($it.DiscontinuedDate.Value.Hour) * 36000000000) + ((Convert($it.DiscontinuedDate.Value.Minute) * 600000000) + ((Convert($it.DiscontinuedDate.Value.Second) * 10000000) + Convert($it.DiscontinuedDate.Value.Millisecond)))) >= ((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))))"
                },
                new object[]
                {
                    new LessThanOrEqualsBinaryDescriptor
                    (
                        new ConvertToNumericTimeDescriptor
                        (
                            new ConstantDescriptor(new TimeOfDay(1, 2, 3, 4))
                        ),
                        new ConvertToNumericTimeDescriptor
                        (
                            new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                        )
                    ),
                    "$it => (((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))) <= ((Convert($it.DiscontinuedDate.Value.Hour) * 36000000000) + ((Convert($it.DiscontinuedDate.Value.Minute) * 600000000) + ((Convert($it.DiscontinuedDate.Value.Second) * 10000000) + Convert($it.DiscontinuedDate.Value.Millisecond)))))"
                },
                new object[]
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName))
                    ),
                    "$it => (null != $it.DiscontinuedDate)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("DiscontinuedDate", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(null)
                    ),
                    "$it => ($it.DiscontinuedDate == null)"
                }
            };

        [Theory]
        [MemberData(nameof(TimeFunction_Nullable_Data))]
        public void TimeFunction_Nullable(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> TimeFunction_NonNullable_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToNumericTimeDescriptor
                        (
                            new MemberSelectorDescriptor("NonNullableDiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertToNumericTimeDescriptor
                        (
                            new ConstantDescriptor(new TimeOfDay(1, 2, 3, 4))
                        )
                    ),
                    "$it => (((Convert($it.NonNullableDiscontinuedDate.Hour) * 36000000000) + ((Convert($it.NonNullableDiscontinuedDate.Minute) * 600000000) + ((Convert($it.NonNullableDiscontinuedDate.Second) * 10000000) + Convert($it.NonNullableDiscontinuedDate.Millisecond)))) == ((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))))"
                },
                new object[]
                {
                    new GreaterThanOrEqualsBinaryDescriptor
                    (
                        new ConvertToNumericTimeDescriptor
                        (
                            new MemberSelectorDescriptor("NonNullableDiscontinuedDate", new ParameterDescriptor(parameterName))
                        ),
                        new ConvertToNumericTimeDescriptor
                        (
                            new ConstantDescriptor(new TimeOfDay(1, 2, 3, 4))
                        )
                    ),
                    "$it => (((Convert($it.NonNullableDiscontinuedDate.Hour) * 36000000000) + ((Convert($it.NonNullableDiscontinuedDate.Minute) * 600000000) + ((Convert($it.NonNullableDiscontinuedDate.Second) * 10000000) + Convert($it.NonNullableDiscontinuedDate.Millisecond)))) >= ((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))))"
                },
                new object[]
                {
                    new LessThanOrEqualsBinaryDescriptor
                    (
                        new ConvertToNumericTimeDescriptor
                        (
                            new ConstantDescriptor(new TimeOfDay(1, 2, 3, 4))
                        ),
                        new ConvertToNumericTimeDescriptor
                        (
                            new MemberSelectorDescriptor("NonNullableDiscontinuedDate", new ParameterDescriptor(parameterName))
                        )
                    ),
                    "$it => (((Convert(01:02:03.0040000.Hours) * 36000000000) + ((Convert(01:02:03.0040000.Minutes) * 600000000) + ((Convert(01:02:03.0040000.Seconds) * 10000000) + Convert(01:02:03.0040000.Milliseconds)))) <= ((Convert($it.NonNullableDiscontinuedDate.Hour) * 36000000000) + ((Convert($it.NonNullableDiscontinuedDate.Minute) * 600000000) + ((Convert($it.NonNullableDiscontinuedDate.Second) * 10000000) + Convert($it.NonNullableDiscontinuedDate.Millisecond)))))"
                }
            };

        [Theory]
        [MemberData(nameof(TimeFunction_NonNullable_Data))]
        public void TimeFunction_NonNullable(IExpressionDescriptor filterBody, string expression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
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
                    new EqualsBinaryDescriptor
                    (
                        new FloorDescriptor
                        (
                            new FloorDescriptor
                            (
                                new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName))
                            )
                        ),
                        new ConstantDescriptor(123m)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new FloorDescriptor
                        (
                            new FloorDescriptor
                            (
                                new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName))
                            )
                        ),
                        new ConstantDescriptor(123m)
                    )
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
                    new GreaterThanBinaryDescriptor
                    (
                        new RoundDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(5.00m)
                    )
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
                    new GreaterThanBinaryDescriptor
                    (
                        new RoundDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(5.00m)
                    )
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
                    new GreaterThanBinaryDescriptor
                    (
                        new RoundDescriptor
                        (
                            new MemberSelectorDescriptor("Weight", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(5d)
                    )
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
                    new GreaterThanBinaryDescriptor
                    (
                        new RoundDescriptor
                        (
                            new MemberSelectorDescriptor("Weight", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(5d)
                    )
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
                    new GreaterThanBinaryDescriptor
                    (
                        new RoundDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("Width", new ParameterDescriptor(parameterName)), typeof(double?))
                        ),
                        new ConstantDescriptor(5d)
                    )
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
                    new GreaterThanBinaryDescriptor
                    (
                        new RoundDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("Width", new ParameterDescriptor(parameterName)), typeof(double?))
                        ),
                        new ConstantDescriptor(5d)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new FloorDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(5m)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new FloorDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(5m)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new FloorDescriptor
                        (
                            new MemberSelectorDescriptor("Weight", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(5d)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new FloorDescriptor
                        (
                            new MemberSelectorDescriptor("Weight", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(5d)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new FloorDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("Width", new ParameterDescriptor(parameterName)), typeof(double?))
                        ),
                        new ConstantDescriptor(5d)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new FloorDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("Width", new ParameterDescriptor(parameterName)), typeof(double?))
                        ),
                        new ConstantDescriptor(5d)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new CeilingDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(5m)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new CeilingDescriptor
                        (
                            new MemberSelectorDescriptor("UnitPrice", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(5m)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new CeilingDescriptor
                        (
                            new MemberSelectorDescriptor("Weight", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(5d)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new CeilingDescriptor
                        (
                            new MemberSelectorDescriptor("Weight", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor(5d)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new CeilingDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("Width", new ParameterDescriptor(parameterName)), typeof(double?))
                        ),
                        new ConstantDescriptor(5d)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new CeilingDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("Width", new ParameterDescriptor(parameterName)), typeof(double?))
                        ),
                        new ConstantDescriptor(5d)
                    )
                );
        }

        public static List<object[]> MathFunctions_VariousTypes_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new FloorDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("FloatProp", new ParameterDescriptor(parameterName)), typeof(double))
                        ),
                        new FloorDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("FloatProp", new ParameterDescriptor(parameterName)), typeof(double))
                        )
                    )
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new RoundDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("FloatProp", new ParameterDescriptor(parameterName)), typeof(double))
                        ),
                        new RoundDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("FloatProp", new ParameterDescriptor(parameterName)), typeof(double))
                        )
                    )
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new CeilingDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("FloatProp", new ParameterDescriptor(parameterName)), typeof(double))
                        ),
                        new CeilingDescriptor
                        (
                            new ConvertDescriptor(new MemberSelectorDescriptor("FloatProp", new ParameterDescriptor(parameterName)), typeof(double))
                        )
                    )
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new FloorDescriptor
                        (
                            new MemberSelectorDescriptor("DoubleProp", new ParameterDescriptor(parameterName))
                        ),
                        new FloorDescriptor
                        (
                            new MemberSelectorDescriptor("DoubleProp", new ParameterDescriptor(parameterName))
                        )
                    )
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new RoundDescriptor
                        (
                            new MemberSelectorDescriptor("DoubleProp", new ParameterDescriptor(parameterName))
                        ),
                        new RoundDescriptor
                        (
                            new MemberSelectorDescriptor("DoubleProp", new ParameterDescriptor(parameterName))
                        )
                    )
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new CeilingDescriptor
                        (
                            new MemberSelectorDescriptor("DoubleProp", new ParameterDescriptor(parameterName))
                        ),
                        new CeilingDescriptor
                        (
                            new MemberSelectorDescriptor("DoubleProp", new ParameterDescriptor(parameterName))
                        )
                    )
                },
                    new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new FloorDescriptor
                        (
                            new MemberSelectorDescriptor("DecimalProp", new ParameterDescriptor(parameterName))
                        ),
                        new FloorDescriptor
                        (
                            new MemberSelectorDescriptor("DecimalProp", new ParameterDescriptor(parameterName))
                        )
                    )
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new RoundDescriptor
                        (
                            new MemberSelectorDescriptor("DecimalProp", new ParameterDescriptor(parameterName))
                        ),
                        new RoundDescriptor
                        (
                            new MemberSelectorDescriptor("DecimalProp", new ParameterDescriptor(parameterName))
                        )
                    )
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new CeilingDescriptor
                        (
                            new MemberSelectorDescriptor("DecimalProp", new ParameterDescriptor(parameterName))
                        ),
                        new CeilingDescriptor
                        (
                            new MemberSelectorDescriptor("DecimalProp", new ParameterDescriptor(parameterName))
                        )
                    )
                },
            };

        [Theory]
        [MemberData(nameof(MathFunctions_VariousTypes_Data))]
        public void MathFunctions_VariousTypes(IExpressionDescriptor filterBody)
        {
            //act
            var filter = CreateFilter<DataTypes>();
            bool result = RunFilter(filter, new DataTypes { });

            //assert
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
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
                    new EqualsBinaryDescriptor
                    (
                        new CustomMethodDescriptor
                        (
                            typeof(string).GetMethod("PadRight", new Type[] { typeof(int) }),
                            new IExpressionDescriptor[]
                            {
                                new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                                new ConstantDescriptor(totalWidth)
                            }
                        ),
                        new ConstantDescriptor(expectedProductName)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new CustomMethodDescriptor
                        (
                            typeof(StringExtender).GetMethod("PadRightExStatic", BindingFlags.Public | BindingFlags.Static),
                            new IExpressionDescriptor[]
                            {
                                new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                                new ConstantDescriptor(totalWidth)
                            }
                        ),
                        new ConstantDescriptor(expectedProductName)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new CustomMethodDescriptor
                        (
                            typeof(FilterDescriptorTests).GetMethod("PadRightStatic", BindingFlags.NonPublic | BindingFlags.Static),
                            new IExpressionDescriptor[]
                            {
                                new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                                new ConstantDescriptor(totalWidth)
                            }
                        ),
                        new ConstantDescriptor(expectedProductName)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("GuidProp", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(new Guid("0EFDAECF-A9F0-42F3-A384-1295917AF95E"))
                    )
                );

            Expression<Func<T, bool>> CreateFilter2<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("GuidProp", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(new Guid("0efdaecf-a9f0-42f3-a384-1295917af95e"))
                    )
                );
        }

        public static List<object[]> DateTimeExpression_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("DateTimeProp", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(new DateTimeOffset(new DateTime(2000, 12, 12, 12, 0, 0), TimeSpan.Zero))
                    ),
                    "$it => ($it.DateTimeProp == {0})"
                },
                new object[]
                {
                    new LessThanBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("DateTimeProp", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(new DateTimeOffset(new DateTime(2000, 12, 12, 12, 0, 0), TimeSpan.Zero))
                    ),
                    "$it => ($it.DateTimeProp < {0})"
                }
            };

        [Theory]
        [MemberData(nameof(DateTimeExpression_Data))]
        public void DateTimeExpression(IExpressionDescriptor filterBody, string expectedExpression)
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
                    filterBody
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
                    new AndBinaryDescriptor
                    (
                        new LessThanBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("LongProp", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor((long)987654321, typeof(long))
                        ),
                        new GreaterThanBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("LongProp", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor((long)123456789, typeof(long))
                        )
                    )
                );

            Expression<Func<T, bool>> CreateFilter2<T>()
                => GetFilter<T>
                (
                    new AndBinaryDescriptor
                    (
                        new LessThanBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("LongProp", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor((long)-987654321, typeof(long))
                        ),
                        new GreaterThanBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("LongProp", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor((long)-123456789, typeof(long))
                        )
                    )
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
            AssertFilterStringIsCorrect(filter, "$it => System.Collections.Generic.List`1[LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data.SimpleEnum].Contains($it.SimpleEnumProp)");
            Assert.Equal(new[] { SimpleEnum.First, SimpleEnum.Second }, values);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new InDescriptor
                    (
                        new MemberSelectorDescriptor("SimpleEnumProp", new ParameterDescriptor(parameterName)),
                        new CollectionConstantDescriptor(new List<object> { SimpleEnum.First, SimpleEnum.Second }, typeof(SimpleEnum))
                    )
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
            AssertFilterStringIsCorrect(filter, "$it => System.Collections.Generic.List`1[System.Nullable`1[LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data.SimpleEnum]].Contains($it.NullableSimpleEnumProp)");
            Assert.Equal(new SimpleEnum?[] { SimpleEnum.First, SimpleEnum.Second }, values);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new InDescriptor
                    (
                        new MemberSelectorDescriptor("NullableSimpleEnumProp", new ParameterDescriptor(parameterName)),
                        new CollectionConstantDescriptor(new List<object> { SimpleEnum.First, SimpleEnum.Second }, typeof(SimpleEnum?))
                    )
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
            AssertFilterStringIsCorrect(filter, "$it => System.Collections.Generic.List`1[System.Nullable`1[LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data.SimpleEnum]].Contains($it.NullableSimpleEnumProp)");
            Assert.Equal(new SimpleEnum?[] { SimpleEnum.First, null }, values);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new InDescriptor
                    (
                        new MemberSelectorDescriptor("NullableSimpleEnumProp", new ParameterDescriptor(parameterName)),
                        new CollectionConstantDescriptor(new List<object> { SimpleEnum.First, null }, typeof(SimpleEnum?))
                    )
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
                    new AndBinaryDescriptor
                    (
                        new LessThanBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("FloatProp", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(4321.56F)
                        ),
                        new GreaterThanBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("FloatProp", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(1234.56f)
                        )
                    )
                );

            Expression<Func<T, bool>> CreateFilter2<T>()
                => GetFilter<T>
                (
                    new AndBinaryDescriptor
                    (
                        new LessThanBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("DecimalProp", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(4321.56M)
                        ),
                        new GreaterThanBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("DecimalProp", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor(1234.56m)
                        )
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(literal)
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(c.ToString())
                    )
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
                    new AnyDescriptor
                    (
                        new CollectionCastDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "EnumerableProducts",
                                new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                            ),
                            typeof(DerivedProduct)
                        ),
                        new EqualsBinaryDescriptor
                        (
                             new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("p")),
                             new ConstantDescriptor("ProductName")
                        ),
                        "p"
                    )
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
                    new AnyDescriptor
                    (
                        new CollectionCastDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "QueryableProducts",
                                new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                            ),
                            typeof(DerivedProduct)
                        ),
                        new EqualsBinaryDescriptor
                        (
                             new MemberSelectorDescriptor("ProductName", new ParameterDescriptor("p")),
                             new ConstantDescriptor("ProductName")
                        ),
                        "p"
                    )
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
                    new AnyDescriptor
                    (
                        new CollectionCastDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "Products",
                                new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                            ),
                            typeof(DerivedProduct)
                        ),
                        new EqualsBinaryDescriptor
                        (
                             new MemberSelectorDescriptor("DerivedProductName", new ParameterDescriptor("p")),
                             new ConstantDescriptor("DerivedProductName")
                        ),
                        "p"
                    )
                );
        }

        [Fact]
        public void NSCast_OnSingleEntity_GeneratesExpression_WithAsDescriptor()
        {
            //act
            var filter = CreateFilter<DerivedProduct>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => (($it As Product).ProductName == \"ProductName\")");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "ProductName",
                            new CastDescriptor
                            (
                                new ParameterDescriptor(parameterName),
                                typeof(Product)
                            )
                        ),
                        new ConstantDescriptor("ProductName")
                    )
                );
        }

        public static List<object[]> Inheritance_WithDerivedInstance_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "ProductName",
                            new CastDescriptor
                            (
                                new ParameterDescriptor(parameterName),
                                typeof(Product)
                            )
                        ),
                        new ConstantDescriptor("ProductName")
                    )
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "DerivedProductName",
                            new CastDescriptor
                            (
                                new ParameterDescriptor(parameterName),
                                typeof(DerivedProduct)
                            )
                        ),
                        new ConstantDescriptor("DerivedProductName")
                    )
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "CategoryID",
                            new MemberSelectorDescriptor
                            (
                                "Category",
                                new CastDescriptor
                                (
                                    new ParameterDescriptor(parameterName),
                                    typeof(DerivedProduct)
                                )
                            )
                        ),
                        new ConstantDescriptor(123)
                    )
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "CategoryID",
                            new CastDescriptor
                            (
                                new MemberSelectorDescriptor
                                (
                                    "Category",
                                    new CastDescriptor
                                    (
                                        new ParameterDescriptor(parameterName),
                                        typeof(DerivedProduct)
                                    )
                                ),
                                typeof(DerivedCategory)
                            )
                        ),
                        new ConstantDescriptor(123)
                    )
                },
            };

        [Theory]
        [MemberData(nameof(Inheritance_WithDerivedInstance_Data))]
        public void Inheritance_WithDerivedInstance(IExpressionDescriptor filterBody)
        {
            //act
            var filter = CreateFilter<DerivedProduct>();
            bool result = RunFilter(filter, new DerivedProduct { Category = new DerivedCategory { CategoryID = 123 }, ProductName = "ProductName", DerivedProductName = "DerivedProductName" });

            //assert
            Assert.True(result);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> Inheritance_WithBaseInstance_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "DerivedProductName",
                            new CastDescriptor
                            (
                                new ParameterDescriptor(parameterName),
                                typeof(DerivedProduct)
                            )
                        ),
                        new ConstantDescriptor("DerivedProductName")
                    )
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "CategoryID",
                            new MemberSelectorDescriptor
                            (
                                "Category",
                                new CastDescriptor
                                (
                                    new ParameterDescriptor(parameterName),
                                    typeof(DerivedProduct)
                                )
                            )
                        ),
                        new ConstantDescriptor(123)
                    )
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor
                        (
                            "CategoryID",
                            new CastDescriptor
                            (
                                new MemberSelectorDescriptor
                                (
                                    "Category",
                                    new CastDescriptor
                                    (
                                        new ParameterDescriptor(parameterName),
                                        typeof(DerivedProduct)
                                    )
                                ),
                                typeof(DerivedCategory)
                            )
                        ),
                        new ConstantDescriptor(123)
                    )
                },
            };

        [Theory]
        [MemberData(nameof(Inheritance_WithBaseInstance_Data))]
        public void Inheritance_WithBaseInstance(IExpressionDescriptor filterBody)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            Assert.Throws<NullReferenceException>(() => RunFilter(filter, new Product()));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> CastMethod_Succeeds_Data 
            => new List<object[]>
            {
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new ConstantDescriptor(null)
                    ),
                    "$it => (null == null)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new ConstantDescriptor(123)
                    ),
                    "$it => (null == Convert(123))"
                },
                new object[]
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new ConstantDescriptor(123)
                    ),
                    "$it => (null != Convert(123))"
                },
                new object[]
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new ConstantDescriptor(true)
                    ),
                    "$it => (null != Convert(True))"
                },
                new object[]
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new ConstantDescriptor(1)
                    ),
                    "$it => (null != Convert(1))"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new ConstantDescriptor(new Guid())
                    ),
                    "$it => (null == Convert(00000000-0000-0000-0000-000000000000))"
                },
                new object[]
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (null != \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new ConstantDescriptor(new DateTimeOffset(new DateTime(2001, 1, 1, 12, 0, 0), new TimeSpan(8, 0, 0)))
                    ),
                    "$it => (null == Convert(01/01/2001 12:00:00 +08:00))"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new ConstantDescriptor(new TimeSpan(7775999999000))
                    ),
                    "$it => (null == Convert(8.23:59:59.9999000))"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("IntProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => ($it.IntProp.ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("LongProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => ($it.LongProp.ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("SingleProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => ($it.SingleProp.ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("DoubleProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => ($it.DoubleProp.ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("DecimalProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => ($it.DecimalProp.ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("BoolProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => ($it.BoolProp.ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("ByteProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => ($it.ByteProp.ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("GuidProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => ($it.GuidProp.ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("StringProp", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor("123")
                    ),
                    "$it => ($it.StringProp == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("DateTimeOffsetProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => ($it.DateTimeOffsetProp.ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("TimeSpanProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => ($it.TimeSpanProp.ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("SimpleEnumProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (Convert($it.SimpleEnumProp).ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("FlagsEnumProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (Convert($it.FlagsEnumProp).ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("LongEnumProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (Convert($it.LongEnumProp).ToString() == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("NullableIntProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (IIF($it.NullableIntProp.HasValue, $it.NullableIntProp.Value.ToString(), null) == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("NullableLongProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (IIF($it.NullableLongProp.HasValue, $it.NullableLongProp.Value.ToString(), null) == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("NullableSingleProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (IIF($it.NullableSingleProp.HasValue, $it.NullableSingleProp.Value.ToString(), null) == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("NullableDoubleProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (IIF($it.NullableDoubleProp.HasValue, $it.NullableDoubleProp.Value.ToString(), null) == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("NullableDecimalProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (IIF($it.NullableDecimalProp.HasValue, $it.NullableDecimalProp.Value.ToString(), null) == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("NullableBoolProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (IIF($it.NullableBoolProp.HasValue, $it.NullableBoolProp.Value.ToString(), null) == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("NullableByteProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (IIF($it.NullableByteProp.HasValue, $it.NullableByteProp.Value.ToString(), null) == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("NullableGuidProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (IIF($it.NullableGuidProp.HasValue, $it.NullableGuidProp.Value.ToString(), null) == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("NullableDateTimeOffsetProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (IIF($it.NullableDateTimeOffsetProp.HasValue, $it.NullableDateTimeOffsetProp.Value.ToString(), null) == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("NullableTimeSpanProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (IIF($it.NullableTimeSpanProp.HasValue, $it.NullableTimeSpanProp.Value.ToString(), null) == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new MemberSelectorDescriptor("NullableSimpleEnumProp", new ParameterDescriptor(parameterName))
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (IIF($it.NullableSimpleEnumProp.HasValue, Convert($it.NullableSimpleEnumProp.Value).ToString(), null) == \"123\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertDescriptor
                        (
                            new MemberSelectorDescriptor("IntProp", new ParameterDescriptor(parameterName)),
                            typeof(long)
                        ),
                        new ConstantDescriptor((long)123)
                    ),
                    "$it => (Convert($it.IntProp) == 123)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertDescriptor
                        (
                            new MemberSelectorDescriptor("NullableLongProp", new ParameterDescriptor(parameterName)),
                            typeof(double)
                        ),
                        new ConstantDescriptor(1.23d)
                    ),
                    "$it => (Convert($it.NullableLongProp) == 1.23)"
                },
                new object[]
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new ConvertDescriptor
                        (
                            new ConstantDescriptor(2147483647),
                            typeof(short)
                        ),
                        new ConstantDescriptor(null)
                    ),
                    "$it => (Convert(Convert(2147483647)) != null)"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new ConstantDescriptor(SimpleEnum.Second, typeof(SimpleEnum))
                        ),
                        new ConstantDescriptor("1")
                    ),
                    "$it => (Convert(Second).ToString() == \"1\")"
                },
                new object[]
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new ConvertDescriptor
                            (
                                new ConvertDescriptor
                                (
                                    new MemberSelectorDescriptor("IntProp", new ParameterDescriptor(parameterName)),
                                    typeof(long)
                                ),
                                typeof(short)
                            )
                        ),
                        new ConstantDescriptor("123")
                    ),
                    "$it => (Convert(Convert($it.IntProp)).ToString() == \"123\")"
                },
                new object[]
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new ConvertToEnumDescriptor
                        (
                            "123",
                            typeof(SimpleEnum)
                        ),
                        new ConstantDescriptor(null)
                    ),
                    "$it => (Convert(123) != null)"
                }
            };

        [Theory]
        [MemberData(nameof(CastMethod_Succeeds_Data))]
        public void CastMethod_Succeeds(IExpressionDescriptor filterBody, string expectedResult)
        {
            //act
            var filter = CreateFilter<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter, expectedResult);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }
        #endregion Casts

        #region 'isof' in query option
        public static List<object[]> IsofMethod_Succeeds_Data 
            => new List<object[]>
            {
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(short)
                    ),
                    "$it => IIF(($it Is System.Int16), True, False)"
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(Product)
                    ),
                    "$it => IIF(($it Is LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data.Product), True, False)"
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        typeof(string)
                    ),
                    "$it => IIF(($it.ProductName Is System.String), True, False)"
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName)),
                        typeof(Category)
                    ),
                    "$it => IIF(($it.Category Is LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data.Category), True, False)"
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName)),
                        typeof(DerivedCategory)
                    ),
                    "$it => IIF(($it.Category Is LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data.DerivedCategory), True, False)"
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new MemberSelectorDescriptor("Ranking", new ParameterDescriptor(parameterName)),
                        typeof(SimpleEnum)
                    ),
                    "$it => IIF(($it.Ranking Is LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data.SimpleEnum), True, False)"
                },
            };

        [Theory]
        [MemberData(nameof(IsofMethod_Succeeds_Data))]
        public void IsofMethod_Succeeds(IExpressionDescriptor filterBody, string expectedExpression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expectedExpression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> IsOfPrimitiveType_Succeeds_WithFalse_Data 
            => new List<object[]>
            {
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(byte[])
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(bool)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(byte)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(DateTimeOffset)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(Decimal)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(double)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(TimeSpan)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(Guid)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(Int16)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(Int32)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(Int64)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(sbyte)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(Single)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(System.IO.Stream)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(string)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(SimpleEnum)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(FlagsEnum)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new MemberSelectorDescriptor("ByteArrayProp", new ParameterDescriptor(parameterName)),
                        typeof(byte[])
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new MemberSelectorDescriptor("IntProp", new ParameterDescriptor(parameterName)),
                        typeof(SimpleEnum)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new MemberSelectorDescriptor("NullableShortProp", new ParameterDescriptor(parameterName)),
                        typeof(short)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(byte[])
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(bool)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(byte)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(DateTimeOffset)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(Decimal)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(double)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(TimeSpan)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(Guid)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(Int16)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(Int32)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(Int64)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(sbyte)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(Single)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(System.IO.Stream)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(string)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(SimpleEnum)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(FlagsEnum)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(23),
                        typeof(byte)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(23),
                        typeof(decimal)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(23),
                        typeof(double)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(23),
                        typeof(short)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(23),
                        typeof(long)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(23),
                        typeof(sbyte)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(23),
                        typeof(float)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor("hello"),
                        typeof(Stream)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(0),
                        typeof(FlagsEnum)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(0),
                        typeof(SimpleEnum)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor("2001-01-01T12:00:00.000+08:00"),
                        typeof(DateTimeOffset)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor("00000000-0000-0000-0000-000000000000"),
                        typeof(Guid)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor("23"),
                        typeof(byte)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor("23"),
                        typeof(short)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor("23"),
                        typeof(int)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor("false"),
                        typeof(bool)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor("OData"),
                        typeof(byte[])
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor("PT12H'"),
                        typeof(TimeSpan)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(23),
                        typeof(string)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor("0"),
                        typeof(FlagsEnum)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor("0"),
                        typeof(SimpleEnum)
                    )
                }
            };

        [Theory]
        [MemberData(nameof(IsOfPrimitiveType_Succeeds_WithFalse_Data))]
        public void IsOfPrimitiveType_Succeeds_WithFalse(IExpressionDescriptor filterBody)
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
                    filterBody
                );
        }

        public static List<object[]> IsOfQuotedNonPrimitiveType 
            => new List<object[]>
            {
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        typeof(DerivedProduct)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new MemberSelectorDescriptor("SupplierAddress", new ParameterDescriptor(parameterName)),
                        typeof(Address)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName)),
                        typeof(DerivedCategory)
                    )
                }
            };

        [Theory]
        [MemberData(nameof(IsOfQuotedNonPrimitiveType))]
        public void IsOfQuotedNonPrimitiveType_Succeeds(IExpressionDescriptor filterBody)
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
                    filterBody
                );
        }

        public static List<object[]> IsOfQuotedNonPrimitiveTypeWithNull_Succeeds_WithFalse_Data 
            => new List<object[]>
            {
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(Address)
                    )
                },
                new object []
                {
                    new IsOfDescriptor
                    (
                        new ConstantDescriptor(null),
                        typeof(DerivedCategory)
                    )
                }
            };

        [Theory]
        [MemberData(nameof(IsOfQuotedNonPrimitiveTypeWithNull_Succeeds_WithFalse_Data))]
        public void IsOfQuotedNonPrimitiveTypeWithNull_Succeeds_WithFalse(IExpressionDescriptor filterBody)
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
                    filterBody
                );
        }
        #endregion 'isof' in query option

        #region
        public static List<object[]> ByteArrayComparisons_Data 
            => new List<object[]>
            {
                new object []
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("ByteArrayProp", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(Convert.FromBase64String("I6v/"))
                    ),
                    "$it => ($it.ByteArrayProp == System.Byte[])",
                    true
                },
                new object []
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("ByteArrayProp", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(Convert.FromBase64String("I6v/"))
                    ),
                    "$it => ($it.ByteArrayProp != System.Byte[])",
                    false
                },
                new object []
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(Convert.FromBase64String("I6v/")),
                        new ConstantDescriptor(Convert.FromBase64String("I6v/"))
                    ),
                    "$it => (System.Byte[] == System.Byte[])",
                    true
                },
                new object []
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(Convert.FromBase64String("I6v/")),
                        new ConstantDescriptor(Convert.FromBase64String("I6v/"))
                    ),
                    "$it => (System.Byte[] != System.Byte[])",
                    false
                },
                new object []
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("ByteArrayPropWithNullValue", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(Convert.FromBase64String("I6v/"))
                    ),
                    "$it => ($it.ByteArrayPropWithNullValue != System.Byte[])",
                    true
                },
                new object []
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("ByteArrayPropWithNullValue", new ParameterDescriptor(parameterName)),
                        new MemberSelectorDescriptor("ByteArrayPropWithNullValue", new ParameterDescriptor(parameterName))
                    ),
                    "$it => ($it.ByteArrayPropWithNullValue != $it.ByteArrayPropWithNullValue)",
                    false
                },
                new object []
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("ByteArrayPropWithNullValue", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(null)
                    ),
                    "$it => ($it.ByteArrayPropWithNullValue != null)",
                    false
                },
                new object []
                {
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("ByteArrayPropWithNullValue", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor(null)
                    ),
                    "$it => ($it.ByteArrayPropWithNullValue == null)",
                    true
                },
                new object []
                {
                    new NotEqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new MemberSelectorDescriptor("ByteArrayPropWithNullValue", new ParameterDescriptor(parameterName))
                    ),
                    "$it => (null != $it.ByteArrayPropWithNullValue)",
                    false
                },
                new object []
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(null),
                        new MemberSelectorDescriptor("ByteArrayPropWithNullValue", new ParameterDescriptor(parameterName))
                    ),
                    "$it => (null == $it.ByteArrayPropWithNullValue)",
                    true
                },
            };

        [Theory]
        [MemberData(nameof(ByteArrayComparisons_Data))]
        public void ByteArrayComparisons(IExpressionDescriptor filterBody, string expectedExpression, bool expected)
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
                    filterBody
                );
        }

        public static List<object[]> DisAllowed_ByteArrayComparisons_Data 
            => new List<object[]>
            {
                new object []
                {
                    new GreaterThanOrEqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(Convert.FromBase64String("AP8Q")),
                        new ConstantDescriptor(Convert.FromBase64String("AP8Q"))
                    )
                },
                new object []
                {
                    new LessThanOrEqualsBinaryDescriptor
                    (
                        new ConstantDescriptor(Convert.FromBase64String("AP8Q")),
                        new ConstantDescriptor(Convert.FromBase64String("AP8Q"))
                    )
                },
                new object []
                {
                    new LessThanBinaryDescriptor
                    (
                        new ConstantDescriptor(Convert.FromBase64String("AP8Q")),
                        new ConstantDescriptor(Convert.FromBase64String("AP8Q"))
                    )
                },
                new object []
                {
                    new GreaterThanBinaryDescriptor
                    (
                        new ConstantDescriptor(Convert.FromBase64String("AP8Q")),
                        new ConstantDescriptor(Convert.FromBase64String("AP8Q"))
                    )
                },
            };

        [Theory]
        [MemberData(nameof(DisAllowed_ByteArrayComparisons_Data))]
        public void DisAllowed_ByteArrayComparisons(IExpressionDescriptor filterBody)
        {
            //assert
            Assert.Throws<InvalidOperationException>(() => CreateFilter<DataTypes>());
            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> Nullable_NonstandardEdmPrimitives_Data 
            => new List<object[]>
            {
                new object []
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertDescriptor
                        (
                            new ConvertToNullableUnderlyingValueDescriptor
                            (
                                new MemberSelectorDescriptor("NullableUShortProp", new ParameterDescriptor(parameterName))
                            ),
                            typeof(int?)
                        ),
                        new ConstantDescriptor(12)
                    ),
                    "$it => (Convert($it.NullableUShortProp.Value) == Convert(12))"
                },
                new object []
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertDescriptor
                        (
                            new ConvertToNullableUnderlyingValueDescriptor
                            (
                                new MemberSelectorDescriptor("NullableULongProp", new ParameterDescriptor(parameterName))
                            ),
                            typeof(long?)
                        ),
                        new ConstantDescriptor(12L)
                    ),
                    "$it => (Convert($it.NullableULongProp.Value) == Convert(12))"
                },
                new object []
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertDescriptor
                        (
                            new ConvertToNullableUnderlyingValueDescriptor
                            (
                                new MemberSelectorDescriptor("NullableUIntProp", new ParameterDescriptor(parameterName))
                            ),
                            typeof(int?)
                        ),
                        new ConstantDescriptor(12)
                    ),
                    "$it => (Convert($it.NullableUIntProp.Value) == Convert(12))"
                },
                new object []
                {
                    new EqualsBinaryDescriptor
                    (
                        new ConvertToStringDescriptor
                        (
                            new ConvertToNullableUnderlyingValueDescriptor
                            (
                                new MemberSelectorDescriptor("NullableCharProp", new ParameterDescriptor(parameterName))
                            )
                        ),
                        new ConstantDescriptor("a")
                    ),
                    "$it => ($it.NullableCharProp.Value.ToString() == \"a\")"
                },
            };

        [Theory]
        [MemberData(nameof(Nullable_NonstandardEdmPrimitives_Data))]
        public void Nullable_NonstandardEdmPrimitives(IExpressionDescriptor filterBody, string expectedExpression)
        {
            //act
            var filter = CreateFilter<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter, expectedExpression);
            Assert.Throws<InvalidOperationException>(() => RunFilter(filter, new DataTypes()));

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
                );
        }

        public static List<object[]> InOnNavigation_Data 
            => new List<object[]>
                {
                    new object []
                    {
                        new InDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "ProductID",
                                new MemberSelectorDescriptor
                                (
                                    "Product",
                                    new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                                )
                            ),
                            new CollectionConstantDescriptor
                            (
                                new List<object>{ 1 },
                                typeof(int)
                            )
                        ),
                        "$it => System.Collections.Generic.List`1[System.Int32].Contains($it.Category.Product.ProductID)"
                    },
                    new object []
                    {
                        new InDescriptor
                        (
                            new MemberSelectorDescriptor("Category.Product.ProductID", new ParameterDescriptor(parameterName)),
                            new CollectionConstantDescriptor
                            (
                                new List<object>{ 1 },
                                typeof(int)
                            )
                        ),
                        "$it => System.Collections.Generic.List`1[System.Int32].Contains($it.Category.Product.ProductID)"
                    },
                    new object []
                    {
                        new InDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "GuidProperty",
                                new MemberSelectorDescriptor
                                (
                                    "Product",
                                    new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                                )
                            ),
                            new CollectionConstantDescriptor
                            (
                                new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") },
                                typeof(Guid)
                            )
                        ),
                        "$it => System.Collections.Generic.List`1[System.Guid].Contains($it.Category.Product.GuidProperty)"
                    },
                    new object []
                    {
                        new InDescriptor
                        (
                            new MemberSelectorDescriptor
                            (
                                "NullableGuidProperty",
                                new MemberSelectorDescriptor
                                (
                                    "Product",
                                    new MemberSelectorDescriptor("Category", new ParameterDescriptor(parameterName))
                                )
                            ),
                            new CollectionConstantDescriptor
                            (
                                new List<object>{ new Guid("dc75698b-581d-488b-9638-3e28dd51d8f7") },
                                typeof(Guid?)
                            )
                        ),
                        "$it => System.Collections.Generic.List`1[System.Nullable`1[System.Guid]].Contains($it.Category.Product.NullableGuidProperty)"
                    }
                };

        [Theory]
        [MemberData(nameof(InOnNavigation_Data))]
        public void InOnNavigation(IExpressionDescriptor filterBody, string expectedExpression)
        {
            //act
            var filter = CreateFilter<Product>();

            //assert
            AssertFilterStringIsCorrect(filter, expectedExpression);

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    filterBody
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
                    new OrBinaryDescriptor
                    (
                        new OrBinaryDescriptor
                        (
                            new OrBinaryDescriptor
                            (
                                new EqualsBinaryDescriptor
                                (
                                    new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                                    new ConstantDescriptor("1")
                                ),
                                new EqualsBinaryDescriptor
                                (
                                    new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                                    new ConstantDescriptor("2")
                                )
                            ),
                            new EqualsBinaryDescriptor
                            (
                                new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                                new ConstantDescriptor("3")
                            )
                        ),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                            new ConstantDescriptor("4")
                        )
                    )
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
                    new EqualsBinaryDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new ConstantDescriptor("1")
                    )
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
                    new InDescriptor
                    (
                        new MemberSelectorDescriptor("ProductName", new ParameterDescriptor(parameterName)),
                        new CollectionConstantDescriptor
                        (
                            new List<object> { "Prod1", "Prod2" },
                            typeof(string)
                        )
                    )
                );
        }

        [Fact]
        public void CollectionConstants_OfEnums_Are_Not_Parameterized_If_Disabled()
        {
            //act
            var filter = CreateFilter<DataTypes>();

            //assert
            AssertFilterStringIsCorrect(filter, "$it => System.Collections.Generic.List`1[LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data.SimpleEnum].Contains($it.SimpleEnumProp)");

            Expression<Func<T, bool>> CreateFilter<T>()
                => GetFilter<T>
                (
                    new InDescriptor
                    (
                        new MemberSelectorDescriptor("SimpleEnumProp", new ParameterDescriptor(parameterName)),
                        new CollectionConstantDescriptor
                        (
                            new List<object> { SimpleEnum.First, SimpleEnum.Second },
                            typeof(SimpleEnum)
                        )
                    )
                );
        }
        #endregion

        static MapperConfiguration MapperConfiguration;
        private void Initialize()
        {
            if (MapperConfiguration == null)
            {
                MapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddExpressionMapping();
                    cfg.AddMaps(typeof(MappingProfile).Assembly);
                });
            }

            serviceProvider = new ServiceCollection()
                .AddSingleton<AutoMapper.IConfigurationProvider>
                (
                    MapperConfiguration
                )
                .AddTransient<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService))
                .BuildServiceProvider();
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

        private void AssertFilterStringIsCorrect(Expression expression, string expected)
        {
            string resultExpression = ExpressionStringBuilder.ToString(expression);
            Assert.True(expected == resultExpression, string.Format("Expected expression '{0}' but the deserializer produced '{1}'", expected, resultExpression));
        }

        private Expression<Func<T, bool>> GetFilter<T>(IExpressionDescriptor filterBody)
        {
            IMapper mapper = serviceProvider.GetRequiredService<IMapper>();

            return (Expression<Func<T, bool>>)mapper.Map<FilterLambdaOperator>
            (
                new FilterLambdaDescriptor
                (
                    filterBody, 
                    typeof(T), 
                    parameterName
                ),
                opts => opts.Items["parameters"] = GetParameters()
            ).Build();
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
