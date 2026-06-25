using Shared.Models;

namespace TraineeManagement.Api.Services;

public interface IRabbitMQService
{
    Task PublishAsync(SubmissionProcessingRequested message, CancellationToken cancellationToken);
}