using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;

namespace Todo.Infrastructure.UnitTests.Shared.MockBuilders;

public static class ServiceScopeFactoryMockFactory
{
    public static Mock<IServiceScopeFactory> BuildServiceScopeFactoryContainingRequiredService<T>(T service)
        where T : notnull
    {
        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        var mockScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockServiceProvider.Setup(_ => _.GetService(typeof(T)))
            .Returns(service);

        mockScope.Setup(_ => _.ServiceProvider)
            .Returns(mockServiceProvider.Object);

        mockServiceScopeFactory.Setup(_ => _.CreateScope())
            .Returns(mockScope.Object);
        return mockServiceScopeFactory;
    }
}
