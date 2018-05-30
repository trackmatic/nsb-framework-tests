using DomainFramework;
using OurDomain.Model;

namespace OurDomain.DomainEvents
{
    public class MyObjectCreatedDomainEvent : IDomainEvent
    {
        public MyObject MyObject { get; }

        public MyObjectCreatedDomainEvent(MyObject myObject)
        {
            MyObject = myObject;
        }
    }
}
