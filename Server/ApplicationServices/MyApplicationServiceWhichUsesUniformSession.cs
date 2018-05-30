using System.Threading.Tasks;
using Commands;
using NServiceBus.UniformSession;

namespace Server.ApplicationServices
{
    public class MyApplicationServiceWhichUsesUniformSession
    {
        private readonly IUniformSession _session;

        public MyApplicationServiceWhichUsesUniformSession(IUniformSession session)
        {
            _session = session;
        }

        public Task DoSomethingElse()
        {
            return _session.Send(new DoSomethingElseCommand());
        }
    }
}
