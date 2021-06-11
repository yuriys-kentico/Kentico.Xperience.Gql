using System.Threading.Tasks;

using CMS.Core;
using CMS.Globalization;
using CMS.MediaLibrary;
using CMS.SiteProvider;

using Kentico.Content.Web.Mvc;

using Kentico.Xperience.Gql.Schema.Types.Objects;

namespace Kentico.Xperience.Gql.Schema
{
    public class Query
    {
        internal readonly IPageRetriever pageRetriever;
        internal readonly IPageUrlRetriever pageUrlRetriever;
        internal readonly ISiteInfoProvider siteInfoProvider;
        internal readonly IPageAttachmentUrlRetriever pageAttachmentUrlRetriever;
        internal readonly IMediaFileInfoProvider mediaFileInfoProvider;
        internal readonly IMediaFileUrlRetriever mediaFileUrlRetriever;
        internal readonly ICountryInfoProvider countryInfoProvider;
        internal readonly IStateInfoProvider stateInfoProvider;
        private readonly ILocalizationService localizationService;

        public Query(
            IPageRetriever pageRetriever,
            IPageUrlRetriever pageUrlRetriever,
            ISiteInfoProvider siteInfoProvider,
            IPageAttachmentUrlRetriever pageAttachmentUrlRetriever,
            IMediaFileInfoProvider mediaFileInfoProvider,
            IMediaFileUrlRetriever mediaFileUrlRetriever,
            ICountryInfoProvider countryInfoProvider,
            IStateInfoProvider stateInfoProvider,
            ILocalizationService localizationService
            )
        {
            this.pageRetriever = pageRetriever;
            this.pageUrlRetriever = pageUrlRetriever;
            this.siteInfoProvider = siteInfoProvider;
            this.pageAttachmentUrlRetriever = pageAttachmentUrlRetriever;
            this.mediaFileInfoProvider = mediaFileInfoProvider;
            this.mediaFileUrlRetriever = mediaFileUrlRetriever;
            this.countryInfoProvider = countryInfoProvider;
            this.stateInfoProvider = stateInfoProvider;
            this.localizationService = localizationService;
        }

        public async Task<Site> Site(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                var result = await siteInfoProvider.GetAsync(SiteContext.CurrentSiteName);

                return new Site(this, result);
            }
            else
            {
                var result = await siteInfoProvider.GetAsync(name);

                return new Site(this, result);
            }
        }

        public string Localize(string key, string? culture, bool useDefaultCulture = true)
        {
            return localizationService.LocalizeExpression(key, culture, useDefaultCulture: useDefaultCulture);
        }
    }
}