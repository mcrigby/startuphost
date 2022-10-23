using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CutilloRigby.Startup;

public interface IConfigurationConfigure : IStartup
{
    void Configure(IHostEnvironment hostingEnvironment, IConfiguration configuration);
}
