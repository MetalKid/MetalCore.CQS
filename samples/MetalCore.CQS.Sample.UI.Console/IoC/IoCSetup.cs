using CacheManager.Core;
using MetalCore.CQS.Command;
using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.DateTimes;
using MetalCore.CQS.Mapper;
using MetalCore.CQS.Mediators;
using MetalCore.CQS.Query;
using MetalCore.CQS.Repository;
using MetalCore.CQS.Sample.Core;
using MetalCore.CQS.Sample.Core.Cache;
using MetalCore.CQS.Sample.Core.UserContext;
using MetalCore.CQS.Sample.Core.DataStore;
using MetalCore.CQS.Sample.Core.Decorators;
using MetalCore.CQS.Sample.UI.Console.UserContext;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Reflection;
using MetalCore.CQS.PubSub;
using System.Linq;

namespace MetalCore.CQS.Sample.UI.Console.IoC
{
    public static class IoCSetup
    {
        public static Container SetupIoC()
        {
            Container container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            container.RegisterSingleton<IUserContext>(() => new MyUserContext { Language = "EN", UserName = "jane.doe" });

            container.Register<ICarDataStore, CarDataStore>(Lifestyle.Singleton);
            container.RegisterSingleton<IDateTimeProvider, DateTimeProvider>();
            container.RegisterSingleton<ICqsMediator>(() => new CqsMediator(type => container.GetInstance(type)));
            container.RegisterSingleton<IRepositoryMediator>(() => new RepositoryMediator(type => container.GetInstance(type)));
            container.RegisterSingleton<IMapperMediator>(() => new MapperMediator(type => container.GetInstance(type)));

            container.RegisterSingleton<IPublisher>(() => new Publisher(type => container.GetAllInstances(type).Cast<dynamic>().ToList()));

            container.Register<IQueryCacheRegion, MyQueryCacheRegion>(Lifestyle.Scoped);
            container.RegisterSingleton(typeof(ICacheManager<object>),
                () => CacheFactory.Build<object>(config => config.WithMicrosoftMemoryCacheHandle(true)));

            Assembly[] assembliesToScan = GetAssembliesToScan();

            RegisterMappers(container, assembliesToScan);
            RegisterQueries(container, assembliesToScan);
            ReigsterCommands(container, assembliesToScan);
            ReigsterCommandQueries(container, assembliesToScan);
            RegisterRepositories(container, assembliesToScan);

            container.Collection.Register(typeof(ISubscriber<>), assembliesToScan);

            container.Verify();

            return container;
        }

        private static void RegisterMappers(Container container, Assembly[] assemblies)
        {
            //NOTE: Only need to register this once and all concrete classes will be auto inclded now and 
            //      in the future.
            container.Register(typeof(IMapper<,>), assemblies);
        }

        private static void RegisterQueries(Container container, Assembly[] assemblies)
        {
            //NOTE: Only need to register this once and all concrete classes will be auto inclded now and 
            //      in the future.
            container.Register(typeof(IQueryHandler<,>), assemblies, Lifestyle.Scoped);

            // Register interfaces that can be injected into other decorators
            container.Collection.Register(typeof(IQueryLogger<,>), assemblies);
            container.Collection.Register(typeof(IQueryPermission<,>), assemblies);

            // Order matters - First one declared is last one run
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(MyQueryHandlerCacheDecorator<,>));
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(QueryHandlerPermissionDecorator<,>));
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(MyQueryHandlerTimingDecorator<,>));
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(QueryHandlerLoggerDecorator<,>));
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(MyQueryHandlerExceptionDecorator<,>));
        }

        private static void ReigsterCommands(Container container, Assembly[] assemblies)
        {
            //NOTE: Only need to register this once and all concrete classes will be auto inclded now and 
            //      in the future.
            container.Register(typeof(ICommandHandler<>), assemblies, Lifestyle.Scoped);

            // Register interfaces that can be injected into other decorators
            container.Collection.Register(typeof(ICommandLogger<>), assemblies);
            container.Collection.Register(typeof(ICommandCacheInvalidation<>), assemblies);
            container.Collection.Register(typeof(ICommandValidator<>), assemblies);
            container.Collection.Register(typeof(ICommandPermission<>), assemblies);

            // Order matters - First one declared is last one run
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerRetryDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerCacheInvalidationDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerValidatorDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerPermissionDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(MyCommandHandlerTimingDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerLoggerDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(MyCommandHandlerExceptionDecorator<>));
        }

        private static void ReigsterCommandQueries(Container container, Assembly[] assemblies)
        {
            //NOTE: Only need to register this once and all concrete classes will be auto inclded now and 
            //      in the future.
            container.Register(typeof(ICommandQueryHandler<,>), assemblies, Lifestyle.Scoped);

            // Register interfaces that can be injected into other decorators
            container.Collection.Register(typeof(ICommandQueryLogger<,>), assemblies);
            container.Collection.Register(typeof(ICommandQueryCacheInvalidation<,>), assemblies);
            container.Collection.Register(typeof(ICommandQueryValidator<,>), assemblies);
            container.Collection.Register(typeof(ICommandQueryPermission<,>), assemblies);

            // Order matters - First one declared is last one run
            container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(CommandQueryHandlerRetryDecorator<,>));
            container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(CommandQueryHandlerCacheInvalidationDecorator<,>));
            container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(CommandQueryHandlerValidatorDecorator<,>));
            container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(CommandQueryHandlerPermissionDecorator<,>));
            container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(MyCommandQueryHandlerTimingDecorator<,>));
            container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(CommandQueryHandlerLoggerDecorator<,>));
            container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(MyCommandQueryHandlerExceptionDecorator<,>));
        }

        private static void RegisterRepositories(Container container, Assembly[] assemblies)
        {
            container.Register(typeof(IRepository<>), assemblies);
            container.Register(typeof(IRepository<,>), assemblies);
            container.Register(typeof(IValidationRepository<>), assemblies);
        }

        private static Assembly[] GetAssembliesToScan() =>
            new[] 
            { 
                typeof(IoCSetup).GetTypeInfo().Assembly, 
                typeof(IoCReference).GetTypeInfo().Assembly  
            };
    }
}
