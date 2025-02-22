using System;
using System.Threading.Tasks;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Snapshooter.Xunit;
using Squadron;
using Xunit;

namespace HotChocolate.Types.Filters;

[Obsolete]
public class MongoFilterTests : IClassFixture<MongoResource>
{
    private readonly MongoResource _mongoResource;

    public MongoFilterTests(MongoResource mongoResource)
    {
        _mongoResource = mongoResource;
    }

    [Fact]
    public async Task GetItems_NoFilter_AllItems_Are_Returned()
    {
        // arrange
        IServiceProvider services = new ServiceCollection()
            .AddSingleton(_ =>
            {
                var database = _mongoResource.CreateDatabase();
                var collection = database.GetCollection<Model>("col");
                collection.InsertMany(new[]
                {
                    new Model { Foo = "abc", Bar = 1, Baz = true },
                    new Model { Foo = "def", Bar = 2, Baz = false },
                });
                return collection;
            })
            .AddGraphQL()
            .AddQueryType<QueryType>()
            .Services
            .BuildServiceProvider();

        var executor =
            await services.GetRequiredService<IRequestExecutorResolver>()
                .GetRequestExecutorAsync();

        var request = QueryRequestBuilder.New()
            .SetQuery("{ items { foo } }")
            .Create();

        // act
        var result = await executor.ExecuteAsync(request);

        // assert
        result.ToJson().MatchSnapshot();
    }

    [Fact]
    public async Task GetItems_EqualsFilter_FirstItems_Is_Returned()
    {
        // arrange
        IServiceProvider services = new ServiceCollection()
            .AddSingleton(_ =>
            {
                var database = _mongoResource.CreateDatabase();
                var collection = database.GetCollection<Model>("col");
                collection.InsertMany(new[]
                {
                    new Model { Foo = "abc", Bar = 1, Baz = true },
                    new Model { Foo = "def", Bar = 2, Baz = false },
                });
                return collection;
            })
            .AddGraphQL()
            .AddQueryType<QueryType>()
            .Services
            .BuildServiceProvider();

        var executor =
            await services.GetRequiredService<IRequestExecutorResolver>()
                .GetRequestExecutorAsync();

        var request = QueryRequestBuilder.New()
            .SetQuery("{ items(where: { foo: \"abc\" }) { foo } }")
            .Create();

        // act
        var result = await executor.ExecuteAsync(request);

        // assert
        result.ToJson().MatchSnapshot();
    }

    [Fact]
    public async Task GetItems_ObjectEqualsFilter_FirstItems_Is_Returned()
    {
        // arrange
        IServiceProvider services = new ServiceCollection()
            .AddSingleton(_ =>
            {
                var database = _mongoResource.CreateDatabase();

                var collection = database.GetCollection<Model>("col");
                collection.InsertMany(new[]
                {
                    new Model
                    {
                        Nested = null
                    },
                    new Model
                    {
                        Nested = new Model
                        {
                            Nested = new Model
                            {
                                Foo = "abc",
                                Bar = 1,
                                Baz = true
                            }
                        }
                    },
                    new Model
                    {
                        Nested = new Model
                        {
                            Nested= new Model
                            {
                                Foo = "def",
                                Bar = 2,
                                Baz = false
                            }
                        }
                    },
                });
                return collection;
            })
            .AddGraphQL()
            .AddQueryType<QueryType>()
            .Services
            .BuildServiceProvider();

        var executor =
            await services.GetRequiredService<IRequestExecutorResolver>()
                .GetRequestExecutorAsync();

        var request = QueryRequestBuilder.New()
            .SetQuery(
                "{ items(where: { nested:{ nested: { foo: \"abc\" " +
                "} } }) { nested { nested { foo } } } }")
            .Create();

        // act
        var result = await executor.ExecuteAsync(request);

        // assert
        result.ToJson().MatchSnapshot();
    }

    [Fact]
    public async Task GetItems_With_Paging_EqualsFilter_FirstItems_Is_Returned()
    {
        // arrange
        IServiceProvider services = new ServiceCollection()
            .AddSingleton(_ =>
            {
                var database = _mongoResource.CreateDatabase();

                var collection = database.GetCollection<Model>("col");
                collection.InsertMany(new[]
                {
                    new Model { Foo = "abc", Bar = 1, Baz = true },
                    new Model { Foo = "def", Bar = 2, Baz = false },
                });
                return collection;
            })
            .AddGraphQL()
            .AddQueryType<QueryType>()
            .Services
            .BuildServiceProvider();

        var executor =
            await services.GetRequiredService<IRequestExecutorResolver>()
                .GetRequestExecutorAsync();

        var request = QueryRequestBuilder.New()
            .SetQuery("{ paging(where: { foo: \"abc\" }) { nodes { foo } } }")
            .Create();

        // act
        var result = await executor.ExecuteAsync(request);

        // assert
        result.ToJson().MatchSnapshot();
    }

