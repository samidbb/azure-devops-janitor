﻿using System;
using System.Net.Http;

namespace AzureDevOpsJanitor.Infrastructure.Vsts.Http.Request.Build.Definition
{
    public sealed class GetDefinitionRequest : ApiRequest
    {
        public GetDefinitionRequest(string organization, string project, int definitionId)
        {
            ApiVersion = "6.1-preview.7";
            Method = HttpMethod.Get;
            RequestUri = new Uri($"https://dev.azure.com/{organization}/{project}/_apis/build/definitions/{definitionId}?api-version={ApiVersion}");
        }
    }
}
