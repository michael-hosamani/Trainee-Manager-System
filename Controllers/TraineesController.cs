using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;
using TraineeManagementApi.Dto;

namespace TraineeManagementApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TraineesController: ControllerBase
{
    private ITraineeService service;

    public TraineesController(ITraineeService service)
    {
        this.service = service;
    }
    
    // GET /api/trainees
    [HttpGet]
    public async Task<ActionResult> GetAllTrainees(string? search,[FromQuery] PaginationParams paginationParams, Status? status)
    {
        PagedResponse<Trainee> traineeData = await service.GetTraineeUsingPagination(paginationParams, search, status);
        return Ok(traineeData.Data);
    }

    // GET /api/trainees/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetTraineeById(int id)
    {
        Trainee? trainee = await service.GetTraineeById(id);
        if(trainee == null)
        {
            return NotFound(new { message = "Trainee not found" });
        }

        return Ok(trainee);
    }   

    // POST /api/trainees
    [HttpPost]
    public async Task<ActionResult> CreateTrainee(CreateTraineeRequest trainee)
    {
        TraineeResponse traineeResponse = await service.CreateTrainee(trainee);

        return Ok(traineeResponse);
    }

    // PUT /api/trainees/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTraineeDetails(int id, UpdateTraineeRequest trainee)
    {
        Trainee? updatedTraineeDetails = await service.UpdateTraineeDetails(id, trainee);

        if(updatedTraineeDetails == null)
        {
            return NotFound(new { message = "Invalid Trainee Id" });
        }

        return Ok(updatedTraineeDetails);
    }

    // DELETE /api/trainees/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTrainee(int id)
    {
        bool isTraineeDeleted = await service.DeleteTraineeDetails(id);
        if (isTraineeDeleted == false)
        {
            return NotFound(new { message = "Invalid Trainee Data"});
        }
        return NoContent();
    }
}

