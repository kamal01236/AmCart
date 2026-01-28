using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using OrderService.Api.Domain;
using OrderService.Api.Repositories;
using OrderService.Api.Services;

namespace OrderService.Api.Config;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("Orders") ??
                                   throw new InvalidOperationException("Orders connection string missing");
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderProcessingService, OrderProcessingService>();

        services.AddAzureClients(builder =>
        {
            var eventHubNamespace = configuration["Messaging:EventHubs:Namespace"];
            if (!string.IsNullOrWhiteSpace(eventHubNamespace))
            {
                builder.AddEventHubProducerClientWithNamespace(eventHubNamespace);
            }
        }, credentialFactory: _ => new DefaultAzureCredential());

        return services;
    }

    private static IAzureClientBuilder<Azure.Messaging.EventHubs.Producer.EventHubProducerClient, Azure.Messaging.EventHubs.Producer.EventHubProducerClientOptions>
        AddEventHubProducerClientWithNamespace(this AzureClientFactoryBuilder builder, string fullyQualifiedNamespace)
    {
        return builder.AddEventHubProducerClientWithNamespace(fullyQualifiedNamespace, new Azure.Messaging.EventHubs.Producer.EventHubProducerClientOptions());
    }
}
