# LogicBuilder.DataComponents
Packages in this repository include abstractions for mapping LINQ expresions and objects between business and data objects types.

## LogicBuilder.EntityFrameworkCore.SqlServer
* Maps business model objects to and from data objects.
* Maps query expressions from business model to data model.

To use:
```c#
    //Create a context
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {
        }
    }

    //Create Store
    public interface ISchoolStore : IStore
    {
    }
    public class SchoolStore : StoreBase, ISchoolStore
    {
        public SchoolStore(SchoolContext context)
            : base(context)
        {
        }
    }

    //Create Repository
    public interface ISchoolRepository : IContextRepository
    {
    }
    public class SchoolRepository : ContextRepositoryBase, ISchoolRepository
    {
        public SchoolRepository(ISchoolStore store, IMapper mapper) : base(store, mapper)
        {
        }
    }

    //Register Services including AutoMapper profiles.
    IServiceProvider serviceProvider = new ServiceCollection()
                .AddDbContext<SchoolContext>
                (
                    options =>
                    {
                        options.UseInMemoryDatabase("ContosoUniVersity");
                        options.UseInternalServiceProvider(new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider());
                    }
                )
                .AddTransient<ISchoolStore, SchoolStore>()
                .AddTransient<ISchoolRepository, SchoolRepository>()
                .AddSingleton<AutoMapper.IConfigurationProvider>(new MapperConfiguration(cfg => cfg.AddProfiles(typeof(SchoolProfile).GetTypeInfo().Assembly)))
                .AddTransient<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService))
                .BuildServiceProvider();

    //Call the repository
    ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
    ICollection<StudentModel> list = Task.Run(() => repository.GetItemsAsync<StudentModel, Student>()).Result;
```

## LogicBuilder.Kendo.ExpressionExtensions
LogicBuilder.Kendo.ExpressionExtensions depends on Telerik.UI.for.AspNet.Core but has not been created or maintained by Telerik/Progress.  It contains extension methods for creating IQueryable expressions given an instance of Telerik's DataSourceRequest class.

First implement the context, store, repository and service registrations as decribed earlier for LogicBuilder.EntityFrameworkCore.SqlServer.

```c#
    //Use the DataSourceRequest helper to get the DataSourceResult.
    ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
    DataSourceResult result = Task.Run(() => request.GetData<StudentModel, Student>(repository)).Result;

    internal static class Helpers
    {
        public static async Task<DataSourceResult> GetData<TModel, TData>(this DataSourceRequest request, IContextRepository contextRepository, IEnumerable<string> includes = null)
				where TModel : BaseModelClass
				where TData : BaseDataClass
        {
            return await request.GetDataSourceResult<TModel, TData>
            (
		contextRepository,
		includes.BuildIncludesExpressionCollection<TModel>()
            );
        }
    }
```

Refer to [the data request tests](https://github.com/BlaiseD/LogicBuilder.DataComponents/blob/master/LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests/DataRequestTests.cs) for complete examples.

## Filtered Includes
Filtered Includes use EF Core's explicit loading to conditionally load child collections or child references.

```c#
    [Fact]
    public void Get_departments_with_filtered_inlude_with_filter_on_child_include()
    {
        ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
        ICollection<DepartmentModel> list = Task.Run
        (
            () => repository.GetItemsAsync<DepartmentModel, Department>
            (
                d => d.Courses.Count > 0,
				null,
				null,
                new FilteredIncludeExpression[]
                {
                    new FilteredIncludeExpression
                    {
                        Include = LinqHelpers.ToSelector<DepartmentModel, ICollection<CourseModel>>(i => i.Courses),
                        FilteredIncludes = new FilteredIncludeExpression[]
                        {
                            new FilteredIncludeExpression
                            {
                                Include = LinqHelpers.ToSelector<CourseModel, ICollection<CourseAssignmentModel>>(i => i.Assignments),
                                Filter = LinqHelpers.ToFilter<CourseAssignmentModel>(e => e.CourseID == -1)
                            }
                        }
                    }
                }
            )
        ).Result;

        Assert.True(list.First().Courses.Count > 0);
        Assert.True(list.First().Courses.First().Assignments.Count == 0);
    }
```
### Design Considerations for Filtered Includes
* Because filtered includes use explicit loading, only consider using this feature for loading single entities or in conjunction with server-side paging.
* The DbContext's service lifetime must be scoped or transient. LogicBuilder.EntityFrameworkCore.SqlServer sets the context's change tracking behavior to no-tracking (`ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;`).  Attempting to attach previously tracked items will throw an exception

Refer to [the repository tests](https://github.com/BlaiseD/LogicBuilder.DataComponents/blob/master/LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests/RepositoryTests.cs) for more examples.