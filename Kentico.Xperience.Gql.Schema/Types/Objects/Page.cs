using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.Relationships;

namespace Kentico.Xperience.Gql.Schema.Types.Objects
{
    public class Page
    {
        private readonly TreeNode treeNode;

        internal readonly Site site;

        public string Name => treeNode.DocumentName;

        public string Path => treeNode.NodeAliasPath;

        public DateTime? PublishFrom => treeNode.DocumentPublishFrom;

        public DateTime? PublishTo => treeNode.DocumentPublishTo;

        public DateTime Created => treeNode.DocumentCreatedWhen;

        public Page(Site site, TreeNode treeNode)
        {
            this.site = site;
            this.treeNode = treeNode;
        }

        public string Url(bool? relative)
        {
            if (relative == true)
            {
                return site.query.pageUrlRetriever
                    .Retrieve(treeNode)
                    .RelativePath
                    .TrimStart('~');
            }
            else
            {
                return site.query.pageUrlRetriever
                    .Retrieve(treeNode)
                    .AbsoluteUrl;
            }
        }

        public async Task<IEnumerable<Page>> Children(
            IEnumerable<string>? types,
            OrderBy? orderBy,
            int top = 0
            )
        {
            if (orderBy == null)
            {
                orderBy = OrderBy.Default;
            }

            IEnumerable<TreeNode> result;

            if (types == null || types?.Any() == false)
            {
                result = await site.query.pageRetriever
                    .RetrieveMultipleAsync(documentQuery =>
                        orderBy.Apply(documentQuery
                            .WithCoupledColumns()
                            .Path(Path, PathTypeEnum.Children)
                            .TopN(top))
                    );
            }
            else if (types.Count() == 1)
            {
                result = await site.query.pageRetriever
                    .RetrieveAsync(types.First(), documentQuery =>
                        orderBy.Apply(documentQuery
                            .Path(Path, PathTypeEnum.Children)
                            .TopN(top))
                    );
            }
            else
            {
                result = await site.query.pageRetriever
                    .RetrieveMultipleAsync(documentQuery =>
                        orderBy.Apply(documentQuery
                            .Types(types.ToArray())
                            .WithCoupledColumns()
                            .Path(Path, PathTypeEnum.Children)
                            .TopN(top))
                    );
            }

            return result
                .Select(child => new Page(site, child));
        }

        public string? Field(string name)
        {
            return treeNode.GetValue(name)?.ToString();
        }

        public async Task<Attachment> Attachment(string field)
        {
            var attachmentGuid = treeNode.GetGuidValue(field, Guid.Empty);

            if (attachmentGuid == Guid.Empty)
            {
                return new Attachment($"TreeNode '{treeNode.NodeGUID}' does not have a GUID field named '{field}'.");
            }

            var attachment = await Task.FromResult(treeNode.AllAttachments
                .SingleOrDefault(attachment => attachment.AttachmentGUID == attachmentGuid));

            if (attachment == null)
            {
                return new Attachment($"TreeNode '{treeNode.NodeGUID}' does not have an attachment with GUID '{attachmentGuid}'.");
            }

            return new Attachment(this, attachment);
        }

        public async Task<Media> Media(string field)
        {
            var imagePath = Field(field);

            // TODO: Find a way to get media file info from relative path

            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return new Media($"Field named '{field}' is empty.");
            }

            return new Media(site, imagePath);
        }

        public async Task<IEnumerable<Page>?> Links(
            string field,
            int top = 0
            )
        {
            var formField = FormHelper
                .GetFormInfo(treeNode.NodeClassName, clone: false)
                .GetFormField(field);

            if (formField == null)
            {
                return null;
            }

            var adHocRelationshipNameCodeName = RelationshipNameInfoProvider
                .GetAdHocRelationshipNameCodeName(treeNode.NodeClassName, formField);

            var relationshipNameInfo = RelationshipNameInfo.Provider.Get(adHocRelationshipNameCodeName);

            var combineWithDefaultCulture = treeNode.TreeProvider.GetCombineWithDefaultCulture(treeNode.Site.SiteName);

            var result = await site.query.pageRetriever.RetrieveMultipleAsync(
                documentQuery =>
                {
                    var query = documentQuery
                        .Culture(treeNode.DocumentCulture)
                        .CombineWithDefaultCulture(combineWithDefaultCulture)
                        .Published(!treeNode.IsLastVersion)
                        .PublishedVersion(!treeNode.IsLastVersion)
                        .WithCoupledColumns()
                        .TopN(top)
                        .InRelationWith(
                            treeNode.NodeGUID,
                            adHocRelationshipNameCodeName,
                            RelationshipSideEnum.Left
                            );

                    query
                        .Source(parameters =>
                         {
                             parameters.Join<RelationshipInfo>(
                                 nameof(TreeNode.NodeID),
                                 nameof(RelationshipInfo.RightNodeId),
                                 JoinTypeEnum.Inner,
                                 new WhereCondition()
                                    .WhereEquals(
                                        nameof(RelationshipInfo.RelationshipNameId),
                                        relationshipNameInfo.RelationshipNameId
                                        )
                                    .WhereEquals(
                                        nameof(RelationshipInfo.LeftNodeId),
                                        treeNode.NodeID
                                        ));
                         })
                        .ResultOrderBy(nameof(RelationshipInfo.RelationshipOrder));
                });

            return result
                .Select(treeNode => new Page(site, treeNode));
        }

        public async Task<CountryState> CountryState(string field)
        {
            var countryState = Field(field);

            if (string.IsNullOrWhiteSpace(countryState))
            {
                return new CountryState($"Field named '{field}' is empty.");
            }

            var tokens = countryState.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            var countryName = tokens[0];
            var stateName = string.Empty;

            if (tokens.Length > 1)
            {
                stateName = tokens[1];
            }

            var country = await site.query.countryInfoProvider.GetAsync(countryName);
            var state = await site.query.stateInfoProvider.GetAsync(stateName);

            return new CountryState(country, state);
        }
    }
}