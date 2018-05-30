using System;
using Raven.Client;

namespace Server
{
    public class AsyncSessionProvider : IAsyncSessionProvider
    {
        private IAsyncDocumentSession _session;

        public AsyncSessionProvider()
        {
            Console.WriteLine("Created a new async session provider");
        }

        public void Set(IAsyncDocumentSession session)
        {
            _session = session;
        }

        public IAsyncDocumentSession Instance
        {
            get
            {
                if (_session == null)
                {
                    throw new InvalidOperationException("A document session has not been created for this unit of work");
                }

                return _session;
            }
        }
    }
}