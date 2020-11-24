﻿using AzureDevOpsJanitor.Application.Commands.Build;
using AzureDevOpsJanitor.Domain.Aggregates.Build;
using AzureDevOpsJanitor.Host.Api.Models;
using Microsoft.AspNetCore.Mvc;
using ResourceProvisioning.Abstractions.Facade;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureDevOpsJanitor.Host.Api.Controllers.V1
{
    [ApiController]
	[Route("[controller]")]
	public class BuildController : ControllerBase
	{
		private readonly IFacade _applicationFacade;

        public BuildController(IFacade applicationFacade)
		{
            _applicationFacade = applicationFacade ?? throw new ArgumentNullException(nameof(applicationFacade));
        }
        
        [HttpGet]
        public async Task<IEnumerable<BuildRoot>> Get()
        {
            return await _applicationFacade.Execute(new GetBuildCommand());
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<BuildRoot>> Get(int buildId)
        {
            return await _applicationFacade.Execute(new GetBuildCommand(buildId));
        }

        [HttpPost]
        public async Task<BuildRoot> Post(CreateBuildModel model)
        {
            return await _applicationFacade.Execute(new CreateBuildCommand(model.CapabilityId, model.Definition));
        }

        [HttpDelete]
        public async Task<bool> Delete(int buildId)
        {
            return await _applicationFacade.Execute(new DeleteBuildCommand(buildId));
        }
    }
}
