using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;
using TraineeManagementApi.Dto;

namespace TraineeManagementApi.Contollers;

[Authorize]
[ApiController]
[Route("api/learning-tasks")]
public class LearningTaskController: ControllerBase 
{
    private ILearningTaskService _service;

    public LearningTaskController(ILearningTaskService service)
    {
        _service = service;
    }

    // GET /api/LearningTasks
    [HttpGet]
    public async Task<ActionResult> GetAllLearningTasks()
    {
        throw new Exception("some error"); 
        // var learningTaskData = await _service.GetAllLearningTasks();
        // return Ok(learningTaskData);
    }

    // GET /api/LearningTasks/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetLearningTaskById(int id)
    {
        LearningTask? learningTask = await _service.GetLearningTaskById(id);
        if(learningTask == null)
        {
            return NotFound(new { message = "LearningTask not found" });
        }

        return Ok(learningTask);
    }   

    // POST /api/LearningTasks
    [HttpPost]
    public async Task<ActionResult> CreateLearningTask(CreateLearningTaskRequest learningTask)
    {
        LearningTaskResponse learningTaskResponse = await _service.CreateLearningTask(learningTask);

        return Ok(learningTaskResponse);
    }

    // PUT /api/LearningTasks/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateLearningTaskDetails(int id, UpdateLearningTaskRequest learningTask)
    {
        LearningTask? updatedLearningTaskDetails = await _service.UpdateLearningTaskDetails(id, learningTask);

        if(updatedLearningTaskDetails == null)
        {
            return NotFound(new { message = "Invalid LearningTask Id" });
        }

        return Ok(updatedLearningTaskDetails);
    }

    // DELETE /api/LearningTasks/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteLearningTask(int id)
    {
        bool isLearningTaskDeleted = await _service.DeleteLearningTaskDetails(id);
        if (isLearningTaskDeleted == false)
        {
            return NotFound(new { message = "Invalid LearningTask Data"});
        }
        return NoContent();
    }
}