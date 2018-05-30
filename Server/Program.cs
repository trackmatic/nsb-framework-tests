using System;
using System.Linq;
using System.Threading.Tasks;
using Commands;
using NServiceBus;
using NServiceBus.InMemory.Outbox;
using NServiceBus.Pipeline;
using Raven.Client.Document;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = new DocumentStore { EnlistInDistributedTransactions = false, Url = "http://localhost:8092", DefaultDatabase = "test" };

            store.Initialize();

            var configuration = new EndpointConfiguration("test-server");

            configuration.UseSerialization<NewtonsoftSerializer>();

            var routing = configuration.UseTransport<RabbitMQTransport>()
                .UseConventionalRoutingTopology()
                .ConnectionString("host=localhost;username=guest;password=guest;virtualHost=/;")
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .Routing();

            routing.RouteToEndpoint(typeof(DoSomethingCommand).Assembly, "test-server");

            var outbox = configuration.EnableOutbox();
            outbox.TimeToKeepDeduplicationData(TimeSpan.FromDays(2));

            configuration.UsePersistence<RavenDBPersistence>()
                .DisableSubscriptionVersioning()
                .SetDefaultDocumentStore(store)
                .DoNotSetupDatabasePermissions();


            var conventions = configuration.Conventions();
            conventions.DefiningEventsAs(type => IsMessage(type, "Events"));
            conventions.DefiningCommandsAs(type => IsMessage(type, "Commands"));

            configuration.EnableInstallers();
            configuration.EnableUniformSession();

            // Registers the shared session behaviour which attempts to inject the IAsynDocumentSession
            // into the session provider when the document session instance is available
            var pipeline = configuration.Pipeline;
            pipeline.Register(typeof(MyCustomBehavior), "My Custom Behavior");

            // ******************************************
            // Registering the component using the configuration works
            // ******************************************

            //configuration.RegisterComponents(x => x.ConfigureComponent<IMyCustomDepedency>(() => new MyCustomDepdency(), DependencyLifecycle.InstancePerUnitOfWork));

            // ******************************************
            // When registering the via the unit container the behavior is not able to resolve the dependency
            // ******************************************
            var container = new UnityContainer();
            container.RegisterType<IMyCustomDepedency>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => new MyCustomDepdency()));


            configuration.UseContainer<UnityBuilder>(customisations => { customisations.UseExistingContainer(container); });

            var endpoint = Endpoint.Start(configuration).Result;

            Console.WriteLine("Running server");
            Console.ReadLine();

            endpoint.Stop().Wait();
        }

        private static bool IsMessage(Type type, params string[] check)
        {
            if (string.IsNullOrEmpty(type.Namespace))
            {
                return false;
            }

            return check.Any(x => type.Namespace.Contains(x));
        }
    }
}
