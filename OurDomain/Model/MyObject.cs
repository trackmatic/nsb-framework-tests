using System.Threading.Tasks;
using DomainFramework;
using OurDomain.DomainEvents;

namespace OurDomain.Model
{
    public class MyObject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public static async Task<MyObject> Create(string name)
        {
            var item = new MyObject {Name = name};
            await DomainPublisher.Instance.Publish(new MyObjectCreatedDomainEvent(item));
            return item;
        }
    }
}