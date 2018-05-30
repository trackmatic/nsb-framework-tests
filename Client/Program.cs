using System;
using System.Linq;
using Commands;
using NServiceBus;
using NServiceBus.InMemory.Outbox;
using Raven.Client.Document;
using Unity;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = new DocumentStore { EnlistInDistributedTransactions = false, Url = "http://localhost:8092", DefaultDatabase = "test" };

            store.Initialize();

            var configuration = new EndpointConfiguration("test-client");

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

            var container = new UnityContainer();
            configuration.UseContainer<UnityBuilder>(customisations => { customisations.UseExistingContainer(container); });


            var conventions = configuration.Conventions();
            conventions.DefiningEventsAs(type => IsMessage(type, "Events"));
            conventions.DefiningCommandsAs(type => IsMessage(type, "Commands"));

            configuration.EnableInstallers();

            configuration.EnableUniformSession();

            var endpoint = Endpoint.Start(configuration).Result;

            Console.WriteLine("Running client");
            Console.WriteLine("Press any key to send a command");

            while (true)
            {
                Console.ReadLine();
                endpoint.Send(new DoSomethingCommand()).Wait();
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
