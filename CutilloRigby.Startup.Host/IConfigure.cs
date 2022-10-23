using Microsoft.Extensions.Hosting;

namespace CutilloRigby.Startup;

public interface IConfigure : IStartup
{
    void Configure(IHostBuilder hostBuilder);
}
