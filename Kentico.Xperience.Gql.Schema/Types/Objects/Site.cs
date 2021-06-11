using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CMS.DocumentEngine;
using CMS.SiteProvider;

namespace Kentico.Xperience.Gql.Schema.Types.Objects
{
    public class Site
    {
        private readonly SiteInfo siteInfo;

        internal readonly Query query;

        public int Id => siteInfo.SiteID;

        public string Name => siteInfo.DisplayName;

        public Site(Query query, SiteInfo siteInfo)
        {
            this.query = query;
            this.siteInfo = siteInfo;
        }

        public async Task<Page> Page(string path)
        {
            var result = await query.pageRetriever
                .RetrieveMultipleAsync(documentQuery =>
                    documentQuery
                    .Path(path)
                    .WithCoupledColumns()
                    .OnSite(siteInfo.SiteName)
                );

            return new Page(this, result.First());
        }

        public async Task<IEnumerable<Page>> Menu(OrderBy? orderBy)
        {
            if (orderBy == null)
            {
                orderBy = OrderBy.Default;
            }

            var result = await query.pageRetriever
                .RetrieveMultipleAsync(documentQuery =>
                    orderBy.Apply(documentQuery
                        .MenuItems()
                        .WithCoupledColumns()
                        .OnSite(siteInfo.SiteName))
                );

            return result
                .Select(child => new Page(this, child));
        }
    }
}