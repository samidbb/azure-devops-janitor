﻿using AzureDevOpsJanitor.Infrastructure.Vsts.DataTransferObjects;
using AzureDevOpsJanitor.Infrastructure.Vsts.Http.Request.Build;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace AzureDevOpsJanitor.Infrastructure.UnitTest.Vsts.Http.Request.Build
{
    public class QueueBuildRequestTests
    {
        [Fact]
        public void CanBeConstructed()
        {
            //Arrange
            QueueBuildRequest sut;

            //Act
            sut = new QueueBuildRequest("my-org", "my-project", 1);

            //Assert
            Assert.NotNull(sut);
            Assert.Equal("6.0", sut.ApiVersion);
            Assert.Equal(HttpMethod.Post, sut.Method);
            Assert.Equal("https://dev.azure.com/my-org/my-project/_apis/build/builds?api-version=6.0&definitionId=1", sut.RequestUri.AbsoluteUri);
        }

        [Fact]
        public async Task CanBeConstructedWithPayload()
        {
            //Arrange
            QueueBuildRequest sut;

            //Act
            sut = new QueueBuildRequest("my-org", "my-project", new DefinitionReferenceDto());

            //Assert
            Assert.NotNull(sut);
            Assert.Equal("6.0", sut.ApiVersion);
            Assert.Equal(HttpMethod.Post, sut.Method);
            Assert.Equal("https://dev.azure.com/my-org/my-project/_apis/build/builds?api-version=6.0&definitionId=0", sut.RequestUri.AbsoluteUri);
            Assert.True(await new StringContent(JsonSerializer.Serialize(new DefinitionReferenceDto())).ReadAsStringAsync() == await sut.Content.ReadAsStringAsync());
        }
    }
}