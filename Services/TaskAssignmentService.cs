using System.Data.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TraineeManagementApi.Models;
using TraineeManagementApi.Dto;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace TraineeManagementApi.Services;

public class TaskAssignmentService: ITaskAssignmentService 
{
    private readonly AppDbContext _db;
    private readonly ILogger<TaskAssignmentService> _logger;

    public TaskAssignmentService(AppDbContext db, ILogger<TaskAssignmentService> logger){
        _db = db;
        _logger = logger;
    }

    // This function returns the list of all the TaskAssignments
    public async Task<List<TaskAssignment>> GetAllTaskAssignments()
    {
        return await _db.TaskAssignments
                            .Include(t => t.Submissions)
                                .ThenInclude(s => s.Reviews)
                            .ToListAsync();
    }

    // This function fetches a TaskAssignment based on its Id
    public async Task<TaskAssignment?> GetTaskAssignmentById(int id)
    {
        var result = await _db.TaskAssignments
                                    .Include(t => t.Submissions)
                                        .ThenInclude(s => s.Reviews)
                                    .SingleOrDefaultAsync(t => t.Id == id);
        if(result == null)
        {
            _logger.LogWarning("Task Assignment not found with {id}", id);
            return null;
        }
        return result;
    }

    // This funciton creates a new TaskAssignment 
    public async Task<TaskAssignmentResponse?> CreateTaskAssignment(CreateTaskAssignmentRequest taskAssignment)
    {   
        Trainee? findTrainee = await _db.Trainees.SingleOrDefaultAsync(t => t.Id == taskAssignment.TraineeId);
        if(findTrainee == null)
        {
            _logger.LogWarning("Trainee not found with {id}", taskAssignment.TraineeId);
            return null;
        }

        Mentor? findMentor = await _db.Mentors.SingleOrDefaultAsync(t => t.Id == taskAssignment.MentorId);
        if(findMentor == null)
        {
            _logger.LogWarning("Mentor not found with {id}", taskAssignment.MentorId);
            return null;
        }

        LearningTask? findLearningTask = await _db.LearningTasks.SingleOrDefaultAsync(t => t.Id == taskAssignment.LearningTaskId);
        if(findLearningTask == null)
        {
            _logger.LogWarning("Learning Task not found with {id}", taskAssignment.LearningTaskId);
            return null;
        }

        if(taskAssignment.DueDate < taskAssignment.AssignedDate)
        {
            return null;
        }

        TaskAssignment newTaskAssignment = new TaskAssignment
        {
            TraineeId = taskAssignment.TraineeId,
            MentorId = taskAssignment.MentorId,
            LearningTaskId = taskAssignment.LearningTaskId,
            Status = taskAssignment.Status,
            AssignedDate = taskAssignment.AssignedDate,
            DueDate = taskAssignment.DueDate,
        };

        await _db.TaskAssignments.AddAsync(newTaskAssignment);
        await _db.SaveChangesAsync();

        TaskAssignmentResponse TaskAssignmentResponse = new TaskAssignmentResponse
        {   
            TraineeId = newTaskAssignment.TraineeId,
            MentorId = newTaskAssignment.MentorId,
            LearningTaskId = newTaskAssignment.LearningTaskId,
            Status = newTaskAssignment.Status,
            AssignedDate = newTaskAssignment.AssignedDate,
            DueDate = newTaskAssignment.DueDate,
        };
        _logger.LogInformation("New TaskAssignment created successfully");
        return TaskAssignmentResponse;
    }

    // This function fetches the TaskAssignment based on its Id and updates certain fields entered through the body
    public async Task<TaskAssignment?> UpdateTaskAssignmentDetails(int id, [FromBody] UpdateTaskAssignmentRequest updateTaskAssignmentRequest)
    {
        var findTaskAssignment = await _db.TaskAssignments.SingleOrDefaultAsync(t => t.Id == id);
        if(findTaskAssignment == null)
        {
            _logger.LogWarning("Task Assignment not found with {id}", id);
            return null;
        }

        if(updateTaskAssignmentRequest.Status.HasValue)
            findTaskAssignment.Status = updateTaskAssignmentRequest.Status.Value;

        await _db.SaveChangesAsync();

        _logger.LogInformation("TaskAssignment updated successfully");

        return findTaskAssignment;
    }
}