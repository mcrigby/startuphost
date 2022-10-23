using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CutilloRigby.Startup;

public interface IPostBuildConfigure : IStartup
{
    void PostBuildConfigure(IHost host, IServiceScope serviceScope);
}
