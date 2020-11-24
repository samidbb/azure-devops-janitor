﻿using AutoMapper;
using AzureDevOpsJanitor.Application.Behaviors;
using AzureDevOpsJanitor.Application.Cache;
using AzureDevOpsJanitor.Application.Commands.Build;
using AzureDevOpsJanitor.Application.Commands.Profile;
using AzureDevOpsJanitor.Application.Events.Build;
using AzureDevOpsJanitor.Application.Repositories;
using AzureDevOpsJanitor.Application.Services;
using AzureDevOpsJanitor.Domain.Aggregates.Build;
using AzureDevOpsJanitor.Domain.Events.Build;
using AzureDevOpsJanitor.Domain.Repository;
using AzureDevOpsJanitor.Domain.Services;
using AzureDevOpsJanitor.Domain.ValueObjects;
using AzureDevOpsJanitor.Infrastructure.EntityFramework;
using AzureDevOpsJanitor.Infrastructure.Vsts;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResourceProvisioning.Abstractions.Commands;
using ResourceProvisioning.Abstractions.Data;
using ResourceProvisioning.Abstractions.Events;
using ResourceProvisioning.Abstractions.Facade;
using ResourceProvisioning.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

namespace AzureDevOpsJanitor.Application
{
	public static class DependencyInjection
	{
		public static void AddApplication(this IServiceCollection services, Action<ApplicationFacadeOptions> configureOptions = default)
		{
			var options = new ApplicationFacadeOptions();

			configureOptions?.Invoke(options);

			services.AddLogging();
			services.AddOptions<ApplicationFacadeOptions>()
					.Configure(configureOptions);
			services.AddCache();
			services.AddEntityFramework(options);
			services.AddAutoMapper(Assembly.GetExecutingAssembly());
			services.AddMediator();
			services.AddBehaviors();
			services.AddCommandHandlers();
			services.AddEventHandlers();
			services.AddRepositories();
			services.AddServices();
			services.AddClients();
			services.AddFacade();
		}

		private static void AddMediator(this IServiceCollection services)
		{
			services.AddTransient<ServiceFactory>(p => p.GetService);
			services.AddTransient<IMediator>(p => new Mediator(p.GetService<ServiceFactory>()));
		}

		private static void AddCache(this IServiceCollection services)
		{
			services.AddSingleton<IMemoryCache, ApplicationCache>();
		}

		private static void AddBehaviors(this IServiceCollection services)
		{
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
		}

		private static void AddCommandHandlers(this IServiceCollection services)
		{
			services.AddTransient<IRequestHandler<GetBuildCommand, IEnumerable<BuildRoot>>, GetBuildCommandHandler>();
			services.AddTransient<IRequestHandler<CreateBuildCommand, BuildRoot>, CreateBuildCommandHandler>();
			services.AddTransient<IRequestHandler<DeleteBuildCommand, bool>, DeleteBuildCommandHandler>();
			services.AddTransient<IRequestHandler<GetProfileCommand, UserProfile>, GetProfileCommandHandler>();

			services.AddTransient<ICommandHandler<GetBuildCommand, IEnumerable<BuildRoot>>, GetBuildCommandHandler>();
			services.AddTransient<ICommandHandler<CreateBuildCommand, BuildRoot>, CreateBuildCommandHandler>();
			services.AddTransient<ICommandHandler<DeleteBuildCommand, bool>, DeleteBuildCommandHandler>();
			services.AddTransient<ICommandHandler<GetProfileCommand, UserProfile>, GetProfileCommandHandler>();
		}

		private static void AddEventHandlers(this IServiceCollection services)
		{
			services.AddTransient<INotificationHandler<BuildRequestedEvent>, BuildRequestedEventHandler>();
			services.AddTransient<INotificationHandler<BuildCreatedEvent>, BuildCreatedEventHandler>();
			services.AddTransient<INotificationHandler<BuildCompletedEvent>, BuildCompletedEventHandler>();

			services.AddTransient<IEventHandler<BuildRequestedEvent>, BuildRequestedEventHandler>();
			services.AddTransient<IEventHandler<BuildCreatedEvent>, BuildCreatedEventHandler>();
			services.AddTransient<IEventHandler<BuildCompletedEvent>, BuildCompletedEventHandler>();
		}

		private static void AddRepositories(this IServiceCollection services)
		{
			services.AddTransient<IRepository<BuildRoot>, BuildRepository>();
			services.AddTransient<IBuildRepository, BuildRepository>();
		}

		private static void AddServices(this IServiceCollection services)
		{
			services.AddTransient<IBuildService, BuildService>();
			services.AddTransient<IProfileService, ProfileService>();
		}

		private static void AddFacade(this IServiceCollection services)
		{
			services.AddTransient<IFacade, ApplicationFacade>();
		}

		private static void AddClients(this IServiceCollection services)
		{
			services.AddTransient<IVstsRestClient, VstsRestClient>(p => new VstsRestClient(p.GetService<IMemoryCache>().Get<JwtSecurityToken>(VstsRestClient.VstsAccessTokenCacheKey)));
		}

		private static void AddEntityFramework(this IServiceCollection services, ApplicationFacadeOptions brokerOptions = default)
		{
			services.AddDbContext<DomainContext>(options =>
			{
				var callingAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
				var connectionString = brokerOptions?.ConnectionStrings?.GetValue<string>(nameof(DomainContext));

				if (string.IsNullOrEmpty(connectionString))
				{
					return;
				}

				services.AddSingleton(factory =>
				{
					var connection = new SqliteConnection(connectionString);

					connection.Open();

					return connection;
				});

				var dbOptions = options.UseSqlite(services.BuildServiceProvider().GetService<SqliteConnection>(),
					sqliteOptions =>
					{
						sqliteOptions.MigrationsAssembly(callingAssemblyName);
						sqliteOptions.MigrationsHistoryTable(callingAssemblyName + "_MigrationHistory");

					}).Options;

				using var context = new DomainContext(dbOptions, new FakeMediator());

				if (!context.Database.EnsureCreated())
				{
					return;
				}

				if (brokerOptions.EnableAutoMigrations)
				{
					context.Database.Migrate();
				}
			});

			services.AddScoped<IUnitOfWork>(factory => factory.GetRequiredService<DomainContext>());
		}
	}
}
