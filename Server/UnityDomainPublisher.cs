using System.Threading.Tasks;
using DomainFramework;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Server
{
    public class UnityDomainPublisher : IDomainPubisher
    {
        private readonly IUnityContainer _container;

        public UnityDomainPublisher(IUnityContainer container)
        {
            _container = container;
        }

        public void RegisterHandler<T>()
        {
            var domainEventHandlerType = typeof(T);

            _container.RegisterType(domainEventHandlerType, new HierarchicalLifetimeManager());

            var interfaces = domainEventHandlerType.GetInterfaces();

            foreach (var i in interfaces)
            {
                if (i.IsGenericType)
                {
                    var genericType = i.GetGenericTypeDefinition();

                    if (genericType == typeof(IDomainEventHandler<>))
                    {
                        _container.RegisterType(i, domainEventHandlerType.FullName,
                            new InjectionFactory(c => c.Resolve(domainEventHandlerType)));
                    }
                }
            }
        }

        public async Task Publish<T>(T @event) where T : IDomainEvent
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());
            var handlers = _container.ResolveAll(handlerType);
            dynamic typedEvent = @event;
            foreach (var temp in handlers)
            {
                dynamic handler = temp;
                await handler.Handle(typedEvent);
            }
        }
    }
}