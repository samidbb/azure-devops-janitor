﻿using AzureDevOpsJanitor.Domain.Aggregates.Project;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace AzureDevOpsJanitor.Domain.UnitTest.Events.Project
{
    public class ProjectRootTests
    {
        [Fact]
        public void CanBeConstructed()
        {
            //Arrange
            ProjectRoot sut;

            //Act
            sut = new ProjectRoot("my_project");

            //Assert
            Assert.NotNull(sut);
            Assert.True(sut.Id != Guid.Empty);
            Assert.True(sut.Name == "my_project");
            Assert.True(sut.DomainEvents.Count == 1);
        }

        [Fact]
        public void CanDetectValidConstruction()
        {
            //Arrange
            var sut = new ProjectRoot("my_project");
            var validationCtx = new ValidationContext(this);

            //Act
            var validationResults = sut.Validate(validationCtx);

            //Assert
            Assert.True(validationResults.Count() == 0);
        }

        [Fact]
        public void CanDetectInvalidConstruction()
        {
            //Arrange
            var sut = new ProjectRoot(string.Empty);
            var validationCtx = new ValidationContext(this);

            //Act
            var validationResults = sut.Validate(validationCtx);

            //Assert
            Assert.True(validationResults.Count() == 1);
        }
    }
}