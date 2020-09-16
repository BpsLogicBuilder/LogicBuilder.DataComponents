using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Contoso.Contexts;
using Contoso.Data.Entities;
using Contoso.Domain.Entities;
using Contoso.Repositories;
using Kendo.Mvc.UI;
using LogicBuilder.EntityFrameworkCore.SqlServer.Mapping;
using LogicBuilder.Expressions.Utils;
using LogicBuilder.Expressions.Utils.DataSource;
using LogicBuilder.Expressions.Utils.Expansions;
using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using LogicBuilder.Expressions.Utils.Strutures;
using LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests.AutoMapperProfiles;
using LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests.Data;
using LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests
{
    public class DataRequestTests
    {
        public DataRequestTests()
        {
            Initialize();
        }

        #region Fields
        private IServiceProvider serviceProvider;
        #endregion Fields

        #region Tests
        [Fact]
        public void Get_students_ungrouped_with_aggregates()
        {
            DataRequest request = new DataRequest
            {
                Options = new DataSourceRequestOptions
                {
                    Aggregate = "lastName-count~enrollmentDate-min",
                    Filter = null,
                    Group = null,
                    Page = 1,
                    Sort = "enrollmentDate-asc",
                    PageSize = 5
                },
                Includes = null,
                Selects = null,
                Distinct = false
            };

            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            DataSourceResult result = Task.Run(() => request.GetData<StudentModel, Student>(repository)).Result;

            Assert.Equal(11, result.Total);
            Assert.Equal(5, ((IEnumerable<StudentModel>)result.Data).Count());
            Assert.Equal("Nino", ((IEnumerable<StudentModel>)result.Data).First().FirstName);
            Assert.Equal(2, result.AggregateResults.Count());
            Assert.Equal("Count", result.AggregateResults.First().AggregateMethodName);
            Assert.Equal(11, (int)result.AggregateResults.First().Value);
        }

        [Fact]
        public void Get_students_grouped_with_aggregates()
        {
            DataRequest request = new DataRequest
            {
                Options = new DataSourceRequestOptions
                {
                    Aggregate = "lastName-count~enrollmentDate-min",
                    Filter = null,
                    Group = "enrollmentDate-asc",
                    Page = 1,
                    Sort = null,
                    PageSize = 5
                },
                Includes = null,
                Selects = null,
                Distinct = false
            };

            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            DataSourceResult result = Task.Run(() => request.GetData<StudentModel, Student>(repository)).Result;

            Assert.Equal(11, result.Total);
            Assert.Equal(3, ((IEnumerable<AggregateFunctionsGroupModel<StudentModel>>)result.Data).Count());
            Assert.Equal(2, result.AggregateResults.Count());
            Assert.Equal("Count", result.AggregateResults.First().AggregateMethodName);
            Assert.Equal(11, (int)result.AggregateResults.First().Value);
        }

        [Fact]
        public void Get_departments_ungrouped_with_aggregates_and_includes()
        {
            DataRequest request = new DataRequest
            {
                Options = new DataSourceRequestOptions
                {
                    //Queryable.Min<TSource, string> throws System.ArgumentException against In-Memory DB
                    //Aggregate = "administratorName-min~name-count~budget-sum~budget-min~startDate-min",
                    Aggregate = "administratorName-count~name-count~budget-sum~budget-min~startDate-min",
                    Filter = null,
                    Group = null,
                    Page = 1,
                    Sort = null,
                    PageSize = 5
                },
                Includes = new string[] { "administratorName" },
                Selects = null,
                Distinct = false
            };

            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            DataSourceResult result = Task.Run(() => request.GetData<DepartmentModel, Department>(repository)).Result;

            Assert.Equal(4, result.Total);
            Assert.Equal(4, ((IEnumerable<DepartmentModel>)result.Data).Count());
            Assert.Equal(5, result.AggregateResults.Count());
            Assert.Equal("Kim Abercrombie", ((IEnumerable<DepartmentModel>)result.Data).First().AdministratorName);
            Assert.Equal("Count", result.AggregateResults.First().AggregateMethodName);
            //Queryable.Min<TSource, string> throws System.ArgumentException against In - Memory DB
           // Assert.Equal("Candace Kapoor", (string)result.AggregateResults.First().Value);
        }

        [Fact]
        public void Get_departments_grouped_with_aggregates()
        {
            DataRequest request = new DataRequest
            {
                Options = new DataSourceRequestOptions
                {
                    //Queryable.Min<TSource, string> throws System.ArgumentException against In-Memory DB
                    //Aggregate = "administratorName-min~name-count~budget-sum~budget-min~startDate-min",
                    Aggregate = "administratorName-count~name-count~budget-sum~budget-min~startDate-min",
                    Filter = null,
                    Group = "budget-asc",
                    Page = 1,
                    Sort = null,
                    PageSize = 5
                },
                Includes = new string[] { "administratorName" },
                Selects = null,
                Distinct = false
            };

            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            DataSourceResult result = Task.Run(() => request.GetData<DepartmentModel, Department>(repository)).Result;

            Assert.Equal(4, result.Total);
            Assert.Equal(2, ((IEnumerable<AggregateFunctionsGroupModel<DepartmentModel>>)result.Data).Count());
            Assert.Equal(5, result.AggregateResults.Count());
            Assert.Equal("Count", result.AggregateResults.First().AggregateMethodName);
            //Queryable.Min<TSource, string> throws System.ArgumentException against In-Memory DB
            //Assert.Equal("Candace Kapoor", (string)result.AggregateResults.First().Value);
        }

        [Fact]
        public void Get_single_student_with_navigation_property_of_navigation_property()
        {
            DataRequest request = new DataRequest
            {
                Options = new DataSourceRequestOptions
                {
                    Aggregate = null,
                    Filter = "id~eq~3",
                    Group = null,
                    Page = 0,
                    Sort = null,
                    PageSize = 0
                },
                Includes = new string[] { "enrollments.courseTitle" },
                Selects = null,
                Distinct = false
            };

            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            StudentModel result = Task.Run(() => request.GetSingle<StudentModel, Student>(repository)).Result;

            Assert.Equal("Chemistry", result.Enrollments.First(e => !e.Grade.HasValue).CourseTitle);
            Assert.Equal("Arturo Anand", result.FullName);
        }

        [Fact]
        public void Get_single_department()
        {
            DataRequest request = new DataRequest
            {
                Options = new DataSourceRequestOptions
                {
                    Aggregate = null,
                    Filter = "departmentID~eq~2",
                    Group = null,
                    Page = 1,
                    Sort = null,
                    PageSize = 5
                },
                Includes = null,
                Selects = null,
                Distinct = false
            };

            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            DepartmentModel result = Task.Run(() => request.GetSingle<DepartmentModel, Department>(repository)).Result;

            Assert.Equal("Mathematics", result.Name);
        }

        [Fact]
        public void Get_instructor_list_with_select_new()
        {
            DataRequest request = new DataRequest
            {
                Options = new DataSourceRequestOptions
                {
                    Aggregate = null,
                    Filter = null,
                    Group = null,
                    Page = 1,
                    Sort = null,
                    PageSize = 5
                },
                Includes = null,
                Selects = new Dictionary<string, string> { ["id"] = "id", ["fullName"] = "fullName" },
                Distinct = false
            };

            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            IEnumerable<dynamic> result = Task.Run(() => request.GetDynamicSelect<InstructorModel, Instructor>(repository)).Result;

            Assert.Equal("Roger Zheng", result.First().fullName);
        }

        [Fact]
        public void Get_students_with_filtered_inlude_no_filter_kendo_filter()
        {
            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            ICollection<StudentModel> list = Task.Run
            (
                () => repository.GetItemsAsync<StudentModel, Student>
                (
                    s => s.Enrollments.Count > 0, null, null,
                    new FilteredInclude[]
                    {
                        new FilteredInclude
                        {
                            Include = "enrollments"
                        }
                    }.Select(fi => fi.GetFilteredIncludeExpression(typeof(StudentModel)))
                        .ToArray()
                )
            ).Result;

            Assert.True(list.First().Enrollments.Count > 0);
            Assert.Null(list.First().Enrollments.First().CourseTitle);
        }

        [Fact]
        public void Get_students_with_filtered_inlude_no_filter_select_expand_definition()
        {
            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            ICollection<StudentModel> list = Task.Run
            (
                () => repository.GetAsync<StudentModel, Student>
                (
                    s => s.Enrollments.Count > 0, 
                    null, 
                    new SelectExpandDefinition
                    {
                        ExpandedItems =  new List<SelectExpandItem> { new SelectExpandItem { MemberName = "enrollments" } }
                    }
                )
            ).Result;

            Assert.True(list.First().Enrollments.Count > 0);
        }

        [Fact]
        public void Get_students_no_filtered_inlude_no_filter_select_expand_definition()
        {
            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            ICollection<StudentModel> list = Task.Run
            (
                () => repository.GetAsync<StudentModel, Student>
                (
                    s => s.Enrollments.Count > 0,
                    null,
                    null
                )
            ).Result;

            Assert.Null(list.First().Enrollments);
        }

        [Fact]
        public void Get_students_with_filtered_inlude_with_filter_kendo_filter()
        {
            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            ICollection<StudentModel> list = Task.Run
            (
                () => repository.GetItemsAsync<StudentModel, Student>
                (
                    s => s.Enrollments.Count > 0, null, null,
                    new FilteredInclude[]
                    {
                        new FilteredInclude
                        {
                            Include = "enrollments",
                            Filter = "enrollmentID~eq~-1"
                        }
                    }.Select(fi => fi.GetFilteredIncludeExpression(typeof(StudentModel)))
                        .ToArray()
                )
            ).Result;

            Assert.False(list.First().Enrollments.Any());
        }

        [Fact]
        public void Get_students_with_filtered_inlude_with_filter_select_expand_definition()
        {
            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            ICollection<StudentModel> list = Task.Run
            (
                () => repository.GetAsync<StudentModel, Student>
                (
                    s => s.Enrollments.Count > 0, 
                    null,
                    new SelectExpandDefinition
                    {
                        ExpandedItems = new List<SelectExpandItem> 
                        { 
                            new SelectExpandItem 
                            { 
                                MemberName = "enrollments",
                                Filter = new SelectExpandItemFilter
                                {
                                    FilterBody = new EqualsBinaryDescriptor
                                    (
                                        new MemberSelectorDescriptor("enrollmentID", new ParameterDescriptor("a")),
                                        new ConstantDescriptor(-1)
                                    ),
                                    ParameterName = "a"
                                }
                            }
                        }
                    }
                )
            ).Result;

            Assert.False(list.First().Enrollments.Any());
        }

        [Fact]
        public void Get_students_with_filtered_inlude_no_filter_logicBuilder_filter()
        {
            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            ICollection<StudentModel> list = Task.Run
            (
                () => repository.GetItemsAsync<StudentModel, Student>
                (
                    s => s.Enrollments.Count > 0, null, null,
                    new FilteredInclude[]
                    {
                        new FilteredInclude
                        {
                            Include = "enrollments"
                        }
                    }.Select(fi => fi.BuildFilteredIncludeExpression((typeof(StudentModel))))
                        .ToArray()
                )
            ).Result;

            Assert.True(list.First().Enrollments.Count > 0);
            Assert.Null(list.First().Enrollments.First().CourseTitle);
        }

        [Fact]
        public void Get_students_with_filtered_inlude_no_filter_sorted_select_expand_definition()
        {
            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            ICollection<StudentModel> list = Task.Run
            (
                () => repository.GetAsync<StudentModel, Student>
                (
                    s => s.Enrollments.Count > 0, 
                    null,
                    new SelectExpandDefinition
                    {
                        ExpandedItems = new List<SelectExpandItem>
                        {
                            new SelectExpandItem
                            {
                                MemberName = "enrollments",
                                Filter = new SelectExpandItemFilter
                                {
                                    FilterBody = new GreaterThanBinaryDescriptor
                                    (
                                        new MemberSelectorDescriptor("enrollmentID", new ParameterDescriptor("a")),
                                        new ConstantDescriptor(0)
                                    ),
                                    ParameterName = "a"
                                },
                                QueryFunction = new SelectExpandItemQueryFunction
                                {
                                    MethodCallDescriptor = new OrderByDescriptor
                                    (
                                        new ConstantDescriptor(null, typeof(IEnumerable<EnrollmentModel>)),
                                        new MemberSelectorDescriptor("GradeLetter", new ParameterDescriptor("b")),
                                        ListSortDirection.Ascending,
                                        "b"
                                    )
                                }
                            }
                        }
                    }
                )
            ).Result;

            Assert.True(list.First().Enrollments.Count > 0);
            Assert.True
            (
                string.Compare
                (
                    list.First().Enrollments.First().GradeLetter, 
                    list.Skip(1).First().Enrollments.First().GradeLetter
                ) <= 0
            );
        }

        [Fact]
        public void Get_students_with_filtered_inlude_no_filter_sort_skip_and_take_select_expand_definition()
        {
            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            ICollection<StudentModel> list = Task.Run
            (
                () => repository.GetAsync<StudentModel, Student>
                (
                    s => s.FirstName == "Carson" && s.LastName == "Alexander",
                    null,
                    new SelectExpandDefinition
                    {
                        ExpandedItems = new List<SelectExpandItem>
                        {
                            new SelectExpandItem
                            {
                                MemberName = "enrollments",
                                Filter = new SelectExpandItemFilter
                                {
                                    FilterBody = new GreaterThanBinaryDescriptor
                                    (
                                        new MemberSelectorDescriptor("enrollmentID", new ParameterDescriptor("a")),
                                        new ConstantDescriptor(0)
                                    ),
                                    ParameterName = "a"
                                },
                                QueryFunction = new SelectExpandItemQueryFunction
                                {
                                    MethodCallDescriptor = new TakeDescriptor
                                    (
                                        new SkipDescriptor
                                        (
                                            new OrderByDescriptor
                                            (
                                                new ConstantDescriptor(null, typeof(IEnumerable<EnrollmentModel>)),
                                                new MemberSelectorDescriptor("GradeLetter", new ParameterDescriptor("b")),
                                                ListSortDirection.Descending,
                                                "b"
                                            ),
                                            1
                                        ),
                                        2
                                    )
                                }
                            }
                        }
                    }
                )
            ).Result;

            Assert.Single(list);
            Assert.Equal(2, list.First().Enrollments.Count);
            Assert.Equal("A", list.First().Enrollments.Last().GradeLetter);
        }

        [Fact]
        public void Get_students_with_filtered_inlude_with_filter_logicBuilder_filter()
        {
            ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
            ICollection<StudentModel> list = Task.Run
            (
                () => repository.GetItemsAsync<StudentModel, Student>
                (
                    s => s.Enrollments.Count > 0, null, null,
                    new FilteredInclude[]
                    {
                        new FilteredInclude
                        {
                            Include = "enrollments",
                            FilterGroup = new FilterGroup
                            {
                                Logic = "and",
                                Filters = new List<Filter>
                                {
                                    new Filter
                                    {
                                        Field = "enrollmentID",
                                        Operator = "eq",
                                        Value = -1
                                    }
                                }
                            }
                        }
                    }.Select(fi => fi.BuildFilteredIncludeExpression((typeof(StudentModel))))
                        .ToArray()
                )
            ).Result;

            Assert.False(list.First().Enrollments.Any());
        }
        #endregion Tests

        #region Methods
        static MapperConfiguration MapperConfiguration;
        private void Initialize()
        {
            if (MapperConfiguration == null)
            {
                MapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddExpressionMapping();
                    cfg.AddMaps(typeof(SchoolProfile).GetTypeInfo().Assembly);
                    cfg.AddMaps(typeof(GroupingProfile).GetTypeInfo().Assembly);
                    cfg.AddProfile<ExpressionOperatorsMappingProfile>();
                });
            }

            serviceProvider = new ServiceCollection()
                 .AddDbContext<SchoolContext>
                 (
                     options =>
                     {
                         options.UseInMemoryDatabase("ContosoUniVersity");
                         options.UseInternalServiceProvider(new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider());
                     },
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

            SchoolContext context = serviceProvider.GetRequiredService<SchoolContext>();
            context.Database.EnsureCreated();

            Task.Run(async () => await Seed_Database(serviceProvider.GetRequiredService<ISchoolRepository>())).Wait();
        }
        #endregion Methods

        #region Seed DB
        private static async Task Seed_Database(ISchoolRepository repository)
        {
            if ((await repository.CountAsync<StudentModel, Student>()) > 0)
                return;//database has been seeded

            InstructorModel[] instructors = new InstructorModel[]
            {
                new InstructorModel { FirstName = "Roger",   LastName = "Zheng", HireDate = DateTime.Parse("2004-02-12"), EntityState = LogicBuilder.Domain.EntityStateType.Added },
                new InstructorModel { FirstName = "Kim", LastName = "Abercrombie", HireDate = DateTime.Parse("1995-03-11"), EntityState = LogicBuilder.Domain.EntityStateType.Added},
                new InstructorModel { FirstName = "Fadi", LastName = "Fakhouri", HireDate = DateTime.Parse("2002-07-06"), OfficeAssignment = new OfficeAssignmentModel { Location = "Smith 17" }, EntityState = LogicBuilder.Domain.EntityStateType.Added},
                new InstructorModel { FirstName = "Roger", LastName = "Harui", HireDate = DateTime.Parse("1998-07-01"), OfficeAssignment = new OfficeAssignmentModel { Location = "Gowan 27" }, EntityState = LogicBuilder.Domain.EntityStateType.Added },
                new InstructorModel { FirstName = "Candace", LastName = "Kapoor", HireDate = DateTime.Parse("2001-01-15"), OfficeAssignment = new OfficeAssignmentModel { Location = "Thompson 304" }, EntityState = LogicBuilder.Domain.EntityStateType.Added }
            };
            await repository.SaveGraphsAsync<InstructorModel, Instructor>(instructors);

            DepartmentModel[] departments = new DepartmentModel[]
            {
                new DepartmentModel
                {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    Name = "English",     Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID = instructors.Single(i => i.FirstName == "Kim" && i.LastName == "Abercrombie").ID,
                    Courses =  new HashSet<CourseModel>
                    {
                        new CourseModel {CourseID = 2021, Title = "Composition",    Credits = 3},
                        new CourseModel {CourseID = 2042, Title = "Literature",     Credits = 4}
                    }
                },
                new DepartmentModel
                {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    Name = "Mathematics",
                    Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID = instructors.Single(i => i.FirstName == "Fadi" && i.LastName == "Fakhouri").ID,
                    Courses =  new HashSet<CourseModel>
                    {
                        new CourseModel {CourseID = 1045, Title = "Calculus",       Credits = 4},
                        new CourseModel {CourseID = 3141, Title = "Trigonometry",   Credits = 4}
                    }
                },
                new DepartmentModel
                {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    Name = "Engineering", Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID = instructors.Single(i => i.FirstName == "Roger" && i.LastName == "Harui").ID,
                    Courses =  new HashSet<CourseModel>
                    {
                        new CourseModel {CourseID = 1050, Title = "Chemistry",      Credits = 3}
                    }
                },
                new DepartmentModel
                {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    Name = "Economics",
                    Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID = instructors.Single(i => i.FirstName == "Candace" && i.LastName == "Kapoor").ID,
                    Courses =  new HashSet<CourseModel>
                    {
                        new CourseModel {CourseID = 4022, Title = "Microeconomics", Credits = 3},
                        new CourseModel {CourseID = 4041, Title = "Macroeconomics", Credits = 3 }
                    }
                }
            };
            await repository.SaveGraphsAsync<DepartmentModel, Department>(departments);

            IEnumerable<CourseModel> courses = departments.SelectMany(d => d.Courses);
            CourseAssignmentModel[] courseInstructors = new CourseAssignmentModel[]
            {
                new CourseAssignmentModel {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID,
                    InstructorID = instructors.Single(i => i.LastName == "Kapoor").ID
                    },
                new CourseAssignmentModel {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID,
                    InstructorID = instructors.Single(i => i.LastName == "Harui").ID
                    },
                new CourseAssignmentModel {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    CourseID = courses.Single(c => c.Title == "Microeconomics" ).CourseID,
                    InstructorID = instructors.Single(i => i.LastName == "Zheng").ID
                    },
                new CourseAssignmentModel {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    CourseID = courses.Single(c => c.Title == "Macroeconomics" ).CourseID,
                    InstructorID = instructors.Single(i => i.LastName == "Zheng").ID
                    },
                new CourseAssignmentModel {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    CourseID = courses.Single(c => c.Title == "Calculus" ).CourseID,
                    InstructorID = instructors.Single(i => i.LastName == "Fakhouri").ID
                    },
                new CourseAssignmentModel {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    CourseID = courses.Single(c => c.Title == "Trigonometry" ).CourseID,
                    InstructorID = instructors.Single(i => i.LastName == "Harui").ID
                    },
                new CourseAssignmentModel {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    CourseID = courses.Single(c => c.Title == "Composition" ).CourseID,
                    InstructorID = instructors.Single(i => i.LastName == "Abercrombie").ID
                    },
                new CourseAssignmentModel {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    CourseID = courses.Single(c => c.Title == "Literature" ).CourseID,
                    InstructorID = instructors.Single(i => i.LastName == "Abercrombie").ID
                    },
            };
            await repository.SaveGraphsAsync<CourseAssignmentModel, CourseAssignment>(courseInstructors);

            StudentModel[] students = new StudentModel[]
            {
                new StudentModel
                {
                    EntityState =  LogicBuilder.Domain.EntityStateType.Added,
                    FirstName = "Carson",   LastName = "Alexander",
                    EnrollmentDate = DateTime.Parse("2010-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID,
                            Grade = Contoso.Domain.Entities.Grade.A
                        },
                        new EnrollmentModel
                        {
                            CourseID = courses.Single(c => c.Title == "Microeconomics" ).CourseID,
                            Grade = Contoso.Domain.Entities.Grade.C
                        },
                        new EnrollmentModel
                        {
                            CourseID = courses.Single(c => c.Title == "Macroeconomics" ).CourseID,
                            Grade = Contoso.Domain.Entities.Grade.B
                        }
                    }
                },
                new StudentModel
                {
                    EntityState =  LogicBuilder.Domain.EntityStateType.Added,
                    FirstName = "Meredith", LastName = "Alonso",
                    EnrollmentDate = DateTime.Parse("2012-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = courses.Single(c => c.Title == "Calculus" ).CourseID,
                            Grade = Contoso.Domain.Entities.Grade.B
                        },
                        new EnrollmentModel
                        {
                            CourseID = courses.Single(c => c.Title == "Trigonometry" ).CourseID,
                            Grade = Contoso.Domain.Entities.Grade.B
                        },
                        new EnrollmentModel
                        {
                            CourseID = courses.Single(c => c.Title == "Composition" ).CourseID,
                            Grade = Contoso.Domain.Entities.Grade.B
                        }
                    }
                },
                new StudentModel
                {
                    EntityState =  LogicBuilder.Domain.EntityStateType.Added,
                    FirstName = "Arturo",   LastName = "Anand",
                    EnrollmentDate = DateTime.Parse("2013-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID
                        },
                        new EnrollmentModel
                        {
                            CourseID = courses.Single(c => c.Title == "Microeconomics").CourseID,
                            Grade = Contoso.Domain.Entities.Grade.B
                        },
                    }
                },
                new StudentModel
                {
                    EntityState =  LogicBuilder.Domain.EntityStateType.Added,
                    FirstName = "Gytis",    LastName = "Barzdukas",
                    EnrollmentDate = DateTime.Parse("2012-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = courses.Single(c => c.Title == "Chemistry").CourseID,
                            Grade = Contoso.Domain.Entities.Grade.B
                        }
                    }
                },
                new StudentModel
                {
                    EntityState =  LogicBuilder.Domain.EntityStateType.Added,
                    FirstName = "Yan",      LastName = "Li",
                    EnrollmentDate = DateTime.Parse("2012-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = courses.Single(c => c.Title == "Composition").CourseID,
                            Grade = Contoso.Domain.Entities.Grade.B
                        }
                    }
                },
                new StudentModel
                {
                    EntityState =  LogicBuilder.Domain.EntityStateType.Added,
                    FirstName = "Peggy",    LastName = "Justice",
                    EnrollmentDate = DateTime.Parse("2011-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = courses.Single(c => c.Title == "Literature").CourseID,
                            Grade = Contoso.Domain.Entities.Grade.B
                        }
                    }
                },
                new StudentModel
                {
                    EntityState =  LogicBuilder.Domain.EntityStateType.Added,
                    FirstName = "Laura",    LastName = "Norman",
                    EnrollmentDate = DateTime.Parse("2013-09-01")
                },
                new StudentModel
                {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    FirstName = "Nino",     LastName = "Olivetto",
                    EnrollmentDate = DateTime.Parse("2005-09-01")
                },
                new StudentModel
                {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    FirstName = "Tom",
                    LastName = "Spratt",
                    EnrollmentDate = DateTime.Parse("2010-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = 1045,
                            Grade = Contoso.Domain.Entities.Grade.B
                        }
                    }
                },
                new StudentModel
                {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    FirstName = "Billie",
                    LastName = "Spratt",
                    EnrollmentDate = DateTime.Parse("2010-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = 1050,
                            Grade = Contoso.Domain.Entities.Grade.B
                        }
                    }
                },
                new StudentModel
                {
                    EntityState = LogicBuilder.Domain.EntityStateType.Added,
                    FirstName = "Jackson",
                    LastName = "Spratt",
                    EnrollmentDate = DateTime.Parse("2017-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = 2021,
                            Grade = Contoso.Domain.Entities.Grade.B
                        }
                    }
                }
            };

            await repository.SaveGraphsAsync<StudentModel, Student>(students);
        }
        #endregion Seed DB
    }
}
