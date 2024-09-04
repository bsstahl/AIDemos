using Beary.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beary.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseBearyWriteRepository(this IServiceCollection services)
    {
        return services
            .AddSingleton<IWriteContent, WriteRepository>();
    }
}
