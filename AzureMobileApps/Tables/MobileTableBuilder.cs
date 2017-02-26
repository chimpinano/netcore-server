using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Mobile.Core.Server.Abstractions;

namespace Microsoft.Azure.Mobile.Core.Server.Tables
{
    public class MobileTableBuilder : ITableBuilder
    {
        public MobileTableBuilder()
        {
            ApplicationBuilder = null;
            Tables = new List<ITable>();
        }

        public MobileTableBuilder(IApplicationBuilder app)
        {
            ApplicationBuilder = app ?? throw new ArgumentNullException(nameof(app));
            Tables = new List<ITable>();
        }

        public IApplicationBuilder ApplicationBuilder { get; }
        public IList<ITable> Tables { get; } 
    }
}
