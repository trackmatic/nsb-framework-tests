using System.Threading.Tasks;

namespace DomainFramework
{
    public interface IDomainPubisher
    {
        void RegisterHandler<T>();
        Task Publish<T>(T @event) where T : IDomainEvent;
    }
}