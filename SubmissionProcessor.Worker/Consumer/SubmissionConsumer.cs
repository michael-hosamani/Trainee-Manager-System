using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Data;
using Shared.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Net.Http.Json;
using System.Security.Authentication;

namespace SubmissionProcessor.Worker.Consumer;
public class SubmissionConsumer : BackgroundService
{
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<SubmissionConsumer> _logger;
    private readonly HttpClient _httpClient;
    private readonly int maxRetries = 3;

    public SubmissionConsumer(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, ILogger<SubmissionConsumer> logger, HttpClient httpClient)
    {
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _httpClient = httpClient;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"], 
            VirtualHost = _configuration["RabbitMQ:VirtualHost"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]),
            UserName = _configuration["RabbitMQ:UserName"], 
            Password = _configuration["RabbitMQ:Password"] 
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        string queueName = _configuration["RabbitMQ:QueueName"];
        var dlx = $"{queueName}-dlx";
        var dlq = $"{queueName}-dlq";

        await _channel.ExchangeDeclareAsync(
            exchange: dlx,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false, 
            cancellationToken: cancellationToken
        );

        await _channel.QueueDeclareAsync(
            queue: dlq,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken
        );

        await _channel.QueueBindAsync(
            queue: dlq,
            exchange: dlx,
            routingKey: dlq,
            arguments: null,
            cancellationToken: cancellationToken
        );

        await _channel.QueueDeclareAsync(
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

        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: cancellationToken
        );

        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (ch, ea) =>
        {
            SubmissionProcessingRequested? res = null;
            try
            {
               var body = ea.Body.ToArray();
            
                var message = Encoding.UTF8.GetString(body);
                res = JsonSerializer.Deserialize<SubmissionProcessingRequested>(message);

                if(res == null)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    return;
                }

                using var scope = _serviceScopeFactory.CreateScope();
                AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                ProcessingJob? job = await db.ProcessingJobs.SingleOrDefaultAsync(p => p.CorrelationId == res.CorrelationId);
                if(job == null)
                {
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    _logger.LogWarning("Processing Job not found with Id : {id}", res.CorrelationId);
                    return;
                }
                // return if job already completed
                if(job.status == ProcessingJobStatus.Completed)
                {
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    _logger.LogWarning("Job Already Processed: {id}", job.Id);
                    return;
                }
                if(job.Attempts > maxRetries)
                {
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    _logger.LogWarning("Job processing attempts exceeded max retries: {id}", job.Id);
                    return;
                }

                UpdateStatus(res, ProcessingJobStatus.Processing);
                Console.WriteLine("Started processing");

                //simulate job processing 
                SubmissionFile? file = await db.SubmissionFiles.FindAsync(res.FileId);
                if (file == null)
                {
                    throw new Exception("File not found");
                }
                Console.WriteLine("Checksum: " + file.Checksum);

                DummyTrainee response = await _httpClient.GetFromJsonAsync<DummyTrainee>("api/trainees", new JsonSerializerOptions(JsonSerializerDefaults.Web));

                Console.WriteLine("response: " + response.ToString());

                await Task.Delay(TimeSpan.FromSeconds(3));

                // Acknowledge message
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

                await UpdateStatus(res, ProcessingJobStatus.Completed);
            }
            catch(Exception ex)
            {
                Console.WriteLine("in catch block");
                using var scope = _serviceScopeFactory.CreateScope();
                AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                ProcessingJob? job = await db.ProcessingJobs.SingleOrDefaultAsync(p => p.CorrelationId == res!.CorrelationId);

                if(job != null && job.Attempts > maxRetries)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    _logger.LogWarning("Max retry limit hit for Job: {id}", job.Id);
                    await UpdateStatus(res!, ProcessingJobStatus.Failed, ex.Message);
                    return;
                }
                
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                return;
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _configuration["RabbitMQ:QueueName"],
            autoAck: false, 
            consumer: consumer,
            cancellationToken: cancellationToken
        );
    }

    public async Task UpdateStatus(SubmissionProcessingRequested payload, ProcessingJobStatus status, string ErrorMessage = "default", CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        Guid CorrelationId = payload.CorrelationId;
        
        ProcessingJob? job = await db.ProcessingJobs.SingleOrDefaultAsync(p => p.CorrelationId == CorrelationId);

        if(job == null)
        {
            Console.WriteLine("Job not found");
            return;
        }

        if (status == ProcessingJobStatus.Processing)
        {
            _logger.LogInformation("Processing job: {id}", job.Id);

            job.status = status;
            job.Attempts++;
            job.StartedAt = DateTime.UtcNow;
        }
        if (status == ProcessingJobStatus.Completed)
        {
            job.status = status;
            job.CompletedAt = DateTime.UtcNow;
            _logger.LogInformation("Completed job: {id} ", job.Id);
        }
        if (status == ProcessingJobStatus.Failed)
        {
            job.status = status;
            job.ErrorSummary = ErrorMessage;
            _logger.LogInformation("Failed job: {id}", job.Id);
        }
        
        await db.SaveChangesAsync(cancellationToken);
    }
}