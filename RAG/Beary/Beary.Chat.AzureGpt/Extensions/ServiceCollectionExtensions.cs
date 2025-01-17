﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Beary.Chat.Interfaces;

namespace Beary.Chat.AzureGpt.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseAzureGptChatClient(this IServiceCollection services)
    {
        return services.AddSingleton<ICreateChatCompletions, AzureGpt.Client>();
    }
}
