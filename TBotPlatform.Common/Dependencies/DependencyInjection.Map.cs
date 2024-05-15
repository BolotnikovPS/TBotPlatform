using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TBotPlatform.Common.Mapper;
using TBotPlatform.Common.Mapper.Config;
using TBotPlatform.Contracts.Abstractions;

namespace TBotPlatform.Common.Dependencies;

public static partial class DependencyInjection
{
    public static IServiceCollection AddMap(this IServiceCollection services, Assembly executingAssembly)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(executingAssembly);
        typeAdapterConfig.Apply(new MarkupRegister());

        services
           .AddSingleton<IMapper>(new MapsterMapper.Mapper(typeAdapterConfig))
           .AddScoped<IMap, Map>();

        return services;
    }
}
