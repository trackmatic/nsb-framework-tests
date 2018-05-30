using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

namespace Server
{
    public class MyCustomBehavior : Behavior<IInvokeHandlerContext>
    {
        public override Task Invoke(IInvokeHandlerContext context, Func<Task> next)
        {
            var myCustomDepedency = context.Builder.Build<IMyCustomDepedency>();
            myCustomDepedency.DoSometing();
            return next();
        }
    }
}
