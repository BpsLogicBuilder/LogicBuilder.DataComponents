using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Contoso.Contexts;
using Contoso.Data.Entities;
using Contoso.Domain.Entities;
using Contoso.Repositories;
using Contoso.Stores;
using LogicBuilder.EntityFrameworkCore.SqlServer.IntegrationTests.AutoMapperProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.IntegrationTests
{
    public class PersistenceTest
    {
        public PersistenceTest()
        {
            Initialize();
        }

        #region Fields
        private IServiceProvider serviceProvider;
        #endregion Fields

        [Fact]
        public void SaveDepartmentSpecifyingRowVersionExpansion()
        {
            //arrange
            ISchoolRepository schoolRepository = serviceProvider.GetRequiredService<ISchoolRepository>();
            var department = schoolRepository.GetAsync<DepartmentModel, Department>
            (
                s => s.Name == "Mathematics",
                selectExpandDefinition: new LogicBuilder.Expressions.Utils.Expansions.SelectExpandDefinition
                {
                    ExpandedItems = new List<LogicBuilder.Expressions.Utils.Expansions.SelectExpandItem>
                    {//Need expansion because RowVersion is not a literal type (included without explicit expansion)
                     //Or use GetItemsAsync which does not use projection.
                     //Todo include check for typeof(byte[]) in LogicBuilder.Expressions.Utils.TypeExtension.GetValueTypeMembers()
                        new LogicBuilder.Expressions.Utils.Expansions.SelectExpandItem
                        {
                            MemberName = "RowVersion"
                        }
                    }
                }
            ).Result.Single();
            department.Budget = 1000.1m;
            department.EntityState = LogicBuilder.Domain.EntityStateType.Modified;

            //act
            bool result = schoolRepository.SaveAsync<DepartmentModel, Department>(department).Result;
            var savedDepartment = schoolRepository.GetAsync<DepartmentModel, Department>
            (
                s => s.Name == "Mathematics"
            ).Result.Single();

            //assert
            Assert.True(result);
            Assert.Equal(1000.1m, savedDepartment.Budget);
        }

        [Fact]
        public void SaveDepartmentWithoutSpecifyingRowVersionExpansion()
        {
            //arrange
            ISchoolRepository schoolRepository = serviceProvider.GetRequiredService<ISchoolRepository>();
            var department = schoolRepository.GetAsync<DepartmentModel, Department>
            (
                s => s.Name == "Mathematics"
            ).Result.Single();
            department.Budget = 1000.1m;
            department.EntityState = LogicBuilder.Domain.EntityStateType.Modified;

            //act
            //Assert.Throws<AggregateException>
            //(
            //    () =>
            //    {
            //        bool result = schoolRepository.SaveAsync<DepartmentModel, Department>(department).Result;
            //    }
            //);
            bool result = schoolRepository.SaveAsync<DepartmentModel, Department>(department).Result;
            var savedDepartment = schoolRepository.GetAsync<DepartmentModel, Department>
            (
                s => s.Name == "Mathematics"
            ).Result.Single();

            //assert
            Assert.True(result);
            Assert.Equal(1000.1m, savedDepartment.Budget);
        }

        #region Helpers
        static MapperConfiguration MapperConfiguration;
        private void Initialize()
        {
            if (MapperConfiguration == null)
            {
                MapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddExpressionMapping();

                    cfg.AddProfile<SchoolProfile>();
                });
            }
            MapperConfiguration.AssertConfigurationIsValid();
            serviceProvider = new ServiceCollection()
                .AddDbContext<SchoolContext>
                (
                    options => options.UseSqlServer
                    (
                        @"Server=(localdb)\mssqllocaldb;Database=PersistenceTest;ConnectRetryCount=0",
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
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
        #endregion Helpers
    }
}
