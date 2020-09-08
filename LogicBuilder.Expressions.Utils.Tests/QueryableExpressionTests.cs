using Contoso.Data.Entities;
using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Cacnonical;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Collection;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Operand;
using LogicBuilder.Expressions.Utils.Tests.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace LogicBuilder.Expressions.Utils.Tests
{
    public class QueryableExpressionTests
    {
        public QueryableExpressionTests()
        {
            parameters = GetParameters();
        }

        private static readonly string parameterName = "$it";
        private IDictionary<string, ParameterExpression> parameters;

        [Fact]
        public void BuildWhere_OrderBy_ThenBy_Skip_Take_Average()
        {
            //arrange
            var parameters = GetParameters();

            //act
            //{q => q.Where(s => ((s.ID > 1) AndAlso (Compare(s.FirstName, s.LastName) > 0))).OrderBy(v => v.LastName).ThenByDescending(v => v.FirstName).Skip(2).Take(3).Average(j => j.ID)}
            Expression<Func<IQueryable<Student>, double>> expression = new AverageOperator
            (
                parameters,
                new TakeOperator
                (
                    new SkipOperator
                    (
                        new ThenByOperator
                        (
                            parameters,
                            new OrderByOperator
                            (
                                parameters,
                                new WhereOperator
                                (//q.Where(s => ((s.ID > 1) AndAlso (Compare(s.FirstName, s.LastName) > 0)))
                                    parameters,
                                    new ParameterOperator(parameters, "q"),//q. the source operand
                                    new AndBinaryOperator//((s.ID > 1) AndAlso (Compare(s.FirstName, s.LastName) > 0)
                                    (
                                        new GreaterThanBinaryOperator
                                        (
                                            new MemberSelectorOperator("Id", new ParameterOperator(parameters, "s")),
                                            new ConstantOperator(1, typeof(int))
                                        ),
                                        new GreaterThanBinaryOperator
                                        (
                                            new MemberSelectorOperator("FirstName", new ParameterOperator(parameters, "s")),
                                            new MemberSelectorOperator("LastName", new ParameterOperator(parameters, "s"))
                                        )
                                    ),
                                    "s"//s => (created in Where operator.  The parameter type is based on the source operand underlying type in this case Student.)
                                ),
                                new MemberSelectorOperator("LastName", new ParameterOperator(parameters, "v")),
                                Strutures.ListSortDirection.Ascending,
                                "v"
                            ),
                            new MemberSelectorOperator("FirstName", new ParameterOperator(parameters, "v")),
                            Strutures.ListSortDirection.Descending,
                            "v"
                        ),
                        2
                    ),
                    3
                ),
                new MemberSelectorOperator("Id", new ParameterOperator(parameters, "j")),
                "j"
            )
            .GetExpression<IQueryable<Student>, double>(parameters, "q");

            //assert
            AssertFilterStringIsCorrect(expression, "q => q.Where(s => ((s.ID > 1) AndAlso (s.FirstName.Compare(s.LastName) > 0))).OrderBy(v => v.LastName).ThenByDescending(v => v.FirstName).Skip(2).Take(3).Average(j => j.ID)");
        }

        [Fact]
        public void BuildGroupBy_OrderBy_ThenBy_Skip_Take_Average()
        {
            //arrange
            var parameters = GetParameters();

            Expression<Func<IQueryable<Department>, IQueryable<object>>> expression1 =
                q => q.GroupBy(a => 1)
                    .OrderBy(b => b.Key)
                    .Select
                    (
                        c => new 
                        { 
                            Sum_budget = q.Where
                            (
                                d => ((d.DepartmentID == q.Count()) 
                                    && (d.DepartmentID == c.Key))
                            )
                            .ToList()
                        }
                    );

            //act
            Expression<Func<IQueryable<Department>, IQueryable<object>>> expression = new SelectOperator
            (
                parameters,
                new OrderByOperator
                (
                    parameters,
                    new GroupByOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, "q"),
                        new ConstantOperator(1, typeof(int)),
                        "a"
                    ),
                    new MemberSelectorOperator("Key", new ParameterOperator(parameters, "b")),
                    Strutures.ListSortDirection.Ascending,
                    "b"
                ),
                new MemberInitOperator
                (
                    new Dictionary<string, IExpressionPart>
                    {
                        ["Sum_budget"] = new ToListOperator
                        (
                            new WhereOperator
                            (
                                parameters,
                                new ParameterOperator(parameters, "q"),
                                new AndBinaryOperator
                                (
                                    new EqualsBinaryOperator
                                    (
                                        new MemberSelectorOperator("DepartmentID", new ParameterOperator(parameters, "d")),
                                        new CountOperator(new ParameterOperator(parameters, "q"))
                                    ),
                                    new EqualsBinaryOperator
                                    (
                                        new MemberSelectorOperator("DepartmentID", new ParameterOperator(parameters, "d")),
                                        new MemberSelectorOperator("Key", new ParameterOperator(parameters, "c"))
                                    )
                                ),
                                "d"
                            )
                        )
                    }
                ),
                "c"
            )
            .GetExpression<IQueryable<Department>, IQueryable<object>>(parameters, "q");

            //assert
            Assert.NotNull(expression);
        }

        [Fact]
        public void BuildGroupBy_AsQueryable_OrderBy_Select_FirstOrDefault()
        {
            //arrange
            var parameters = GetParameters();

            Expression<Func<IQueryable<Department>, object>> expression1 =
                q => q.GroupBy(item => 1)
                .AsQueryable()
                .OrderBy(group => group.Key)
                .Select
                (
                    sel => new
                    {
                        Min_administratorName = q.Where(d => (1 == sel.Key)).Min(item => string.Concat(string.Concat(item.Administrator.LastName, " "), item.Administrator.FirstName)),
                        Count_name = q.Where(d => (1 == sel.Key)).Count(),
                        Sum_budget = q.Where(d => (1 == sel.Key)).Sum(item => item.Budget),
                        Min_budget = q.Where(d => (1 == sel.Key)).Min(item => item.Budget),
                        Min_startDate = q.Where(d => (1 == sel.Key)).Min(item => item.StartDate)
                    }
                )
                .FirstOrDefault();

            //act
            Expression<Func<IQueryable<Department>, object>> expression = new FirstOrDefaultOperator
            (
                new SelectOperator
                (
                    parameters,
                    new OrderByOperator
                    (
                        parameters,
                        new AsQueryableOperator
                        (
                            new GroupByOperator
                            (
                                parameters,
                                new ParameterOperator(parameters, "q"),
                                new ConstantOperator(1, typeof(int)),
                                "item"
                            )
                        ),
                        new MemberSelectorOperator("Key", new ParameterOperator(parameters, "group")),
                        Strutures.ListSortDirection.Ascending,
                        "group"
                    ),
                    new MemberInitOperator
                    (
                        new Dictionary<string, IExpressionPart>
                        {
                            ["Min_administratorName"] = new MinOperator
                            (
                                parameters,
                                new WhereOperator
                                (
                                    parameters,
                                    new ParameterOperator(parameters, "q"),
                                    new EqualsBinaryOperator
                                    (
                                        new ConstantOperator(1, typeof(int)),
                                        new MemberSelectorOperator("Key", new ParameterOperator(parameters, "sel"))
                                    ),
                                    "d"
                                ),
                                new ConcatOperator
                                (
                                    new ConcatOperator
                                    (
                                        new MemberSelectorOperator("Administrator.LastName", new ParameterOperator(parameters, "item")), 
                                        new ConstantOperator(" ", typeof(string))
                                    ),
                                    new MemberSelectorOperator("Administrator.FirstName", new ParameterOperator(parameters, "item"))
                                ),
                                "item"
                            ),
                            ["Count_name"] = new CountOperator
                            (
                                new WhereOperator
                                (
                                    parameters,
                                    new ParameterOperator(parameters, "q"),
                                    new EqualsBinaryOperator
                                    (
                                        new ConstantOperator(1, typeof(int)),
                                        new MemberSelectorOperator("Key", new ParameterOperator(parameters, "sel"))
                                    ),
                                    "d"
                                )
                            ),
                            ["Sum_budget"] = new SumOperator
                            (
                                parameters,
                                new WhereOperator
                                (
                                    parameters,
                                    new ParameterOperator(parameters, "q"),
                                    new EqualsBinaryOperator
                                    (
                                        new ConstantOperator(1, typeof(int)),
                                        new MemberSelectorOperator("Key", new ParameterOperator(parameters, "sel"))
                                    ),
                                    "d"
                                ),
                                new MemberSelectorOperator("Budget", new ParameterOperator(parameters, "item")),
                                "item"
                            ),
                            ["Min_budget"] = new MinOperator
                            (
                                parameters,
                                new WhereOperator
                                (
                                    parameters,
                                    new ParameterOperator(parameters, "q"),
                                    new EqualsBinaryOperator
                                    (
                                        new ConstantOperator(1, typeof(int)),
                                        new MemberSelectorOperator("Key", new ParameterOperator(parameters, "sel"))
                                    ),
                                    "d"
                                ),
                                new MemberSelectorOperator("Budget", new ParameterOperator(parameters, "item")),
                                "item"
                            ),
                            ["Min_startDate"] = new MinOperator
                            (
                                parameters,
                                new WhereOperator
                                (
                                    parameters,
                                    new ParameterOperator(parameters, "q"),
                                    new EqualsBinaryOperator
                                    (
                                        new ConstantOperator(1, typeof(int)),
                                        new MemberSelectorOperator("Key", new ParameterOperator(parameters, "sel"))
                                    ),
                                    "d"
                                ),
                                new MemberSelectorOperator("StartDate", new ParameterOperator(parameters, "item")),
                                "item"
                            )
                        }
                    ),
                    "sel"
                )
            ).GetExpression<IQueryable<Department>, object>(parameters, "q");

            //assert
            Assert.NotNull(expression);
        }

        [Fact]
        public void All_Filter()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, bool>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.All(a => ((a.CategoryName == \"CategoryOne\") OrElse (a.CategoryName == \"CategoryTwo\")))");
            Assert.True(result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new AllOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new OrBinaryOperator
                        (
                            new EqualsBinaryOperator
                            (
                                new MemberSelectorOperator("CategoryName", new ParameterOperator(parameters, "a")),
                                new ConstantOperator("CategoryOne")
                            ),
                            new EqualsBinaryOperator
                            (
                                new MemberSelectorOperator("CategoryName", new ParameterOperator(parameters, "a")),
                                new ConstantOperator("CategoryTwo")
                            )
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Any_Filter()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, bool>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Any(a => (a.CategoryName == \"CategoryOne\"))");
            Assert.True(result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new AnyOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryName", new ParameterOperator(parameters, "a")),
                            new ConstantOperator("CategoryOne")
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Any()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, bool>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Any()");
            Assert.True(result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new AnyOperator
                    (
                        new ParameterOperator(parameters, parameterName)
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void AsQueryable()
        {
            //act
            var expression = CreateExpression<IEnumerable<Category>, IQueryable<Category>>();
            var result = RunExpression(expression, new List<Category> { new Category() });

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.AsQueryable()");
            Assert.True(result.GetType().IsIQueryable());

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new AsQueryableOperator
                    (
                        new ParameterOperator(parameters, parameterName)
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Average_Selector()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, double>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Average(a => a.CategoryID)");
            Assert.Equal(1.5, result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new AverageOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Average()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, double>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Select(a => a.CategoryID).Average()");
            Assert.Equal(1.5, result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new AverageOperator
                    (
                        new SelectOperator
                        (
                            parameters,
                            new ParameterOperator(parameters, parameterName),
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            "a"
                        )
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Count_Filter()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, int>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Count(a => (a.CategoryID == 1))");
            Assert.Equal(1, result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new CountOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(1)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Count()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, int>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Count()");
            Assert.Equal(2, result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new CountOperator
                    (
                        new ParameterOperator(parameters, parameterName)
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Distinct()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, IQueryable<Category>>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Distinct()");
            Assert.Equal(2, result.Count());

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new DistinctOperator
                    (
                        new ParameterOperator(parameters, parameterName)
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void First_Filter_Throws_Exception()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.First(a => (a.CategoryID == -1))");
            Assert.Throws<InvalidOperationException>(() => RunExpression(expression, GetCategories()));

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new FirstOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(-1)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void First_Filter_Returns_match()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.First(a => (a.CategoryID == 1))");
            Assert.Equal(1, result.CategoryID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new FirstOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(1)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void First()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.First()");
            Assert.NotNull(result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new FirstOperator
                    (
                        new ParameterOperator(parameters, parameterName)
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void FirstOrDefault_Filter_Returns_null()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.FirstOrDefault(a => (a.CategoryID == -1))");
            Assert.Null(result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new FirstOrDefaultOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(-1)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void FirstOrDefault_Filter_Returns_match()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.FirstOrDefault(a => (a.CategoryID == 1))");
            Assert.Equal(1, result.CategoryID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new FirstOrDefaultOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(1)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void FirstOrDefault()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.FirstOrDefault()");
            Assert.NotNull(result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new FirstOrDefaultOperator
                    (
                        new ParameterOperator(parameters, parameterName)
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void GroupBy()
        {
            //act
            var expression = CreateExpression<IQueryable<Product>, IQueryable<IGrouping<int, Product>>>();
            var result = RunExpression(expression, GetProducts());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.GroupBy(a => a.SupplierID)");
            Assert.Equal(1, result.Count());
            Assert.Equal(2, result.First().Count());
            Assert.Equal(3, result.First().First().SupplierID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new GroupByOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new MemberSelectorOperator("SupplierID", new ParameterOperator(parameters, "a")),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Last_Filter_Throws_Exception()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Last(a => (a.CategoryID == -1))");
            Assert.Throws<InvalidOperationException>(() => RunExpression(expression, GetCategories()));

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new LastOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(-1)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Last_Filter_Returns_match()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Last(a => (a.CategoryID == 2))");
            Assert.Equal(2, result.CategoryID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new LastOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(2)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Last()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Last()");
            Assert.NotNull(result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new LastOperator
                    (
                        new ParameterOperator(parameters, parameterName)
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void LastOrDefault_Filter_Returns_null()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.LastOrDefault(a => (a.CategoryID == -1))");
            Assert.Null(result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new LastOrDefaultOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(-1)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void LastOrDefault_Filter_Returns_match()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.LastOrDefault(a => (a.CategoryID == 2))");
            Assert.Equal(2, result.CategoryID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new LastOrDefaultOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(2)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void LastOrDefault()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.LastOrDefault()");
            Assert.NotNull(result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new LastOrDefaultOperator
                    (
                        new ParameterOperator(parameters, parameterName)
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Max_Selector()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, int>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Max(a => a.CategoryID)");
            Assert.Equal(2, result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new MaxOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Max()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, int>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Select(a => a.CategoryID).Max()");
            Assert.Equal(2, result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new MaxOperator
                    (
                        new SelectOperator
                        (
                            parameters,
                            new ParameterOperator(parameters, parameterName),
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            "a"
                        )
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Min_Selector()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, int>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Min(a => a.CategoryID)");
            Assert.Equal(1, result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new MinOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Min()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, int>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Select(a => a.CategoryID).Min()");
            Assert.Equal(1, result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new MinOperator
                    (
                        new SelectOperator
                        (
                            parameters,
                            new ParameterOperator(parameters, parameterName),
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            "a"
                        )
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void OrderBy()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, IOrderedQueryable<Category>>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.OrderBy(a => a.CategoryID)");
            Assert.Equal(1, result.First().CategoryID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new OrderByOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                        Strutures.ListSortDirection.Ascending,
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void OrderByDescending()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, IOrderedQueryable<Category>>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.OrderByDescending(a => a.CategoryID)");
            Assert.Equal(2, result.First().CategoryID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new OrderByOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                        Strutures.ListSortDirection.Descending,
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void OrderByThenBy()
        {
            //act
            var expression = CreateExpression<IQueryable<Product>, IOrderedQueryable<Product>>();
            var result = RunExpression(expression, GetProducts());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.OrderBy(a => a.SupplierID).ThenBy(a => a.ProductID)");
            Assert.Equal(1, result.First().ProductID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new ThenByOperator
                    (
                        parameters,
                        new OrderByOperator
                        (
                            parameters,
                            new ParameterOperator(parameters, parameterName),
                            new MemberSelectorOperator("SupplierID", new ParameterOperator(parameters, "a")),
                            Strutures.ListSortDirection.Ascending,
                            "a"
                        ),
                        new MemberSelectorOperator("ProductID", new ParameterOperator(parameters, "a")),
                        Strutures.ListSortDirection.Ascending,
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void OrderByThenByDescending()
        {
            //act
            var expression = CreateExpression<IQueryable<Product>, IOrderedQueryable<Product>>();
            var result = RunExpression(expression, GetProducts());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.OrderBy(a => a.SupplierID).ThenByDescending(a => a.ProductID)");
            Assert.Equal(2, result.First().ProductID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new ThenByOperator
                    (
                        parameters,
                        new OrderByOperator
                        (
                            parameters,
                            new ParameterOperator(parameters, parameterName),
                            new MemberSelectorOperator("SupplierID", new ParameterOperator(parameters, "a")),
                            Strutures.ListSortDirection.Ascending,
                            "a"
                        ),
                        new MemberSelectorOperator("ProductID", new ParameterOperator(parameters, "a")),
                        Strutures.ListSortDirection.Descending,
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Paging()
        {
            //act
            var expression = CreateExpression<IQueryable<Product>, IQueryable<Address>>();
            var result = RunExpression(expression, GetProducts());

            //assert
            AssertFilterStringIsCorrect
            (
                expression,
                "$it => $it.SelectMany(a => a.AlternateAddresses).OrderBy(a => a.State).ThenBy(a => a.AddressID).Skip(1).Take(2)"
            );
            Assert.Equal(2, result.Count());
            Assert.Equal(4, result.First().AddressID );

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new TakeOperator
                    (
                        new SkipOperator
                        (
                            new ThenByOperator
                            (
                                parameters,
                                new OrderByOperator
                                (
                                    parameters,
                                    new SelectManyOperator
                                    (
                                        parameters,
                                        new ParameterOperator(parameters, parameterName),
                                        new MemberSelectorOperator("AlternateAddresses", new ParameterOperator(parameters, "a")),
                                        "a"
                                    ),
                                    new MemberSelectorOperator("State", new ParameterOperator(parameters, "a")),
                                    Strutures.ListSortDirection.Ascending,
                                    "a"
                                ),
                                new MemberSelectorOperator("AddressID", new ParameterOperator(parameters, "a")),
                                Strutures.ListSortDirection.Ascending,
                                "a"
                            ),
                            1
                        ),
                        2
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Select_New()
        {
            var expression = CreateExpression<IQueryable<Category>, IQueryable<dynamic>>();
            var result = RunExpression(expression, GetCategories());

            Assert.Equal(2, result.First().CategoryID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new SelectOperator
                    (
                        parameters,
                        new OrderByOperator
                        (
                            parameters,
                            new ParameterOperator(parameters, parameterName),
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            Strutures.ListSortDirection.Descending,
                            "a"
                        ),
                        new MemberInitOperator
                        (
                            new Dictionary<string, IExpressionPart>
                            {
                                ["CategoryID"] = new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                                ["CategoryName"] = new MemberSelectorOperator("CategoryName", new ParameterOperator(parameters, "a")),
                                ["Products"] = new MemberSelectorOperator("Products", new ParameterOperator(parameters, "a"))
                            }
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void SelectMany()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, IQueryable<Product>>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.SelectMany(a => a.Products)");
            Assert.Equal(3, result.Count());

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new SelectManyOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new MemberSelectorOperator("Products", new ParameterOperator(parameters, "a")),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Single_Filter_Throws_Exception()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Single(a => (a.CategoryID == -1))");
            Assert.Throws<InvalidOperationException>(() => RunExpression(expression, GetCategories()));

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new SingleOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(-1)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Single_Filter_Returns_match()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Single(a => (a.CategoryID == 1))");
            Assert.Equal(1, result.CategoryID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new SingleOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(1)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Single_with_multiple_matches_Throws_Exception()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, Category>();

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Single()");
            Assert.Throws<InvalidOperationException>(() => RunExpression(expression, GetCategories()));

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new SingleOperator
                    (
                        new ParameterOperator(parameters, parameterName)
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Sum_Selector()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, int>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Sum(a => a.CategoryID)");
            Assert.Equal(3, result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new SumOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Sum()
        {
            //act
            var expression = CreateExpression<IQueryable<Category>, int>();
            var result = RunExpression(expression, GetCategories());

            //assert
            AssertFilterStringIsCorrect(expression, "$it => $it.Select(a => a.CategoryID).Sum()");
            Assert.Equal(3, result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new SumOperator
                    (
                        new SelectOperator
                        (
                            parameters,
                            new ParameterOperator(parameters, parameterName),
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            "a"
                        )
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void ToList()
        {
            var expression = CreateExpression<IQueryable<Category>, List<Category>>();
            var result = RunExpression(expression, GetCategories());

            Assert.Equal(2, result.Count);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new ToListOperator
                    (
                       new ParameterOperator(parameters, parameterName)
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Where_with_matches()
        {
            var expression = CreateExpression<IQueryable<Category>, IQueryable<Category>>();
            var result = RunExpression(expression, GetCategories());

            Assert.Equal(2, result.First().CategoryID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new WhereOperator
                    (
                        parameters,
                        new OrderByOperator
                        (
                            parameters,
                            new ParameterOperator(parameters, parameterName),
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            Strutures.ListSortDirection.Descending,
                            "a"
                        ),
                        new NotEqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(1)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                );
        }

        [Fact]
        public void Where_without_matches()
        {
            var expression = CreateExpression<IQueryable<Category>, IQueryable<Category>>();
            var result = RunExpression(expression, GetCategories());

            Assert.Empty(result);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new WhereOperator
                    (
                        parameters,
                        new ParameterOperator(parameters, parameterName),
                        new EqualsBinaryOperator
                        (
                            new MemberSelectorOperator("CategoryID", new ParameterOperator(parameters, "a")),
                            new ConstantOperator(-1)
                        ),
                        "a"
                    ),
                    parameters,
                    parameterName
                ); ;
        }

        private void AssertFilterStringIsCorrect(Expression expression, string expected)
        {
            AssertStringIsCorrect(ExpressionStringBuilder.ToString(expression));

            void AssertStringIsCorrect(string resultExpression) 
                => Assert.True
                (
                    expected == resultExpression, 
                    $"Expected expression '{expected}' but the deserializer produced '{resultExpression}'"
                );
        }

        private static IDictionary<string, ParameterExpression> GetParameters()
            => new Dictionary<string, ParameterExpression>();

        static object[] GetArguments(IDictionary<string, ParameterExpression> parameters, Func<IDictionary<string, ParameterExpression>, object[]> getList)
            => getList(parameters);

        static object[] GetArguments(Func<IDictionary<string, ParameterExpression>, object[]> getList)
            => GetArguments(GetParameters(), getList);

        private Expression<Func<T, bool>> GetFilter<T>(IExpressionPart filterBody, IDictionary<string, ParameterExpression> parameters, string parameterName = "$it")
            => filterBody.GetFilter<T>(parameters, parameterName);

        private Expression<Func<T, TResult>> GetExpression<T, TResult>(IExpressionPart filterBody, IDictionary<string, ParameterExpression> parameters, string parameterName = "$it")
            => filterBody.GetExpression<T, TResult>(parameters, parameterName);

        private TResult RunExpression<T, TResult>(Expression<Func<T, TResult>> filter, T instance)
            => filter.Compile().Invoke(instance);

        private IQueryable<Category> GetCategories()
         => new Category[]
            {
                new Category
                {
                    CategoryID = 1,
                    CategoryName = "CategoryOne",
                    Products = new Product[]
                    {
                        new Product
                        {
                            ProductID = 1,
                            ProductName = "ProductOne",
                            AlternateAddresses = new Address[]
                            {
                                new Address { AddressID = 1, City = "CityOne" },
                                new Address { AddressID = 2, City = "CityTwo"  },
                            }
                        },
                        new Product
                        {
                            ProductID = 2,
                            ProductName = "ProductTwo",
                            AlternateAddresses = new Address[]
                            {
                                new Address { AddressID = 3, City = "CityThree" },
                                new Address { AddressID = 4, City = "CityFour"  },
                            }
                        }
                    }
                },
                new Category
                {
                    CategoryID = 2,
                    CategoryName = "CategoryTwo",
                    Products =  new Product[]
                    {
                        new Product
                        {
                            AlternateAddresses = new Address[0]
                        }
                    }
                }
            }.AsQueryable();

        private IQueryable<Product> GetProducts()
         => new Product[]
         {
             new Product
             {
                 ProductID = 1,
                 ProductName = "ProductOne",
                 SupplierID = 3,
                 AlternateAddresses = new Address[]
                 {
                     new Address { AddressID = 1, City = "CityOne", State = "OH" },
                     new Address { AddressID = 2, City = "CityTwo", State = "MI"   },
                 }
             },
             new Product
             {
                 ProductID = 2,
                 ProductName = "ProductTwo",
                 SupplierID = 3,
                 AlternateAddresses = new Address[]
                 {
                     new Address { AddressID = 3, City = "CityThree", State = "OH"  },
                     new Address { AddressID = 4, City = "CityFour", State = "MI"   },
                 }
             }
         }.AsQueryable();
    }
}
