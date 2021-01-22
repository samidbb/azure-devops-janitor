﻿using AutoMapper;
using AzureDevOpsJanitor.Infrastructure.Kafka.Strategies;
using AzureDevOpsJanitor.Infrastructure.Vsts.DataTransferObjects;
using Confluent.Kafka;
using ResourceProvisioning.Abstractions.Aggregates;
using ResourceProvisioning.Abstractions.Commands;
using ResourceProvisioning.Abstractions.Events;
using ResourceProvisioning.Abstractions.Facade;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AzureDevOpsJanitor.Infrastructure.Vsts.Kafka
{
    public sealed class WebHookConsumptionStrategy : ConsumtionStrategy
    {
        public WebHookConsumptionStrategy(IMapper mapper, IFacade applicationFacade) : base(mapper, applicationFacade)
        {
        }

        public override ValueTask<ConsumeResult<Ignore, string>> Apply(ConsumeResult<Ignore, string> target, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(target.Message.Value))
            {
                var payload = JsonSerializer.Deserialize<IIntegrationEvent>(target.Message.Value);
                IAggregateRoot aggregate;

                //TODO: Finish switch or create a automapper type converter that can figure out how to convert JsonElement to any dto.
                switch (payload.Type)
                {
                    case nameof(JsonElement):
                    default:
                        var jsonElement = JsonSerializer.Deserialize<JsonElement>(payload.Payload?.GetString());

                        aggregate = _mapper.Map<IAggregateRoot>(jsonElement);
                        break;
                }

                var command = _mapper.Map<IAggregateRoot, ICommand<IAggregateRoot>>(aggregate);

                if (command != null)
                {
                    _applicationFacade.Execute(command, cancellationToken);
                }
            }

            return new ValueTask<ConsumeResult<Ignore, string>>(target);
        }
    }
}
