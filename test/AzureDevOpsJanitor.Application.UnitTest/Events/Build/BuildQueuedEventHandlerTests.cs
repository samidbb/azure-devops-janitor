﻿using AutoMapper;
using AzureDevOpsJanitor.Application.Events.Build;
using AzureDevOpsJanitor.Domain.ValueObjects;
using AzureDevOpsJanitor.Infrastructure.Vsts;
using AzureDevOpsJanitor.Infrastructure.Vsts.DataTransferObjects;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AzureDevOpsJanitor.Application.UnitTest.Events.Build
{
    public class BuildQueuedEventHandlerTests
    {
        [Fact]
        public void CanBeConstructed()
        {
            //Arrange
            BuildQueuedEventHandler sut;
            var mockMapper = new Mock<IMapper>();
            var mockVstsRestClient = new Mock<IVstsRestClient>();

            //Act
            sut = new BuildQueuedEventHandler(mockMapper.Object, mockVstsRestClient.Object);

            //Assert
            Assert.NotNull(sut);
        }

        [Fact]
        public async Task CanHandleEvent() 
        {
            //Arrange
            var mockMapper = new Mock<IMapper>();
            var mockVstsRestClient = new Mock<IVstsRestClient>();
            var fakeVstsPayload = new DefinitionReferenceDto() { 
                Id = 1,
                Name = "my-def",
                Project = "my-proj",
                QueueStatus = "my-queue-status",
                Type = "my-type",
                Revision = "my-revision"
            };

            var sut = new BuildQueuedEventHandler(mockMapper.Object, mockVstsRestClient.Object);

            mockMapper.Setup(m => m.Map<DefinitionReferenceDto>(It.IsAny<BuildDefinition>())).Returns(fakeVstsPayload);
            mockVstsRestClient.Setup(m => m.QueueBuild(It.IsAny<string>(), It.IsAny<string>(), fakeVstsPayload));

            //Act
            await sut.Handle(new Domain.Events.Build.BuildQueuedEvent(new Domain.Aggregates.Build.BuildRoot(Guid.NewGuid(), "my-capability-identifier", new BuildDefinition("name", "yaml", 1))));

            //Assert
            mockMapper.VerifyAll();
        }
    }
}