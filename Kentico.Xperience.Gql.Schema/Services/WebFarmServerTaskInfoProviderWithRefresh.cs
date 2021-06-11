using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.WebFarmSync;

using Kentico.Xperience.Gql.Schema.Services;

[assembly: AssemblyDiscoverable]
[assembly: RegisterCustomProvider(typeof(WebFarmServerTaskInfoProviderWithRefresh))]

namespace Kentico.Xperience.Gql.Schema.Services
{
    public class WebFarmServerTaskInfoProviderWithRefresh : WebFarmServerTaskInfoProvider
    {
        public static SchemaRefreshService? SchemaRefreshService { get; set; }

        protected override ObjectQuery<WebFarmServerTaskInfo> GetObjectQueryInternal()
        {
            var objectQueryInternal = base.GetObjectQueryInternal();

            if (objectQueryInternal
                .WhereEquals("ServerId", WebFarmServerInfo.Provider.Get(SystemContext.ServerName).ServerID)
                .WhereNull("ErrorMessage")
                .TopN(1)
                .HasResults() && SchemaRefreshService != null)
            {
                SchemaRefreshService.Refresh().ConfigureAwait(false);
            }

            return objectQueryInternal;
        }
    }
}