using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;
using TraineeManagementApi.Dto;

namespace TraineeManagementApi.Contollers;

[Authorize]
[ApiController]
[Route("api/task-assignments")]
public class TaskAssignmentController: ControllerBase 
{
    private ITaskAssignmentService _service;

    public TaskAssignmentController(ITaskAssignmentService service)
    {
        _service = service;
    }

    // GET /api/TaskAssignments
    [HttpGet]
    public async Task<ActionResult> GetAllTaskAssignments()
    {
        var taskAssignmentData = await _service.GetAllTaskAssignments();
        return Ok(taskAssignmentData);
    }

    // GET /api/TaskAssignments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetTaskAssignmentById(int id)
    {
        TaskAssignment? taskAssignment = await _service.GetTaskAssignmentById(id);
        if(taskAssignment == null)
        {
            return NotFound(new { message = "TaskAssignment not found" });
        }

        return Ok(taskAssignment);
    }   

    // POST /api/TaskAssignments
    [HttpPost]
    public async Task<ActionResult> CreateTaskAssignment(CreateTaskAssignmentRequest taskAssignment)
    {
        TaskAssignmentResponse? taskAssignmentResponse = await _service.CreateTaskAssignment(taskAssignment);

        if(taskAssignmentResponse == null)
        {
            return BadRequest();
        }

        return Ok(taskAssignmentResponse);
    }

    // PUT /api/TaskAssignments/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTaskAssignmentDetails(int id, TaskAssignmentStatus? status)
    {
        TaskAssignment? updatedTaskAssignmentDetails = await _service.UpdateTaskAssignmentDetails(id, status);

        if(updatedTaskAssignmentDetails == null)
        {
            return NotFound(new { message = "Invalid TaskAssignment Id" });
        }

        return Ok(updatedTaskAssignmentDetails);
    }
}