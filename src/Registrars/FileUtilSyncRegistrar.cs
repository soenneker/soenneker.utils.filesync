using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Utils.Directory.Registrars;
using Soenneker.Utils.FileSync.Abstract;

namespace Soenneker.Utils.FileSync.Registrars;

/// <summary>
/// A utility library encapsulating synchronous file IO operations
/// </summary>
public static class FileUtilSyncRegistrar
{
    /// <summary>
    /// Adds <see cref="IFileUtilSync"/> as a scoped service. <para/>
    /// Shorthand for <code>services.AddScoped</code> <para/>
    /// Does NOT need to be registered if you're also using IFileUtil.
    /// </summary>
    public static void AddFileUtilSync(this IServiceCollection services)
    {
        services.TryAddScoped<IFileUtilSync, FileUtilSync>();
        services.AddDirectoryUtilAsScoped();
    }

    /// <summary>
    /// Adds <see cref="IFileUtilSync"/> as a singleton service. <para/>
    /// Shorthand for <code>services.AddSingleton</code> <para/>
    /// Does NOT need to be registered if you're also using IFileUtil.
    /// </summary>
    public static void AddFileUtilSyncAsSingleton(this IServiceCollection services)
    {
        services.TryAddSingleton<IFileUtilSync, FileUtilSync>();
        services.AddDirectoryUtilAsSingleton();
    }
}