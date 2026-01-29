using System.ComponentModel.DataAnnotations;

namespace OrderService.Api.Config;

public sealed class KafkaOptions
{
    [Required]
    public string BootstrapServers { get; set; } = string.Empty;

    [Required]
    public string OrderTopic { get; set; } = string.Empty;
}
