using CommentsApp.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CommentsApp.Infrastructure.Queue;

public sealed class QueuedHostedService(
    IBackgroundTaskQueue queue,
    IServiceScopeFactory scopeFactory,
    ILogger<QueuedHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[Queue] Background task worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await queue.DequeueAsync(stoppingToken);

                // Each task gets its own DI scope (so scoped services like DbContext work)
                using var scope = scopeFactory.CreateScope();
                await workItem(scope.ServiceProvider, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break; // Graceful shutdown
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Queue] Error executing background work item");
            }
        }
    }
}