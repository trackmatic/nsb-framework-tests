using System.Threading.Tasks;
using DomainFramework;
using OurDomain.DomainEvents;

namespace Server.DomainHandlers
{
    public class MyCustomDomainEventHandler : IDomainEventHandler<MyObjectCreatedDomainEvent>
    {
        private readonly IAsyncSessionProvider _session;

        public MyCustomDomainEventHandler(IAsyncSessionProvider session)
        {
            _session = session;
        }
        
        public async Task Handle(MyObjectCreatedDomainEvent @event)
        {
            await _session.Instance.StoreAsync(@event.MyObject).ConfigureAwait(false);
        }
    }
}
