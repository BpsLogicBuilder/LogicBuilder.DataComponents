using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Contoso.Contexts;
using Contoso.Data.Entities;
using Contoso.Domain.Entities;
using Contoso.Repositories;
using Contoso.Stores;
using LogicBuilder.EntityFrameworkCore.SqlServer.IntegrationTests.AutoMapperProfiles;
using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.IntegrationTests
{
    public class QueryableExpressionTests
    {
        public QueryableExpressionTests()
        {
            Initialize();
        }

        [Fact]
        public void Select_Group_Students_By_EnrollmentDate_Return_EnrollmentDate_With_Count()
        {
            //arrange
            //Expression<Func<IQueryable<StudentModel>, IQueryable<LookUpsModel>>> expression1 =
            //    q => q.GroupBy(item => item.EnrollmentDate)
            //    .OrderBy(group => group.Key)
            //    .Select
            //    (
            //        sel => new LookUpsModel
            //        {
            //            DateTimeValue = sel.Key,
            //            NumericValue = sel.Count()
            //        }
            //    );


            //arrange
            var selectorLambdaOperatorDescriptor = GetExpressionDescriptor<IQueryable<StudentModel>, IQueryable<LookUpsModel>>
            (
                GetAboutBody(),
                "q"
            );
            var expression = GetExpression<IQueryable<StudentModel>, IQueryable<LookUpsModel>>(selectorLambdaOperatorDescriptor);

            //act
            var result = serviceProvider.GetRequiredService<ISchoolRepository>().QueryAsync<StudentModel, Student, IQueryable<LookUpsModel>, IQueryable<LookUps>>(expression).Result.ToList();

            //assert
            AssertFilterStringIsCorrect(expression, "q => q.GroupBy(item => item.EnrollmentDate).OrderByDescending(group => group.Key).Select(sel => new LookUpsModel() {DateTimeValue = sel.Key, NumericValue = Convert(sel.AsQueryable().Count())})");
            Assert.Equal(6, result.Count);
        }

        #region Fields
        private IServiceProvider serviceProvider;
        #endregion Fields

        #region Helpers
        private static SelectDescriptor GetAboutBody()
            => new()
            {
                SourceOperand = new OrderByDescriptor
                {
                    SourceOperand = new GroupByDescriptor
                    {
                        SourceOperand = new ParameterDescriptor
                        {
                            ParameterName = "q"
                        },
                        SelectorBody = new MemberSelectorDescriptor
                        {
                            MemberFullName = "EnrollmentDate",
                            SourceOperand = new ParameterDescriptor
                            {
                                ParameterName = "item"
                            }
                        },
                        SelectorParameterName = "item"
                    },
                    SortDirection = LogicBuilder.Expressions.Utils.Strutures.ListSortDirection.Descending,
                    SelectorBody = new MemberSelectorDescriptor
                    {
                        MemberFullName = "Key",
                        SourceOperand = new ParameterDescriptor
                        {
                            ParameterName = "group"
                        }
                    },
                    SelectorParameterName = "group"
                },
                SelectorBody = new MemberInitDescriptor
                {
                    MemberBindings = new Dictionary<string, IExpressionDescriptor>
                    {
                        ["DateTimeValue"] = new MemberSelectorDescriptor
                        {
                            MemberFullName = "Key",
                            SourceOperand = new ParameterDescriptor
                            {
                                ParameterName = "sel"
                            }
                        },
                        ["NumericValue"] = new ConvertDescriptor
                        {
                            SourceOperand = new CountDescriptor
                            {
                                SourceOperand = new AsQueryableDescriptor()
                                {
                                    SourceOperand = new ParameterDescriptor
                                    {
                                        ParameterName = "sel"
                                    }
                                }
                            },
                            Type = typeof(double?)
                        }
                    },
                    NewType = typeof(LookUpsModel)
                },
                SelectorParameterName = "sel"
            };

        private static SelectorLambdaDescriptor GetExpressionDescriptor<T, TResult>(IExpressionDescriptor selectorBody, string parameterName = "$it")
            => new()
            {
                Selector = selectorBody,
                SourceElementType = typeof(T),
                ParameterName = parameterName,
                BodyType = typeof(TResult)
            };

        private Expression<Func<T, TResult>> GetExpression<T, TResult>(SelectorLambdaDescriptor selectorLambdaDescriptor)
        {
            IMapper mapper = serviceProvider.GetRequiredService<IMapper>();

            return (Expression<Func<T, TResult>>)mapper.Map<SelectorLambdaOperator>
            (
                selectorLambdaDescriptor,
                opts => opts.Items["parameters"] = new Dictionary<string, ParameterExpression>()
            ).Build();
        }

        private static void AssertFilterStringIsCorrect(Expression expression, string expected)
        {
            string resultExpression = ExpressionStringBuilder.ToString(expression);
            Assert.True(expected == resultExpression, string.Format("Expected expression '{0}' but the deserializer produced '{1}'", expected, resultExpression));
        }

        static MapperConfiguration MapperConfiguration;
        private void Initialize()
        {
            MapperConfiguration ??= new MapperConfiguration(cfg =>
                {
                    cfg.AddExpressionMapping();

                    cfg.AddProfile<SchoolProfile>();
                    cfg.AddProfile<Mapping.ExpressionOperatorsMappingProfile>();
                });
            MapperConfiguration.AssertConfigurationIsValid();
            serviceProvider = new ServiceCollection()
                .AddDbContext<SchoolContext>
                (
                    options => options.UseSqlServer
                    (
                        @"Server=(localdb)\mssqllocaldb;Database=Integration_QueryableExpressionTests;ConnectRetryCount=0",
                        options => options.EnableRetryOnFailure()
                    ),
                    ServiceLifetime.Transient
                )
                .AddTransient<ISchoolStore, SchoolStore>()
                .AddTransient<ISchoolRepository, SchoolRepository>()
                .AddSingleton<AutoMapper.IConfigurationProvider>
                (
                    MapperConfiguration
                )
                .AddTransient<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService))
                .BuildServiceProvider();

            ReCreateDataBase(serviceProvider.GetRequiredService<SchoolContext>());
            DatabaseSeeder.Seed_Database(serviceProvider.GetRequiredService<ISchoolRepository>()).Wait();
        }

        private static void ReCreateDataBase(SchoolContext context)
        {
            context.Database.EnsureCreated();
        }
        #endregion Helpers
    }
}