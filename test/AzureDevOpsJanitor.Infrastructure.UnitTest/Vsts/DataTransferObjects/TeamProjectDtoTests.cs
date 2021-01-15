﻿using AzureDevOpsJanitor.Infrastructure.Vsts.DataTransferObjects;
using System;
using System.Text.Json;
using Xunit;

namespace AzureDevOpsJanitor.Infrastructure.UnitTest.Vsts.Events
{
    public class TeamProjectDtoTests
    {
        [Fact]
        public void CanBeConstructed()
        {
            //Arrange
            TeamProjectDto sut;

            //Act
            sut = new TeamProjectDto();

            //Assert
            Assert.NotNull(sut);
        }

        [Fact]
        public void CanBeSerialized()
        {
            //Arrange
            var sut = new TeamProjectDto() { 
                Id = Guid.NewGuid(),
                Name = "MyName",
                Description = "MyDescription",
                State = "MyState",
                Url = new Uri("https://my-team-project-url/")
            };
            
            //Act
            var payload = JsonSerializer.Serialize(sut, new JsonSerializerOptions { IgnoreNullValues = true });

            //Assert
            Assert.NotNull(JsonDocument.Parse(payload));
        }

        [Fact]
        public void CanBeDeserialized()
        {
            //Arrange
            TeamProjectDto sut;

            //Act
            sut = JsonSerializer.Deserialize<TeamProjectDto>("{\"id\": \"db6af750-1f7b-474d-af65-f7c6106604ec\",\"name\":\"MyName\",\"description\":\"MyDescription\",\"state\":\"MyState\",\"url\":\"https://my-team-project-url\"}");

            //Assert
            Assert.NotNull(sut);
            Assert.Equal("db6af750-1f7b-474d-af65-f7c6106604ec".ToLower(), sut.Id.ToString());
            Assert.Equal("MyName", sut.Name);
            Assert.Equal("MyDescription", sut.Description);
            Assert.Equal("MyState", sut.State);
            Assert.Equal("https://my-team-project-url/", sut.Url.AbsoluteUri);
        }
    }
}