using Microsoft.Azure.Mobile.Core.Server.Abstractions;
using System;

namespace Microsoft.Azure.Mobile.Core.Server.Tables
{
    public class MobileTable : ITable
    {
        public MobileTable(string name, IDomainManager manager)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.DomainManager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public string Name { get; }

        public IDomainManager DomainManager { get; }

        public IMobileTableAuthorization Authorization { get; set; } = null;
        public IMobileTableFilter Filter { get; set; } = null;
        public IMobileTableTransform Transform { get; set; } = null;
        public IMobileTableAction Action { get; set; } = null;
    }
}
