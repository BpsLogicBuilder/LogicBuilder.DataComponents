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