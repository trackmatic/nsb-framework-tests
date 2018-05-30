using System.Threading.Tasks;

namespace DomainFramework
{
    public interface IDomainEventHandler<in T>
    {
        Task Handle(T @event);
    }
}
