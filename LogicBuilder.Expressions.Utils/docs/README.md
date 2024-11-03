## Build LINQ Expressions Dynamically
Create complex expressions from single descriptor objects.

To use:
```c#
        public void BuildFilter()
        {
            IConfigurationProvider config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ExpressionOperatorsMappingProfile>();
            });
            IMapper mapper = config.CreateMapper();

            Expression<Func<Product, bool>> filter = GetFilterExpression<Product>
            (
                //$it => $it.AlternateAddresses.Any(address => (address.City == "Redmond"))
                new FilterLambdaDescriptor
                (
                    new AnyDescriptor
                    (
                        new MemberSelectorDescriptor("AlternateAddresses", new ParameterDescriptor("$it")),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("City", new ParameterDescriptor("address")),
                            new ConstantDescriptor("Redmond")
                        ),
                        "address"
                    ),
                    typeof(Product),
                    "$it"
                )
            );

            Expression<Func<T, bool>> GetFilterExpression<T>(FilterLambdaDescriptor descriptor)
                => (Expression<Func<T, bool>>)mapper.Map<FilterLambdaOperator>
                (
                    descriptor,
                    opts => opts.Items["parameters"] = new Dictionary<string, ParameterExpression>()
                ).Build();
        }

        public void BuildSelector()
        {
            IConfigurationProvider config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ExpressionOperatorsMappingProfile>();
            });
            IMapper mapper = config.CreateMapper();

            Expression<Func<IQueryable<Category>, Category>> selector = GetSelectorExpression<IQueryable<Category>, Category>
            (
                //$it => $it.FirstOrDefault(a => (a.CategoryID == -1))
                new SelectorLambdaDescriptor
                (
                    new FirstOrDefaultDescriptor
                    (
                        new ParameterDescriptor("$it"),
                        new EqualsBinaryDescriptor
                        (
                            new MemberSelectorDescriptor("CategoryID", new ParameterDescriptor("a")),
                            new ConstantDescriptor(-1)
                        ),
                        "a"
                    ),
                    typeof(IQueryable<Category>),
                    typeof(Category),
                    "$it"
                )
            );
            
            Expression<Func<T, TResult>> GetSelectorExpression<T, TResult>(SelectorLambdaDescriptor descriptor)
                => (Expression<Func<T, TResult>>)mapper.Map<SelectorLambdaOperator>
                (
                    descriptor,
                    opts => opts.Items["parameters"] = new Dictionary<string, ParameterExpression>()
                ).Build();
        }
```

`ExpressionOperatorsMappingProfile` is a mapping profile from `LogicBuilder.EntityFrameworkCore.SqlServer`.

For serializable descriptors, replace `cfg.AddProfile<ExpressionOperatorsMappingProfile>();` with a [custom mapping profile](https://github.com/BpsLogicBuilder/LogicBuilder.Samples/blob/master/Xamarin/Contoso/Contoso.AutoMapperProfiles/DescriptorToOperatorMappingProfile.cs) with serializable descriptors.