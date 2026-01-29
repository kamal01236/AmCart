using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using SearchService.Api.Clients;
using SearchService.Api.Options;
using SearchService.Api.Services;

namespace SearchService.Api.Config;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSearchServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SearchIndexOptions>()
            .Bind(configuration.GetSection("SearchIndex"))
            .ValidateDataAnnotations()
            .Validate(options => Uri.TryCreate(options.Endpoint, UriKind.Absolute, out _), "SearchIndex:Endpoint must be an absolute URI")
            .ValidateOnStart();

        services.AddScoped<ISearchService, Services.SearchService>();

        services.AddHttpClient<ISearchIndexClient, ElasticSearchIndexClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<SearchIndexOptions>>().Value;
                client.BaseAddress = new Uri(options.Endpoint);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrWhiteSpace(options.ApiKey))
                {
                    client.DefaultRequestHeaders.Remove("api-key");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("api-key", options.ApiKey);
                }
                else if (!string.IsNullOrWhiteSpace(options.Username))
                {
                    var password = options.Password ?? string.Empty;
                    var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.Username}:{password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt)));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}
