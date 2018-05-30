using System;
using System.Threading.Tasks;
using Commands;
using NServiceBus;

namespace Server
{
    public class DoSomethingCommandHandler : IHandleMessages<DoSomethingCommand>
    {
        public Task Handle(DoSomethingCommand message, IMessageHandlerContext context)
        {
            Console.WriteLine("Do Something");
            return Task.CompletedTask;
        }
    }
}