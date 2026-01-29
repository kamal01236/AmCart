using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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

        services.AddOptions<KafkaOptions>()
            .Bind(configuration.GetSection("Messaging:Kafka"))
            .ValidateDataAnnotations()
            .Validate(options => !string.IsNullOrWhiteSpace(options.BootstrapServers), "Kafka bootstrap servers are required")
            .Validate(options => !string.IsNullOrWhiteSpace(options.OrderTopic), "Kafka order topic is required")
            .ValidateOnStart();

        services.AddSingleton<IProducer<string, string>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<KafkaOptions>>().Value;
            var config = new ProducerConfig
            {
                BootstrapServers = options.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true,
                MessageTimeoutMs = 30000
            };

            return new ProducerBuilder<string, string>(config).Build();
        });

        return services;
    }
}
