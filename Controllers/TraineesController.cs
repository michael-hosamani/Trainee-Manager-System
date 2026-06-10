using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Helpers;

namespace TraineeManagementApi.Controllers;

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
    public ActionResult GetAllTrainees(string? search,[FromQuery] PaginationParams paginationParams)
    {
        if(search == null)
        {
            // Task<List<Trainee>> allTrainees = service.GetAllTrainees();
            // return Ok(allTrainees.Result);
            Task<PagedResponse<Trainee>> traineeData = service.GetTraineeUsingPagination(paginationParams);
            return Ok(traineeData.Result.Data);
        }
        var result = service.SearchTrainees(search);

        return Ok(new
        {
            result.Result
        });
    }

    // GET /api/trainees/{id}
    [HttpGet("{id}")]
    public ActionResult GetTraineeById(int id)
    {
        Task<Trainee?> trainee = service.GetTraineeById(id);
        if(trainee.Result == null)
        {
            return NotFound(new { message = "Trainee not found" });
        }

        return Ok(trainee.Result);
    }   

    // POST /api/trainees
    [HttpPost]
    public ActionResult CreateTrainee(CreateTraineeRequest trainee)
    {
        Task<TraineeResponse> traineeResponse = service.CreateTrainee(trainee);

        return Ok(traineeResponse.Result);
    }

    // PUT /api/trainees/{id}
    [HttpPut("{id}")]
    public ActionResult UpdateTraineeDetails(int id, UpdateTraineeRequest trainee)
    {
        Task<Trainee?> updatedTraineeDetails = service.UpdateTraineeDetails(id, trainee);

        if(updatedTraineeDetails == null)
        {
            return NotFound(new { message = "Invalid Trainee Id" });
        }

        return Ok(updatedTraineeDetails.Result);
    }

    // DELETE /api/trainees/{id}
    [HttpDelete("{id}")]
    public ActionResult DeleteTrainee(int id)
    {
        Task<bool> isTraineeDeleted = service.DeleteTraineeDetails(id);
        if (isTraineeDeleted.Result == false)
        {
            return NotFound(new { message = "Invalid Trainee Data"});
        }
        return NoContent();
    }
}

