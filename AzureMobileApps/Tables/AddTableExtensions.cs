using Microsoft.Azure.Mobile.Core.Server.Abstractions;

namespace Microsoft.Azure.Mobile.Core.Server.Tables
{
    public static class AddTableExtensions
    {
        public static ITableBuilder AddTable(
            this ITableBuilder builder,
            string name,
            IDomainManager manager,
            IMobileTableAuthorization authorization = null,
            IMobileTableFilter filter = null,
            IMobileTableTransform transform = null,
            IMobileTableAction action = null
            )
        {
            var table = new MobileTable(name, manager)
            {
                Authorization = authorization,
                Filter = filter,
                Transform = transform,
                Action = action
            };
            builder.Tables.Add(table);
            return builder;
        }
    }
}
