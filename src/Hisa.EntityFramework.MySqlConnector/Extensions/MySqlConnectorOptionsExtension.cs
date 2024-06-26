using Hisa.EntityFramework.Expressions;
using Hisa.EntityFramework.Providers;
using Lysa.EntityFramework.MySqlConnector.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Lysa.EntityFramework.Options;

/// <summary>
///     MySqlConnector数据库配置选项扩展
/// </summary>
public class MySqlConnectorOptionsExtension : IEntityFrameworkOptionsExtension
{
    /// <summary>
    ///     配置选项
    /// </summary>
    private readonly Action<MySqlConnectorOptions> _optionAction;

    /// <summary>
    ///     构造函数
    /// </summary>
    /// <param name="optionAction">配置选项</param>
    public MySqlConnectorOptionsExtension(Action<MySqlConnectorOptions> optionAction)
    {
        _optionAction = optionAction;
    }

    /// <summary>
    ///     添加服务
    /// </summary>
    /// <param name="services">服务容器</param>
    public void AddServices(IServiceCollection services)
    {
        if (_optionAction != null)
        {
            services.Configure(_optionAction);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<MySqlConnectorOptions>>().Value);
        }

        services.TryAddScoped<IDatabaseConnectionProvider, DatabaseConnectionProvider>();
        services.TryAddSingleton<IDataParameterProvider, DataParameterProvider>();
        services.TryAddTransient<IDatabaseCommandProvider, DatabaseCommandProvider>();
        services.TryAddSingleton<IDataAdapterProvider, DataAdapterProvider>();

        services.TryAddTransient<IQueryBuilderProvider, MySqlQueryBuilderProvider>();

        services.TryAddTransient<ISqlFormatProvider, MySqlFormatProvider>();
        services.TryAddSingleton<ISqlParameterFormatValueProvider, MySqlParameterFormatValueProvider>();


        services.TryAddSingleton<ILambdaExpressionProvider, DefaultLambdaExpressionProvider>();
    }
}