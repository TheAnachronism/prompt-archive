using FastEndpoints;

namespace Magnetar.Features.Health;

public class HealthCheckEndpoint : EndpointWithoutRequest<HealthCheckResponse>
{
    public override void Configure()
    {
        AllowAnonymous();
        Get("health");
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return SendAsync(new HealthCheckResponse
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        }, cancellation: ct);
    }
}

public class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}