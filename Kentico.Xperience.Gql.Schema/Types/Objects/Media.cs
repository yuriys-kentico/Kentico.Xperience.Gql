using CMS.DocumentEngine;

namespace Kentico.Xperience.Gql.Schema.Types.Objects
{
    public class Media : ErrorObject
    {
        private readonly string relativePath;

        internal readonly Site site;

        public Media(Site site, string relativePath) : base(null)
        {
            this.site = site;
            this.relativePath = relativePath;
        }

        public Media(string errorMessage) : base(errorMessage)
        {
            site = null!;
            relativePath = null!;
        }

        public string Url(bool? relative)
        {
            if (relative == true)
            {
                return relativePath.TrimStart('~');
            }
            else
            {
                return DocumentURLProvider.GetAbsoluteUrl(relativePath, site.Id);
            }
        }
    }
}