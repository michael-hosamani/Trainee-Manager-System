using System.Data.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Shared.Models;
using TraineeManagement.Api.Dto;
using TraineeManagement.Api.Services;
using System.Security.Cryptography;
using Shared.Data;
using System.Text.Json;

namespace TraineeManagement.Api.Services;

public class SubmissionService: ISubmissionService 
{
    private readonly AppDbContext _db;
    private readonly ILogger<SubmissionService> _logger;

    private readonly IFileStorageService _fileStorageService;
    private readonly IRedisCacheService _redisCacheService;
    private readonly IRabbitMQService _rabbitMQService;

    public SubmissionService(AppDbContext db, ILogger<SubmissionService> logger, IFileStorageService fileStorageService, IRedisCacheService redisCacheService, IRabbitMQService rabbitMQService){
        _db = db;
        _logger = logger;
        _fileStorageService = fileStorageService;
        _redisCacheService = redisCacheService;
        _rabbitMQService = rabbitMQService;
    }

    // This function returns the list of all the Submissions
    public async Task<List<Submission>> GetAllSubmissions()
    {
        return await _db.Submissions
                            .Include(s => s.Reviews)
                            .Include(s => s.SubmissionFiles)
                            .ToListAsync();
    }

    // This function fetches a Submission based on its Id
    public async Task<Submission?> GetSubmissionById(int id)
    {
        var result = await _db.Submissions
                                .Include(s => s.Reviews)
                                .Include(s => s.SubmissionFiles)
                                .SingleOrDefaultAsync(t => t.Id == id);
        if(result == null)
        {
            _logger.LogWarning("Submission not found with {id}", id);
            return null;
        }
        return result;
    }

    // This funciton creates a new Submission
    public async Task<SubmissionResponse> CreateSubmission(CreateSubmissionRequest submission)
    {
        TaskAssignment? findTaskAssignment = await _db.TaskAssignments.SingleOrDefaultAsync(t => t.Id == submission.TaskAssignmentId);
        if(findTaskAssignment == null)
        {
            _logger.LogWarning("Task Assignment not found with {id}", submission.TaskAssignmentId);
            throw new NotFoundException("Task Assignment not found", submission.TaskAssignmentId);
        }
        
        Submission newSubmission = new Submission
        {
            TaskAssignmentId = submission.TaskAssignmentId,
            SubmissionUrl = submission.SubmissionUrl,
            Notes = submission.Notes,
            Status = submission.Status,
            SubmissionDate = submission.SubmissionDate
        };

        await _db.Submissions.AddAsync(newSubmission);
        await _db.SaveChangesAsync();

        SubmissionResponse submissionResponse = new SubmissionResponse
        {   
            Id = newSubmission.Id,
            TaskAssignmentId = newSubmission.TaskAssignmentId,
            SubmissionUrl = newSubmission.SubmissionUrl,
            Notes = newSubmission.Notes,
            Status = newSubmission.Status,
            SubmissionDate = newSubmission.SubmissionDate
        };
        _logger.LogInformation("New Submission created successfully");
        return submissionResponse;
    }

    // This function is used to add submissionfile to the SubmissionFile model in the database and store the file using fileStorageService
    public async Task<string> UploadFile(int submissionId, CreateSubmissionFileRequest createSubmissionFileRequest, CancellationToken cancellationToken)
    {   
        Submission? findSubmission = await _db.Submissions.SingleOrDefaultAsync(s => s.Id == submissionId);
        if(findSubmission == null)
        {
            _logger.LogWarning("Submissions not found with {id}", submissionId);
            throw new NotFoundException("Submissions not found", submissionId);
        }

        string generatedPath = await _fileStorageService.SaveAsync(createSubmissionFileRequest.File, cancellationToken);

        string checksum = GetFileChecksum(generatedPath, "SHA256");

        SubmissionFile submissionFile = new SubmissionFile
        {
            OriginalFileName = createSubmissionFileRequest.File.FileName,
            GeneratedStorageName = generatedPath,
            SubmissionId = submissionId,
            UploadedByUser = createSubmissionFileRequest.UploadedByUser,
            ContentType = createSubmissionFileRequest.File.ContentType,
            Size = createSubmissionFileRequest.File.Length,
            Checksum = checksum,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };
        await _db.SubmissionFiles.AddAsync(submissionFile);
        await _db.SaveChangesAsync();
        SubmissionProcessingRequested submissionProcessingRequested = new()
        {
            MessageId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            SubmissionId = submissionId,
            FileId = submissionFile.Id,
            RequestedAt = DateTime.Now,
            ContractVersion = "1.0.0"
        };
        ProcessingJob processingJob = new ()
        {
            status = ProcessingJobStatus.Queued,
            Attempts = 0,
            ErrorSummary = "",
            CorrelationId = submissionProcessingRequested.CorrelationId  
        };
        await _db.ProcessingJobs.AddAsync(processingJob);
        await _db.SaveChangesAsync();
        await _rabbitMQService.PublishAsync(submissionProcessingRequested);
        
        _logger.LogInformation("Submission file created successfully");
        return generatedPath;
    }

    // This function fetches a Submission summary based on its Id
    public async Task<Submission?> GetSubmissionSummaryById(int id, CancellationToken cancellationToken)
    {
        string key = $"submission-summary:{id}";
        Submission? data = await _redisCacheService.GetAsync<Submission>(key, cancellationToken);
        if(data != null)
        {
            return data;
        }

        Submission? result = await _db.Submissions
                                .Include(s => s.Reviews)
                                .Include(s => s.SubmissionFiles)
                                .SingleOrDefaultAsync(t => t.Id == id);
        if(result == null)
        {
            _logger.LogWarning("Submissionn not found with {id}", id);
            return null;
        }

        await _redisCacheService.SetAsync(key, result, TimeSpan.FromMinutes(30), cancellationToken);

        return result;
    }

    /// <summary>
    /// Generates a checksum for a given file using the specified algorithm.
    /// Supported algorithms: MD5, SHA1, SHA256, SHA384, SHA512
    /// </summary>
    private static string GetFileChecksum(string filePath, string algorithmName)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            throw new FileNotFoundException("File not found.", filePath);

        using (var algorithm = HashAlgorithm.Create(algorithmName))
        {
            if (algorithm == null)
                throw new ArgumentException("Invalid hash algorithm name.");

            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = algorithm.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}