    [Fact]
    public async Task Boolean_Filter_Equals()
    {
        // arrange
        IServiceProvider services = new ServiceCollection()
            .AddSingleton(_ =>
            {
                var database = _mongoResource.CreateDatabase();
                var collection = database.GetCollection<Model>("col");
                collection.InsertMany(new[]
                {
                    new Model { Foo = "abc", Bar = 1, Baz = true },
                    new Model { Foo = "def", Bar = 2, Baz = false },
                });
                return collection;
            })
            .AddGraphQL()
            .AddQueryType<QueryType>()
            .Services
            .BuildServiceProvider();

        var executor =
            await services.GetRequiredService<IRequestExecutorResolver>()
                .GetRequestExecutorAsync();

        var request = QueryRequestBuilder.New()
            .SetQuery("{ paging(where: { baz: true }) { nodes { foo } } }")
            .Create();

        // act
        var result = await executor.ExecuteAsync(request);

        // assert
        result.ToJson().MatchSnapshot();
    }

    [Fact]
    public async Task Boolean_Filter_Not_Equals()
    {
        // arrange
        IServiceProvider services = new ServiceCollection()
            .AddSingleton(_ =>
            {
                var database = _mongoResource.CreateDatabase();
                var collection = database.GetCollection<Model>("col");
                collection.InsertMany(new[]
                {
                    new Model { Foo = "abc", Bar = 1, Baz = true },
                    new Model { Foo = "def", Bar = 2, Baz = false },
                });
                return collection;
            })
            .AddGraphQL()
            .AddQueryType<QueryType>()
            .Services
            .BuildServiceProvider();

        var executor =
            await services.GetRequiredService<IRequestExecutorResolver>()
                .GetRequestExecutorAsync();

        var request = QueryRequestBuilder.New()
            .SetQuery("{ paging(where: { baz_not: false }) { nodes { foo } } }")
            .Create();

        // act
        var result = await executor.ExecuteAsync(request);

        // assert
        result.ToJson().MatchSnapshot();
    }

    [Fact]
    public async Task DateTimeType_GreaterThan_Filter()
    {
        // arrange
        IServiceProvider services = new ServiceCollection()
            .AddSingleton(_ =>
            {
                var database = _mongoResource.CreateDatabase();
                var collection = database.GetCollection<Model>("col");
                collection.InsertMany(new[]
                {
                    new Model { Time = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc) },
                    new Model { Time = new DateTime(2016, 1, 1, 1, 1, 1, DateTimeKind.Utc) },
                });
                return collection;
            })
            .AddGraphQL()
            .AddQueryType<QueryType>()
            .Services
            .BuildServiceProvider();

        var executor =
            await services.GetRequiredService<IRequestExecutorResolver>()
                .GetRequestExecutorAsync();

        var request = QueryRequestBuilder.New()
            .SetQuery("{ items(where: { time_gt: \"2001-01-01\" }) { time } }")
            .Create();

        // act
        var result = await executor.ExecuteAsync(request);

        // assert
        result.ToJson().MatchSnapshot();
    }

    [Fact]
    public async Task DateType_GreaterThan_Filter()
    {
        // arrange
        IServiceProvider services = new ServiceCollection()
            .AddSingleton(_ =>
            {
                var database = _mongoResource.CreateDatabase();
                var collection = database.GetCollection<Model>("col");
                collection.InsertMany(new[]
                {
                    new Model { Date = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc).Date },
                    new Model { Date = new DateTime(2016, 1, 1, 1, 1, 1, DateTimeKind.Utc).Date },
                });
                return collection;
            })
            .AddGraphQL()
            .AddQueryType<QueryType>()
            .Services
            .BuildServiceProvider();

        var executor =
            await services.GetRequiredService<IRequestExecutorResolver>()
                .GetRequestExecutorAsync();

        var request = QueryRequestBuilder.New()
            .SetQuery("{ items(where: { date_gt: \"2001-01-01\" }) { date } }")
            .Create();

        // act
        var result = await executor.ExecuteAsync(request);

        // assert
        result.ToJson().MatchSnapshot();
    }

    public class QueryType : ObjectType
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Name("Query");
            descriptor.Field("items")
                .Type<ListType<ModelType>>()
                .UseFiltering<FilterInputType<Model>>()
                .Resolve(ctx => ctx.Service<IMongoCollection<Model>>().AsQueryable());

            descriptor.Field("paging")
                .UsePaging<ModelType>()
                .UseFiltering<FilterInputType<Model>>()
                .Resolve(ctx => ctx.Service<IMongoCollection<Model>>().AsQueryable());
        }
    }

    public class ModelType : ObjectType<Model>
    {
        protected override void Configure(
            IObjectTypeDescriptor<Model> descriptor)
        {
            descriptor.Field(t => t.Id)
                .Type<IdType>()
                .Resolve(c => c.Parent<Model>().Id);

            descriptor.Field(t => t.Time)
                .Type<NonNullType<DateTimeType>>();

            descriptor.Field(t => t.Date)
                .Type<NonNullType<DateType>>();
        }
    }

    public class Model
    {
        public ObjectId Id { get; set; }

        public string Foo { get; set; }

        public int Bar { get; set; }

        public bool Baz { get; set; }

        public Model Nested { get; set; }

        [GraphQLType(typeof(NonNullType<DateTimeType>))]
        public DateTime Time { get; set; }

        [GraphQLType(typeof(NonNullType<DateType>))]
        public DateTime Date { get; set; }
    }
}
