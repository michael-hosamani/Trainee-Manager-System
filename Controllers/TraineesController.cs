using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;
using TraineeManagementApi.Dto;

namespace TraineeManagementApi.Controllers;

// [Authorize(Roles = nameof(Role.Admin))]
[ApiController]
[Route("api/trainees")]
public class TraineesController: ControllerBase
{
    private ITraineeService service;

    public TraineesController(ITraineeService service)
    {
        this.service = service;
    }
    
    // GET /api/trainees
    /// <summary>
    /// Retrieves all the Trainees.
    /// </summary>
    /// <returns>All the Trainees.</returns>
    /// <response code="200">Returns all the trainees.</response>
    [HttpGet]
    public async Task<ActionResult> GetAllTrainees(string? search,[FromQuery] PaginationParams paginationParams, Status? status)
    {
        PagedResponse<Trainee> traineeData = await service.GetTraineeUsingPagination(paginationParams, search, status);
        return Ok(traineeData.Data);
    }

    // GET /api/trainees/{id}
    /// <summary>
    /// Retrieves a specific Trainee by ID.
    /// </summary>
    /// <param name="id">The ID of the Trainee to retrieve.</param>
    /// <returns>The requested Trainee.</returns>
    /// <response code="200">Returns the requested Trainee.</response>
    /// <response code="404">If the Trainee is not found.</response>
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
    /// <summary>
    /// Creates a new Trainee.
    /// </summary>
    /// <param name="trainee">Trainee details required for creation</param>
    /// <returns>The newly created Trainee.</returns>
    /// <response code="200">Returns the newly created Trainee.</response>
    [HttpPost]
    public async Task<ActionResult> CreateTrainee(CreateTraineeRequest trainee)
    {
        TraineeResponse traineeResponse = await service.CreateTrainee(trainee);

        return Ok(traineeResponse);
    }

    // PUT /api/trainees/{id}
    /// <summary>
    /// Update a specific Trainee by ID.
    /// </summary>
    /// <param name="id">The ID of the Trainee to retrieve.</param>
    /// <returns>The updated Trainee.</returns>
    /// <response code="200">Returns the updated Trainee.</response>
    /// <response code="404">If the Trainee is not found.</response>
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
    /// <summary>
    /// Delete a specific Trainee by ID.
    /// </summary>
    /// <param name="id">The ID of the Trainee to retrieve.</param>
    /// <returns>The deleted Trainee.</returns>
    /// <response code="200">Returns the deleted Trainee.</response>
    /// <response code="404">If the Trainee is not found.</response>
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

