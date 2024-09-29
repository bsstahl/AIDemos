using Beary.Articles.FileSystem.Extensions;
using Beary.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bimp;

/// <summary>
/// Beary Import - Grab data from the blog post repository and 
/// import it into Azure Search.
/// </summary>
internal class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Bimp.Program>()
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .UseBearyFileSystemRepository()
            .AddSingleton<Program>()
            // .UseBearyReadRepository() TODO: Restore if needed
            // .UseBearyWriteRepository()  TODO: Restore if needed
            .BuildServiceProvider();

        var program = services.GetRequiredService<Import>();
        await program.Execute();
    }
}