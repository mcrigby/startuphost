using Microsoft.Extensions.Hosting;

namespace CutilloRigby.Startup;

public interface IHostConfigure : IStartup
{
    void Configure(IHostBuilder hostBuilder);
}
