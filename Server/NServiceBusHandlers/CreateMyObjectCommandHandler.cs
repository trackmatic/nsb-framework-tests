using System.Threading.Tasks;
using Commands;
using NServiceBus;
using OurDomain.Model;
using Server.ApplicationServices;

namespace Server.NServiceBusHandlers
{
    public class CreateMyObjectCommandHandler : IHandleMessages<CreateMyObjectCommand>
    {
        private readonly MyApplicationServiceWhichUsesUniformSession _service;
        private readonly IAsyncSessionProvider _session;

        public CreateMyObjectCommandHandler(MyApplicationServiceWhichUsesUniformSession service, IAsyncSessionProvider session)
        {
            _service = service;
            _session = session;
        }

        public async Task Handle(CreateMyObjectCommand message, IMessageHandlerContext context)
        {
            await MyObject.Create(message.Name);
            await _service.DoSomethingElse();
        }
    }
}