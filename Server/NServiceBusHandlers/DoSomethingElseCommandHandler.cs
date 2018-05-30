using System;
using System.Threading.Tasks;
using Commands;
using NServiceBus;

namespace Server.NServiceBusHandlers
{
    public class DoSomethingElseCommandHandler : IHandleMessages<DoSomethingElseCommand>
    {
        public Task Handle(DoSomethingElseCommand message, IMessageHandlerContext context)
        {
            Console.WriteLine("Somethign else happened");
            return Task.CompletedTask;
        }
    }
}
