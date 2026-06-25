using RabbitMQ.Client;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Shared.Models;

namespace TraineeManagement.Api.Services;

public class RabbitMQService: IRabbitMQService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMQService> _logger;

    public RabbitMQService(IConfiguration configuration, ILogger<RabbitMQService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task PublishAsync(SubmissionProcessingRequested message, CancellationToken cancellationToken)
    {
        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"], 
            VirtualHost = _configuration["RabbitMQ:VirtualHost"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]),
            UserName = _configuration["RabbitMQ:UserName"], 
            Password = _configuration["RabbitMQ:Password"] 
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
 
        string queueName = _configuration["RabbitMQ:QueueName"];
        var dlx = $"{queueName}-dlx";
        var dlq = $"{queueName}-dlq";

        await channel.ExchangeDeclareAsync(
            exchange: dlx,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false, 
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync(
            queue: dlq,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync(
            queue: dlq,
            exchange: dlx,
            routingKey: dlq,
            arguments: null,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", dlx },
                { "x-dead-letter-routing-key", dlq } 
            },
            cancellationToken: cancellationToken
        );

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var properties = new BasicProperties{
            Persistent=true,
            MessageId=message.MessageId.ToString(),
            CorrelationId=message.CorrelationId.ToString(),
            ContentType="application/json",
            Type=nameof(SubmissionProcessingRequested)
        };

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: _configuration["RabbitMQ:QueueName"],
            mandatory: true,
            basicProperties: properties,
            body: body
        );

        _logger.LogInformation("RabbitMQ publish success. Message Id = {MessageId}, CorrelationId = {correlationId}, SubmissionId = {SubmissionId}, FileId = {FileId}",
            message.MessageId,
            message.CorrelationId,
            message.SubmissionId,
            message.FileId        
        );
    }
}