using System.Data.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TraineeManagementApi.Models;
using TraineeManagementApi.Dto;
using TraineeManagementApi.Services;

namespace TraineeManagementApi.Services;

public class LearningTaskService: ILearningTaskService 
{
    private readonly AppDbContext _db;
    private readonly ILogger<LearningTaskService> _logger;

    public LearningTaskService(AppDbContext db, ILogger<LearningTaskService> logger){
        _db = db;
        _logger = logger;
    }

    // This function returns the list of all the LearningTasks
    public async Task<List<LearningTask>> GetAllLearningTasks()
    {
        return await _db.LearningTasks
                            .Include(l => l.TaskAssignments)
                                .ThenInclude(t => t.Submissions)
                                    .ThenInclude(s => s.Reviews)
                            .ToListAsync();
    }

    // This function fetches a LearningTask based on its Id
    public async Task<LearningTask?> GetLearningTaskById(int id)
    {
        var result = await _db.LearningTasks
                                .Include(l => l.TaskAssignments)       
                                    .ThenInclude(t => t.Submissions) 
                                        .ThenInclude(s => s.Reviews)        
                                .SingleOrDefaultAsync(t => t.Id == id);
        if(result == null)
        {
            _logger.LogWarning("Learning Task not found with {id}", id);
            return null;
        }
        return result;
    }

    // This funciton creates a new LearningTask
    public async Task<LearningTaskResponse> CreateLearningTask(CreateLearningTaskRequest learningTask)
    {

        LearningTask newLearningTask = new LearningTask
        {
            Title = learningTask.Title,
            Description = learningTask.Description,
            ExpectedTechStack = learningTask.ExpectedTechStack,
            DueDate = learningTask.DueDate,
            Status = learningTask.Status,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };

        await _db.LearningTasks.AddAsync(newLearningTask);
        await _db.SaveChangesAsync();

        LearningTaskResponse learningTaskResponse = new LearningTaskResponse
        {   
            Id = newLearningTask.Id,
            Title = newLearningTask.Title,
            Description = newLearningTask.Description,
            ExpectedTechStack = newLearningTask.ExpectedTechStack,
            Status = newLearningTask.Status,
            DueDate = newLearningTask.DueDate,
            CreatedDate = newLearningTask.CreatedDate,
            UpdatedDate = newLearningTask.UpdatedDate
        };
        _logger.LogInformation("New LearningTask created successfully");
        return learningTaskResponse;
    }

    // This function fetches the LearningTask based on its Id and updates certain fields entered through the body
    public async Task<LearningTask?> UpdateLearningTaskDetails(int id, UpdateLearningTaskRequest learningTask)
    {
        var findLearningTask = await _db.LearningTasks.SingleOrDefaultAsync(t => t.Id == id);
        if(findLearningTask == null)
        {
            _logger.LogWarning("Learning Task not found with {id}", id);
            return null;
        }

        if(learningTask.Title != null)
            findLearningTask.Title = learningTask.Title;
        
        if(learningTask.Description != null)
            findLearningTask.Description = learningTask.Description;

        if(learningTask.ExpectedTechStack != null)
            findLearningTask.ExpectedTechStack = learningTask.ExpectedTechStack;

        if(learningTask.DueDate.HasValue)
            findLearningTask.DueDate = learningTask.DueDate.Value;

        if(learningTask.Status.HasValue)
            findLearningTask.Status = learningTask.Status.Value;

        findLearningTask.UpdatedDate = DateTime.Now;

        await _db.SaveChangesAsync();

        _logger.LogInformation("LearningTask updated successfully");

        return findLearningTask;
    }

    // This function fetches by Id and deletes a LearningTask
    public async Task<bool> DeleteLearningTaskDetails(int id)
    {
        var learningTask = await _db.LearningTasks.SingleOrDefaultAsync(t => t.Id == id);
        if(learningTask == null)
        {
            _logger.LogWarning("Learning Task not found with {id}", id);
            return false;
        }

        _db.LearningTasks.Remove(learningTask);
        await _db.SaveChangesAsync();

        _logger.LogInformation("LearningTask deleted successfully");

        return true;
    }
}