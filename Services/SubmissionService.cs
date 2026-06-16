using System.Data.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TraineeManagementApi.Models;
using TraineeManagementApi.Dto;
using TraineeManagementApi.Services;

namespace TraineeManagementApi.Services;

public class SubmissionService: ISubmissionService 
{
    private readonly AppDbContext _db;
    private readonly ILogger<SubmissionService> _logger;

    public SubmissionService(AppDbContext db, ILogger<SubmissionService> logger){
        _db = db;
        _logger = logger;
    }

    // This function returns the list of all the Submissions
    public async Task<List<Submission>> GetAllSubmissions()
    {
        return await _db.Submissions
                            .Include(s => s.Reviews)
                            .ToListAsync();
    }

    // This function fetches a Submission based on its Id
    public async Task<Submission?> GetSubmissionById(int id)
    {
        var result = await _db.Submissions
                                .Include(s => s.Reviews)
                                .SingleOrDefaultAsync(t => t.Id == id);
        if(result == null)
        {
            _logger.LogWarning("Submissoin not found with {id}", id);
            return null;
        }
        return result;
    }

    // This funciton creates a new Submission
    public async Task<SubmissionResponse?> CreateSubmission(CreateSubmissionRequest submission)
    {
        TaskAssignment? findTaskAssignment = await _db.TaskAssignments.SingleOrDefaultAsync(t => t.Id == submission.TaskAssignmentId);
        if(findTaskAssignment == null)
        {
            _logger.LogWarning("Task ASsignment not found with {id}", submission.TaskAssignmentId);
            return null;
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
}