using Microsoft.Extensions.DependencyInjection;
using Soenneker.Utils.FileSync.Abstract;

namespace Soenneker.Utils.FileSync.Extensions;

public static class MemoryStreamUtilRegistrar
{
    /// <summary>
    /// Adds IFileUtilSync as a scoped service. <para/>
    /// Shorthand for <code>services.AddScoped</code> <para/>
    /// Does not need to be registered if you're also using IFileUtil.
    /// </summary>
    public static void AddFileUtilSync(this IServiceCollection services)
    {
        services.AddSingleton<IFileUtilSync, FileUtilSync>();
    }
}