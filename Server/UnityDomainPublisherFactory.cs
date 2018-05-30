using DomainFramework;
using Unity;

namespace Server
{
    public class UnityDomainPublisherFactory : IDomainPublisherFactory
    {
        private readonly IUnityContainer _container;

        public UnityDomainPublisherFactory(IUnityContainer container)
        {
            _container = container;
        }

        public IDomainPubisher Create()
        {
            return new UnityDomainPublisher(_container);
        }
    }
}
