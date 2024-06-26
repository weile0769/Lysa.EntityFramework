using Hisa.EntityFramework;
using Hisa.EntityFramework.Builders;
using Hisa.EntityFramework.Caching;
using Hisa.EntityFramework.Expressions;
using Hisa.EntityFramework.Options;
using Hisa.EntityFramework.Providers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     IServiceCollection服务容器扩展类
/// </summary>
public static class IServiceCollectionExtension
{
    /// <summary>
    ///     注册数据库实体框架
    /// </summary>
    /// <param name="services">服务容器</param>
    /// <param name="optionAction">配置选项</param>
    /// <returns></returns>
    public static IEntityFrameworkBuilder AddHisaSqlEntityFramework(this IServiceCollection services, Action<EntityFrameworkOptions> optionAction)
    {
        //配置选注册
        var options = new EntityFrameworkOptions();
        optionAction(options);
        foreach (var serviceExtension in options.Extensions)
        {
            serviceExtension.AddServices(services);
        }

        services.AddMemoryCache();
        services.TryAddSingleton<ICacheProvider, DefaultCacheProvider>();

        services.Configure(optionAction);
        services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<EntityFrameworkOptions>>().Value);
        services.AddSingleton<IDatabaseConnectorOptionsProvider, DefaultDatabaseConnectorOptionsProvider>();


        services.TryAddTransient(typeof(IQueryableProvider<>), typeof(DefaultQueryableProvider<>));

        services.TryAddTransient<IAdoProvider, DefaultAdoProvider>();
        services.TryAddSingleton<ISqlParameterProvider, DefaultSqlParameterProvider>();
        services.TryAddTransient<IDataReaderProvider, DefaultDataReaderProvider>();
        services.TryAddSingleton<ISqlParameterTypeConvertProvider, DefaultSqlParameterTypeConvertProvider>();
        services.TryAddSingleton<ISqlParameterFormatProvider, DefaultSqlParameterFormatProvider>();

        services.TryAddSingleton<IDataReaderTypeConvertProvider, DefaultDataReaderTypeConvertProvider>();

        services.TryAddSingleton(typeof(DefaultEntityMappingProvider));
        services.TryAddSingleton<IEntityMappingProvider, CachingEntityMappingProvider<DefaultEntityMappingProvider>>();
        services.TryAddTransient(typeof(DataReaderEntityBuilder<>));
        services.TryAddSingleton(typeof(IDataReaderEntityBuilder<>), typeof(CachingDataReaderEntityBuilder<>));


        services.TryAddScoped<ISqlClient, DefaultSqlClient>();


        services.TryAddSingleton<IBinaryExpressionResolver, DefaultBinaryExpressionResolver>();
        services.TryAddSingleton<IMemberAccessExpressionResolver, DefaultMemberAccessExpressionResolver>();
        services.TryAddSingleton<IConstantExpressionResolver, DefaultConstantExpressionResolver>();

        return new EntityFrameworkBuilder(services);
    }
}