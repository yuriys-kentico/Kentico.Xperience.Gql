using System;
using System.Net.Http;
using System.Threading.Tasks;

using CMS.Core;

using Kentico.Xperience.Gql.Core;
using Kentico.Xperience.Gql.Core.Services;

using Microsoft.Extensions.Options;

namespace Kentico.Xperience.Gql.Schema.Services
{
    public class SchemaRefreshService : ISchemaRefreshService
    {
        private readonly HttpClient httpClient;
        private readonly XperienceGqlOptions options;
        private readonly IEventLogService eventLogService;
        private bool refreshing;

        public SchemaRefreshService(
            HttpClient httpClient,
            IOptions<XperienceGqlOptions> options,
            IEventLogService eventLogService
            )
        {
            this.httpClient = httpClient;
            this.options = options.Value;
            this.eventLogService = eventLogService;
        }

        public async Task Refresh()
        {
            if (refreshing)
            {
                return;
            }

            try
            {
                refreshing = true;

                var response = await httpClient.PostAsync(options.RefreshEndpoint, null);

                if (!response.IsSuccessStatusCode)
                {
                    eventLogService.LogError(
                        $"{nameof(SchemaRefreshService)}.{nameof(Refresh)}",
                        "NOTSUCCESS",
                        $"Refresh request to '{options.RefreshEndpoint}' resulted in code '{response.StatusCode}' with message '{response.RequestMessage}'."
                        );
                }
                else
                {
                    eventLogService.LogInformation(
                        $"{nameof(SchemaRefreshService)}.{nameof(Refresh)}",
                        "SUCCESS",
                        $"Refresh request to '{options.RefreshEndpoint}' was successful."
                        );
                }
            }
            catch (Exception exception)
            {
                eventLogService.LogException(
                    $"{nameof(SchemaRefreshService)}.{nameof(Refresh)}",
                    "EXCEPTION",
                    exception
                    );
            }
            finally
            {
                refreshing = false;
            }
        }
    }
}