﻿using AzureDevOpsJanitor.Infrastructure.Vsts.Events;
using System;
using System.Text.Json;
using Xunit;

namespace AzureDevOpsJanitor.Infrastructure.UnitTest.Vsts.Events
{
    public class BuildCompletedEventTests
    {
        [Fact]
        public void CanBeConstructed()
        {
            //Arrange
            BuildCompletedEvent sut;

            //Act
            sut = new BuildCompletedEvent();

            //Assert
            Assert.NotNull(sut);
        }

        [Fact]
        public void CanBeSerialized()
        {
            //Arrange
            var sut = new BuildCompletedEvent() { 
                Id = Guid.NewGuid(),
                EventType = "MyEventType",
                PublisherId = "MyPublisherId",
                Scope = "MyScope"
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
            BuildCompletedEvent sut;

            //Act
            sut = JsonSerializer.Deserialize<BuildCompletedEvent>("{\"id\":\"3ededfb7-5b60-49d9-9c47-80bbf8f2dcb1\",\"publisherId\":\"MyPublisherId\",\"eventType\":\"MyEventType\",\"scope\":\"MyScope\"}");

            //Assert
            Assert.NotNull(sut);
            Assert.Equal(Guid.Parse("3ededfb7-5b60-49d9-9c47-80bbf8f2dcb1"), sut.Id);
            Assert.Equal("MyPublisherId", sut.PublisherId);
            Assert.Equal("MyEventType", sut.EventType);
            Assert.Equal("MyScope", sut.Scope);
        }
    }
}