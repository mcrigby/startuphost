using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CutilloRigby.Startup;

public interface IConfigureServices : IStartup
{
    void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration);
}
