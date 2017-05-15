using System;

namespace Auction.Desktop.Persistence
{
    public class PersistenceUnavailableException : Exception
    {
        public PersistenceUnavailableException(String message) : base(message) { }

        public PersistenceUnavailableException(Exception innerException) : base("Exception occurred.", innerException) { }
    }
}
