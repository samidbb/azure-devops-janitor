﻿using System;
using System.Net.Http;

namespace AzureDevOpsJanitor.Infrastructure.Vsts.Http.Request.Profile
{
    public sealed class GetProfileRequest : ApiRequest
    {
        public GetProfileRequest(string profileId)
        {
            ApiVersion = "6.1-preview.3";
            Method = HttpMethod.Get;
            RequestUri = new Uri($"https://app.vssps.visualstudio.com/_apis/profile/profiles/{profileId}?api-version={ApiVersion}");
        }
    }
}
