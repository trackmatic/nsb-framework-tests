namespace DomainFramework
{
    public class DomainPublisher
    {
        public static IDomainPublisherFactory Factory;

        public static IDomainPubisher Instance => Factory.Create();
    }
}
