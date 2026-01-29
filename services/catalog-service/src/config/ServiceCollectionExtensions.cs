using Azure.Identity;
using CatalogService.Api.Domain;
using CatalogService.Api.Repositories;
using CatalogService.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

namespace CatalogService.Api.Config;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("Catalog") ??
                                   throw new InvalidOperationException("Catalog connection string missing");
            options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure());
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISearchIndexer, EventHubSearchIndexer>();

        services.AddAzureClients(clientBuilder =>
            {
                var eventHubNamespace = configuration["Messaging:EventHubs:Namespace"];
                var indexerHubName = configuration["Messaging:EventHubs:CatalogIndexer:Name"];
                if (!string.IsNullOrWhiteSpace(eventHubNamespace) && !string.IsNullOrWhiteSpace(indexerHubName))
                {
                    clientBuilder.AddEventHubProducerClientWithNamespace(eventHubNamespace, indexerHubName);
                }
            }, credentialFactory: _ => new DefaultAzureCredential());

        return services;
    }

    private static IAzureClientBuilder<Azure.Messaging.EventHubs.Producer.EventHubProducerClient, Azure.Messaging.EventHubs.Producer.EventHubProducerClientOptions>
        AddEventHubProducerClientWithNamespace(this AzureClientFactoryBuilder builder, string fullyQualifiedNamespace, string eventHubName)
    {
        return builder.AddEventHubProducerClient(eventHubName, options =>
        {
            options.ConnectionOptions = new Azure.Messaging.EventHubs.Producer.EventHubConnectionOptions
            {
                FullyQualifiedNamespace = fullyQualifiedNamespace
            };
        });
    }
}
