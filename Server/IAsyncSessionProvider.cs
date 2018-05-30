using Raven.Client;

namespace Server
{
    public interface IAsyncSessionProvider
    {
        IAsyncDocumentSession Instance { get; }
        void Set(IAsyncDocumentSession session);
    }
}
