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
        public void BuildWhere()
        {
            IDictionary<string, ParameterExpression> parameters = new Dictionary<string, ParameterExpression>
            {
                ["q"] = Expression.Parameter(typeof(IQueryable<Student>), "q")
            };

            //{q => q.Where(s => ((s.ID > 1) AndAlso (Compare(s.FirstName, s.LastName) > 0))).OrderBy(v => v.LastName).ThenByDescending(v => v.FirstName).Skip(2).Take(3).Average(j => j.ID)}
            Expression<Func<IQueryable<Student>, double>> expression = new AverageOperator
            (
                new TakeOperator
                (
                    new SkipOperator
                    (
                        new ThenByOperator
                        (
                            new OrderByOperator
                            (
                                new WhereOperator
                                (
                                    new ParameterOperator(parameters, "q"),
                                    new FilterLambdaOperator
                                    (
                                        parameters,
                                        new AndBinaryOperator
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
                                        typeof(Student),
                                        "s"
                                    )
                                ),
                                new LambdaOperator
                                (
                                    parameters,
                                    new MemberSelector("LastName", new ParameterOperator(parameters, "v")),
                                    typeof(string),
                                    typeof(Student),
                                    "v"
                                ),
                                Strutures.ListSortDirection.Ascending
                            ),
                            new LambdaOperator
                            (
                                parameters,
                                new MemberSelector("FirstName", new ParameterOperator(parameters, "v")),
                                typeof(string),
                                typeof(Student),
                                "v"
                            ),
                            Strutures.ListSortDirection.Descending
                        ),
                        2
                    ),
                    3
                ),
                new LambdaOperator
                (
                    parameters,
                    new MemberSelector("Id", new ParameterOperator(parameters, "j")),
                    typeof(int),
                    typeof(Student),
                    "j"
                )
            )
            .GetExpression<IQueryable<Student>, double>(parameters["q"]);

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
