using System;

namespace Microsoft.Azure.Mobile.Core.Server.Managers
{
    public class DomainManagerException : Exception
    {
        public DomainManagerException() : base() { }

        public DomainManagerException(string message): base(message) { }

        public DomainManagerException(string message, Exception inner) : base(message, inner) { }
    }
}
