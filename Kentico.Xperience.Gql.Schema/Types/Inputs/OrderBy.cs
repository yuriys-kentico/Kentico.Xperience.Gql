using System.Collections.Generic;
using System.Linq;

using CMS.DataEngine;

using Kentico.Xperience.Gql.Schema.Types.Enums;

namespace Kentico.Xperience.Gql.Schema.Types.Objects
{
    public class OrderBy
    {
        public static OrderBy Default => new OrderBy();

        public IEnumerable<string> Columns { get; set; } = Enumerable.Empty<string>();

        public Order? Order { get; set; }

        internal DataQuerySettingsBase<TQuery> Apply<TQuery>(DataQuerySettingsBase<TQuery> documentQuery)
            where TQuery : DataQuerySettingsBase<TQuery>, new()
        {
            if (Order == null)
            {
                Order = Enums.Order.Ascending;
            }

            return Order.Value switch
            {
                Enums.Order.Descending => documentQuery
                    .OrderByDescending(Columns.ToArray()),
                _ => documentQuery
                    .OrderByAscending(Columns.ToArray()),
            };
        }
    }
}