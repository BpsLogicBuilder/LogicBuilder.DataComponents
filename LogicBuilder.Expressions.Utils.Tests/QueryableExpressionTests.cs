using Contoso.Data.Entities;
using LogicBuilder.Expressions.Utils.ExpressionBuilder;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Collection;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Operand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace LogicBuilder.Expressions.Utils.Tests
{
    public class QueryableExpressionTests
    {
        [Fact]
        public void BuildWhere_OrderBy_ThenBy_Skip_Take_Average()
        {
            IDictionary<string, ParameterExpression> parameters = new Dictionary<string, ParameterExpression>
            {
                ["q"] = Expression.Parameter(typeof(IQueryable<Student>), "q")
            };

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
                                            new MemberSelector("Id", new ParameterOperator(parameters, "s")),
                                            new ConstantOperand(typeof(int), 1)
                                        ),
                                        new GreaterThanBinaryOperator
                                        (
                                            new MemberSelector("FirstName", new ParameterOperator(parameters, "s")),
                                            new MemberSelector("LastName", new ParameterOperator(parameters, "s"))
                                        )
                                    ),
                                    "s"//s => (created in Where operator.  The parameter type is based on the source operand underlying type in this case Student.)
                                ),
                                new MemberSelector("LastName", new ParameterOperator(parameters, "v")),
                                Strutures.ListSortDirection.Ascending,
                                "v"
                            ),
                            new MemberSelector("FirstName", new ParameterOperator(parameters, "v")),
                            Strutures.ListSortDirection.Descending,
                            "v"
                        ),
                        2
                    ),
                    3
                ),
                new MemberSelector("Id", new ParameterOperator(parameters, "j")),
                "j"
            )
            .GetExpression<IQueryable<Student>, double>(parameters["q"]);

            Assert.NotNull(expression);
        }

        [Fact]
        public void BuildGroupBy_OrderBy_ThenBy_Skip_Take_Average()
        {
            IDictionary<string, ParameterExpression> parameters = new Dictionary<string, ParameterExpression>
            {
                ["q"] = Expression.Parameter(typeof(IQueryable<Department>), "q")
            };
            //Expression<Func<IQueryable<Department>, IQueryable<IGrouping<int, Department>>>> expression = q => q.GroupBy(g => 1);
            //{q => q.GroupBy(a => 1).OrderBy(b => b.Key)}
            //{q => q.GroupBy(a => 1).OrderBy(b => b.Key).Select(c => new AnonymousType1() {Sum_budget = q.Where(d => (d.DepartmentID == q.Count())).ToList()})}
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
                        new ConstantOperand(typeof(int), 1),
                        "a"
                    ),
                    new MemberSelector("Key", new ParameterOperator(parameters, "b")),
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
                                        new MemberSelector("DepartmentID", new ParameterOperator(parameters, "d")),
                                        new CountOperator(new ParameterOperator(parameters, "q"))
                                    ),
                                    new EqualsBinaryOperator
                                    (
                                        new MemberSelector("DepartmentID", new ParameterOperator(parameters, "d")),
                                        new MemberSelector("Key", new ParameterOperator(parameters, "c"))
                                    )
                                ),
                                "d"
                            )
                        )
                    }
                ),
                "c"
            )
            .GetExpression<IQueryable<Department>, IQueryable<object>>(parameters["q"]);

            Assert.NotNull(expression);
        }

        [Fact]
        public void Get_Select_New()
        {
            Type queryableType = typeof(IQueryable<Department>);
            ParameterExpression param = Expression.Parameter(queryableType, "q");
            Expression exp = param.GetSelectNew<Department>
            (
                new Dictionary<string, string> { { "Name", "Name" } }
            );
            Assert.NotNull(exp);
        }
    }
}
