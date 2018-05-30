using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commands;
using DomainFramework;
using NServiceBus;
using NServiceBus.InMemory.Outbox;
using NServiceBus.Pipeline;
using Raven.Client.Document;
using Server.DomainHandlers;
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

            routing.RouteToEndpoint(typeof(CreateMyObjectCommand).Assembly, "test-server");

            var outbox = configuration.EnableOutbox();
            outbox.TimeToKeepDeduplicationData(TimeSpan.FromDays(2));

            configuration.UsePersistence<RavenDBPersistence>()
                .DisableSubscriptionVersioning()
                .SetDefaultDocumentStore(store)
                .DoNotSetupDatabasePermissions();

            var container = CreateContainer();
            configuration.UseContainer<UnityBuilder>(customisations => { customisations.UseExistingContainer(container); });

            var conventions = configuration.Conventions();
            conventions.DefiningEventsAs(type => IsMessage(type, "Events"));
            conventions.DefiningCommandsAs(type => IsMessage(type, "Commands"));

            configuration.EnableInstallers();
            configuration.EnableUniformSession();

            // Registers the shared session behaviour which attempts to inject the IAsynDocumentSession
            // into the session provider when the document session instance is available
            var pipeline = configuration.Pipeline;
            pipeline.Register(typeof(SharedSessionBehavior), "RavenDB Session Provider");

            var endpoint = Endpoint.Start(configuration).Result;

            Console.WriteLine("Running server");
            Console.ReadLine();

            endpoint.Stop().Wait();
        }

        static IUnityContainer CreateContainer()
        {
            var container = new UnityContainer();
            DomainPublisher.Factory = new UnityDomainPublisherFactory(container);
            var domainHandlerFactory = new UnityDomainPublisher(container);
            domainHandlerFactory.RegisterHandler<MyCustomDomainEventHandler>();
            container.RegisterType<IDomainPubisher>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => domainHandlerFactory));
            container.RegisterType<IAsyncSessionProvider>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => new AsyncSessionProvider()));
            return container;
        }

        public class SharedSessionBehavior : Behavior<IInvokeHandlerContext>
        {
            public override Task Invoke(IInvokeHandlerContext context, Func<Task> next)
            {
                context.Builder.Build<IAsyncSessionProvider>().Set(context.SynchronizedStorageSession.RavenSession());
                return next();
            }
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
