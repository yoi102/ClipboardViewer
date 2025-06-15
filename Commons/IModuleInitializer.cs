using Microsoft.Extensions.DependencyInjection;

namespace Commons;

public interface IModuleInitializer
{
    void Initialize(IServiceCollection services);
}