using CMS.DocumentEngine;

namespace Kentico.Xperience.Gql.Schema.Types.Objects
{
    public class Attachment : ErrorObject
    {
        private readonly DocumentAttachment attachment;

        internal readonly Page page;

        public string Name => attachment.AttachmentName;

        public bool IsImage => page.site.query.pageAttachmentUrlRetriever
            .Retrieve(attachment)
            .IsImage;

        public Attachment(Page page, DocumentAttachment attachment) : base(null)
        {
            this.page = page;
            this.attachment = attachment;
        }

        public Attachment(string errorMessage) : base(errorMessage)
        {
            page = null!;
            attachment = null!;
        }

        public string Url(bool? relative)
        {
            if (relative == true)
            {
                return page.site.query.pageAttachmentUrlRetriever
                    .Retrieve(attachment)
                    .RelativePath
                    .TrimStart('~');
            }
            else
            {
                return page.site.query.pageAttachmentUrlRetriever
                    .Retrieve(attachment)
                    .AbsoluteUrl;
            }
        }
    }
}