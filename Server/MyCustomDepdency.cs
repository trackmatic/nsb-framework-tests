using System;

namespace Server
{
    public class MyCustomDepdency : IMyCustomDepedency
    {
        public void DoSometing()
        {
            Console.WriteLine("Do something");
        }
    }
}
