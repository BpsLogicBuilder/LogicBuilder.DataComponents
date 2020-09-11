using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Contoso.Data.Entities;
using LogicBuilder.EntityFrameworkCore.SqlServer.Tests.Data;
using LogicBuilder.Expressions.Utils;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using LogicBuilder.Expressions.Utils.Strutures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Tests
{
    public class QueryableExpressionTests
    {
        public QueryableExpressionTests()
        {
            Initialize();
        }

        #region Fields
        private IServiceProvider serviceProvider;
        private static readonly string parameterName = "$it";
        #endregion Fields

        [Fact]
        public void BuildWhere_OrderBy_ThenBy_Skip_Take_Average()
        {
            //act
            var descriptor = new AverageDescriptor
            (
                new TakeDescriptor
                (
                    new SkipDescriptor
                    (
                        new ThenByDescriptor
                        (
                            new OrderByDescriptor
                            (
                                new WhereDescriptor
                                (//q.Where(s => ((s.ID > 1) AndAlso (Compare(s.FirstName, s.LastName) > 0)))
                                    new ParameterDescriptor("q"),//q. the source operand
                                    new AndBinaryDescriptor//((s.ID > 1) AndAlso (Compare(s.FirstName, s.LastName) > 0)
                                    (
                                        new GreaterThanBinaryDescriptor
                                        (
                                            new MemberSelectorDescriptor("Id", new ParameterDescriptor("s")),
                                            new ConstantDescriptor(1, typeof(int))
                                        ),
                                        new GreaterThanBinaryDescriptor
                                        (
                                            new MemberSelectorDescriptor("FirstName", new ParameterDescriptor("s")),
                                            new MemberSelectorDescriptor("LastName", new ParameterDescriptor("s"))
                                        )
                                    ),
                                    "s"//s => (created in Where operator.  The parameter type is based on the source operand underlying type in this case Student.)
                                ),
                                new MemberSelectorDescriptor("LastName", new ParameterDescriptor("v")),
                                ListSortDirection.Ascending,
                                "v"
                            ),
                            new MemberSelectorDescriptor("FirstName", new ParameterDescriptor("v")),
                            ListSortDirection.Descending,
                            "v"
                        ),
                        2
                    ),
                    3
                ),
                new MemberSelectorDescriptor("Id", new ParameterDescriptor("j")),
                "j"
            );

            Expression<Func<IQueryable<Student>, double>> expression = GetExpression<IQueryable<Student>, double>(descriptor, "q");

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
            var descriptor = new SelectDescriptor
            (
                new OrderByDescriptor
                (
                    new GroupByDescriptor
                    (
                        new ParameterDescriptor("q"),
                        new ConstantDescriptor(1, typeof(int)),
                        "a"
                    ),
                    new MemberSelectorDescriptor("Key", new ParameterDescriptor("b")),
                    ListSortDirection.Ascending,
                    "b"
                ),
                new MemberInitDescriptor
                (
                    new Dictionary<string, IExpressionDescriptor>
                    {
                        ["Sum_budget"] = new ToListDescriptor
                        (
                            new WhereDescriptor
                            (
                                new ParameterDescriptor("q"),
                                new AndBinaryDescriptor
                                (
                                    new EqualsBinaryDescriptor
                                    (
                                        new MemberSelectorDescriptor("DepartmentID", new ParameterDescriptor("d")),
                                        new CountDescriptor(new ParameterDescriptor("q"))
                                    ),
                                    new EqualsBinaryDescriptor
                                    (
                                        new MemberSelectorDescriptor("DepartmentID", new ParameterDescriptor("d")),
                                        new MemberSelectorDescriptor("Key", new ParameterDescriptor("c"))
                                    )
                                ),
                                "d"
                            )
                        )
                    }
                ),
                "c"
            );

            Expression<Func<IQueryable<Department>, IQueryable<object>>> expression = GetExpression<IQueryable<Department>, IQueryable<object>>(descriptor, "q");

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
            var descriptor = new FirstOrDefaultDescriptor
            (
                new SelectDescriptor
                (
                    new OrderByDescriptor
                    (
                        new AsQueryableDescriptor
                        (
                            new GroupByDescriptor
                            (
                                new ParameterDescriptor("q"),
                                new ConstantDescriptor(1, typeof(int)),
                                "item"
                            )
                        ),
                        new MemberSelectorDescriptor("Key", new ParameterDescriptor("group")),
                        ListSortDirection.Ascending,
                        "group"
                    ),
                    new MemberInitDescriptor
                    (
                        new Dictionary<string, IExpressionDescriptor>
                        {
                            ["Min_administratorName"] = new MinDescriptor
                            (
                                new WhereDescriptor
                                (
                                    new ParameterDescriptor("q"),
                                    new EqualsBinaryDescriptor
                                    (
                                        new ConstantDescriptor(1, typeof(int)),
                                        new MemberSelectorDescriptor("Key", new ParameterDescriptor("sel"))
                                    ),
                                    "d"
                                ),
                                new ConcatDescriptor
                                (
                                    new ConcatDescriptor
                                    (
                                        new MemberSelectorDescriptor("Administrator.LastName", new ParameterDescriptor("item")),
                                        new ConstantDescriptor(" ", typeof(string))
                                    ),
                                    new MemberSelectorDescriptor("Administrator.FirstName", new ParameterDescriptor("item"))
                                ),
                                "item"
                            ),
                            ["Count_name"] = new CountDescriptor
                            (
                                new WhereDescriptor
                                (
                                    new ParameterDescriptor("q"),
                                    new EqualsBinaryDescriptor
                                    (
                                        new ConstantDescriptor(1, typeof(int)),
                                        new MemberSelectorDescriptor("Key", new ParameterDescriptor("sel"))
                                    ),
                                    "d"
                                )
                            ),
                            ["Sum_budget"] = new SumDescriptor
                            (
                                new WhereDescriptor
                                (
                                    new ParameterDescriptor("q"),
                                    new EqualsBinaryDescriptor
                                    (
                                        new ConstantDescriptor(1, typeof(int)),
                                        new MemberSelectorDescriptor("Key", new ParameterDescriptor("sel"))
                                    ),
                                    "d"
                                ),
                                new MemberSelectorDescriptor("Budget", new ParameterDescriptor("item")),
                                "item"
                            ),
                            ["Min_budget"] = new MinDescriptor
                            (
                                new WhereDescriptor
                                (
                                    new ParameterDescriptor("q"),
                                    new EqualsBinaryDescriptor
                                    (
                                        new ConstantDescriptor(1, typeof(int)),
                                        new MemberSelectorDescriptor("Key", new ParameterDescriptor("sel"))
                                    ),
                                    "d"
                                ),
                                new MemberSelectorDescriptor("Budget", new ParameterDescriptor("item")),
                                "item"
                            ),
                            ["Min_startDate"] = new MinDescriptor
                            (
                                new WhereDescriptor
                                (
                                    new ParameterDescriptor("q"),
                                    new EqualsBinaryDescriptor
                                    (
                                        new ConstantDescriptor(1, typeof(int)),
                                        new MemberSelectorDescriptor("Key", new ParameterDescriptor("sel"))
                                    ),
                                    "d"
                                ),
                                new MemberSelectorDescriptor("StartDate", new ParameterDescriptor("item")),
                                "item"
                            )
                        }
                    ),
                    "sel"
                )
            );

            Expression<Func<IQueryable<Department>, object>> expression = GetExpression<IQueryable<Department>, object>(descriptor, "q");

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
                    new AllDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new OrBinaryDescriptor
                        (
                            new EqualsBinaryDescriptor
                            (
                                new MemberSelectorDescriptor("CategoryName", new ParameterDescriptor("a")),
                                new ConstantDescriptor("CategoryOne")
                            ),
                            new EqualsBinaryDescriptor
                            (
                                new MemberSelectorDescriptor("CategoryName", new ParameterDescriptor("a")),
                                new ConstantDescriptor("CategoryTwo")
                            )
                        ),
                        "a"
                    ),
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
                    new AnyDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryName", new ParameterDescriptor("a")),
                            new ConstantDescriptor("CategoryOne")
                        ),
                        "a"
                    ),
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
                    new AnyDescriptor
                    (
                        new ParameterDescriptor(parameterName)
                    ),
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
                    new AsQueryableDescriptor
                    (
                        new ParameterDescriptor(parameterName)
                    ),
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
                    new AverageDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                        "a"
                    ),
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
                    new AverageDescriptor
                    (
                        new SelectDescriptor
                        (
                            new ParameterDescriptor(parameterName),
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            "a"
                        )
                    ),
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
                    new CountDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(1)
                        ),
                        "a"
                    ),
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
                    new CountDescriptor
                    (
                        new ParameterDescriptor(parameterName)
                    ),
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
                    new DistinctDescriptor
                    (
                        new ParameterDescriptor(parameterName)
                    ),
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
                    new FirstDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(-1)
                        ),
                        "a"
                    ),
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
                    new FirstDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(1)
                        ),
                        "a"
                    ),
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
                    new FirstDescriptor
                    (
                        new ParameterDescriptor(parameterName)
                    ),
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
                    new FirstOrDefaultDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(-1)
                        ),
                        "a"
                    ),
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
                    new FirstOrDefaultDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(1)
                        ),
                        "a"
                    ),
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
                    new FirstOrDefaultDescriptor
                    (
                        new ParameterDescriptor(parameterName)
                    ),
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
                    new GroupByDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new MemberSelectorDescriptor("SupplierID", new ParameterDescriptor("a")),
                        "a"
                    ),
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
                    new LastDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(-1)
                        ),
                        "a"
                    ),
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
                    new LastDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(2)
                        ),
                        "a"
                    ),
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
                    new LastDescriptor
                    (
                        new ParameterDescriptor(parameterName)
                    ),
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
                    new LastOrDefaultDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(-1)
                        ),
                        "a"
                    ),
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
                    new LastOrDefaultDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(2)
                        ),
                        "a"
                    ),
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
                    new LastOrDefaultDescriptor
                    (
                        new ParameterDescriptor(parameterName)
                    ),
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
                    new MaxDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                        "a"
                    ),
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
                    new MaxDescriptor
                    (
                        new SelectDescriptor
                        (
                            new ParameterDescriptor(parameterName),
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            "a"
                        )
                    ),
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
                    new MinDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                        "a"
                    ),
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
                    new MinDescriptor
                    (
                        new SelectDescriptor
                        (
                            new ParameterDescriptor(parameterName),
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            "a"
                        )
                    ),
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
                    new OrderByDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                        ListSortDirection.Ascending,
                        "a"
                    ),
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
                    new OrderByDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                        ListSortDirection.Descending,
                        "a"
                    ),
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
                    new ThenByDescriptor
                    (
                        new OrderByDescriptor
                        (
                            new ParameterDescriptor(parameterName),
                            new MemberSelectorDescriptor("SupplierID", new ParameterDescriptor("a")),
                            ListSortDirection.Ascending,
                            "a"
                        ),
                        new MemberSelectorDescriptor("ProductID", new ParameterDescriptor("a")),
                        ListSortDirection.Ascending,
                        "a"
                    ),
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
                    new ThenByDescriptor
                    (
                        new OrderByDescriptor
                        (
                            new ParameterDescriptor(parameterName),
                            new MemberSelectorDescriptor("SupplierID", new ParameterDescriptor("a")),
                            ListSortDirection.Ascending,
                            "a"
                        ),
                        new MemberSelectorDescriptor("ProductID", new ParameterDescriptor("a")),
                        ListSortDirection.Descending,
                        "a"
                    ),
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
            Assert.Equal(4, result.First().AddressID);

            Expression<Func<T, TReturn>> CreateExpression<T, TReturn>()
                => GetExpression<T, TReturn>
                (
                    new TakeDescriptor
                    (
                        new SkipDescriptor
                        (
                            new ThenByDescriptor
                            (
                                new OrderByDescriptor
                                (
                                    new SelectManyDescriptor
                                    (
                                        new ParameterDescriptor(parameterName),
                                        new MemberSelectorDescriptor("AlternateAddresses", new ParameterDescriptor("a")),
                                        "a"
                                    ),
                                    new MemberSelectorDescriptor("State", new ParameterDescriptor("a")),
                                    ListSortDirection.Ascending,
                                    "a"
                                ),
                                new MemberSelectorDescriptor("AddressID", new ParameterDescriptor("a")),
                                ListSortDirection.Ascending,
                                "a"
                            ),
                            1
                        ),
                        2
                    ),
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
                    new SelectDescriptor
                    (
                        new OrderByDescriptor
                        (
                            new ParameterDescriptor(parameterName),
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            ListSortDirection.Descending,
                            "a"
                        ),
                        new MemberInitDescriptor
                        (
                            new Dictionary<string, IExpressionDescriptor>
                            {
                                ["CategoryID"] = new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                                ["CategoryName"] = new MemberSelectorDescriptor("CategoryName", new ParameterDescriptor("a")),
                                ["Products"] = new MemberSelectorDescriptor("Products", new ParameterDescriptor("a"))
                            }
                        ),
                        "a"
                    ),
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
                    new SelectManyDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new MemberSelectorDescriptor("Products", new ParameterDescriptor("a")),
                        "a"
                    ),
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
                    new SingleDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(-1)
                        ),
                        "a"
                    ),
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
                    new SingleDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(1)
                        ),
                        "a"
                    ),
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
                    new SingleDescriptor
                    (
                        new ParameterDescriptor(parameterName)
                    ),
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
                    new SumDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                        "a"
                    ),
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
                    new SumDescriptor
                    (
                        new SelectDescriptor
                        (
                            new ParameterDescriptor(parameterName),
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            "a"
                        )
                    ),
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
                    new ToListDescriptor
                    (
                       new ParameterDescriptor(parameterName)
                    ),
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
                    new WhereDescriptor
                    (
                        new OrderByDescriptor
                        (
                            new ParameterDescriptor(parameterName),
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            ListSortDirection.Descending,
                            "a"
                        ),
                        new NotEqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(1)
                        ),
                        "a"
                    ),
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
                    new WhereDescriptor
                    (
                        new ParameterDescriptor(parameterName),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(-1)
                        ),
                        "a"
                    ),
                    parameterName
                ); ;
        }


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

        private TResult RunExpression<T, TResult>(Expression<Func<T, TResult>> filter, T instance)
            => filter.Compile().Invoke(instance);

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
