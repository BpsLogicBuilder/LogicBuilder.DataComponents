## LogicBuilder.Kendo.ExpressionExtensions
LogicBuilder.Kendo.ExpressionExtensions depends on Telerik.UI.for.AspNet.Core but has not been created or maintained by Telerik/Progress.  It contains extension methods for creating IQueryable expressions given an instance of Telerik's DataSourceRequest class.

First implement the context, store, repository and service registrations as in [LogicBuilder.EntityFrameworkCore.SqlServer](https://github.com/BpsLogicBuilder/LogicBuilder.DataComponents).

```c#
    //Use the DataSourceRequest helper to get the DataSourceResult.
    ISchoolRepository repository = serviceProvider.GetRequiredService<ISchoolRepository>();
    DataSourceResult result = await request.GetData<StudentModel, Student>(repository);

    public static async Task<DataSourceResult> GetData<TModel, TData>(this DataRequest request, IContextRepository contextRepository)
        where TModel : BaseModelClass
        where TData : BaseDataClass
    {
        return await request.Options.CreateDataSourceRequest().GetDataSourceResult<TModel, TData>
        (
            contextRepository,
            request.SelectExpandDefinition,
            request.Includes.BuildIncludesExpressionCollection<TModel>()
        );
    }

```

Refer to [the data request tests](https://github.com/BpsLogicBuilder/LogicBuilder.DataComponents/blob/master/LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests/DataRequestTests.cs) for complete examples.