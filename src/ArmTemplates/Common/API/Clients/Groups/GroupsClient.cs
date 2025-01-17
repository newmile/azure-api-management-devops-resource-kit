﻿// --------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License.
// --------------------------------------------------------------------------

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common.API.Clients.Abstractions;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common.Constants;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common.Templates.Groups;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Extractor.Models;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Extractor.Utilities.DataProcessors.Absctraction;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common.API.Clients.Groups
{
    public class GroupsClient : ApiClientBase, IGroupsClient
    {
        const string GetAllGroupsLinkedToProductRequest = "{0}/subscriptions/{1}/resourceGroups/{2}/providers/Microsoft.ApiManagement/service/{3}/products/{4}/groups?api-version={5}";
        const string GetAllRequest = "{0}/subscriptions/{1}/resourceGroups/{2}/providers/Microsoft.ApiManagement/service/{3}/groups?api-version={4}";

        readonly IGroupDataProcessor groupDataProcessor;

        public GroupsClient(
            IHttpClientFactory httpClientFactory,
            IGroupDataProcessor groupDataProcessor): base(httpClientFactory)
        {
            this.groupDataProcessor = groupDataProcessor;
        }

        public async Task<List<GroupTemplateResource>> GetAllAsync(ExtractorParameters extractorParameters)
        {
            var (azToken, azSubId) = await this.Auth.GetAccessToken();

            var requestUrl = string.Format(GetAllRequest,
                this.BaseUrl, azSubId, extractorParameters.ResourceGroup, extractorParameters.SourceApimName, GlobalConstants.ApiVersion);

            var groupTemplates = await this.GetPagedResponseAsync<GroupTemplateResource>(azToken, requestUrl);
            this.groupDataProcessor.ProcessData(groupTemplates, extractorParameters);

            return groupTemplates;
        }

        public async Task<List<GroupTemplateResource>> GetAllLinkedToProductAsync(string productName, ExtractorParameters extractorParameters)
        {
            var (azToken, azSubId) = await this.Auth.GetAccessToken();

            var requestUrl = string.Format(GetAllGroupsLinkedToProductRequest,
                this.BaseUrl, azSubId, extractorParameters.ResourceGroup, extractorParameters.SourceApimName, productName, GlobalConstants.ApiVersion);

            var groupTemplates = await this.GetPagedResponseAsync<GroupTemplateResource>(azToken, requestUrl);
            this.groupDataProcessor.ProcessData(groupTemplates, extractorParameters);

            return groupTemplates;
        }
    }
}
