using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDDD.Core.Testing;

public static class DummyServiceProvider
{
    public static Mock<IServiceProvider> Setup()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var scope = new Mock<IServiceScope>();
        scope.SetupGet(s => s.ServiceProvider)
            .Returns(serviceProvider.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope())
            .Returns(scope.Object);

        serviceProvider.Setup(s => s.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);

        return serviceProvider;
    }
}
