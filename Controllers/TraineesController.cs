using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class TraineesController: ControllerBase
{
    private ITraineeService service;

    public TraineesController(ITraineeService service)
    {
        this.service = service;
    }
    
    [HttpGet]
    public ActionResult GetAllTrainees(string? search)
    {
        if(search == null)
        {
            // GET /api/trainees
            Task<List<Trainee>> allTrainees = service.GetAllTrainees();
            return Ok(allTrainees.Result);
        }
        var result = service.SearchTrainees(search);

        return Ok(new
        {
            result.Result
        });
    }

    [HttpGet("{id}")]
    public ActionResult GetTraineeById(int id)
    {
        // GET /api/trainees/{id}
        Task<Trainee?> trainee = service.GetTraineeById(id);
        if(trainee == null)
        {
            return NotFound(new { message = "Trainee not found" });
        }

        return Ok(trainee.Result);
    }   

    [HttpPost]
    public ActionResult CreateTrainee(CreateTraineeRequest trainee)
    {
        // POST /api/trainees
        Task<TraineeResponse> traineeResponse = service.CreateTrainee(trainee);

        return Ok(traineeResponse.Result);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateTraineeDetails(int id, UpdateTraineeRequest trainee)
    {
        // PUT /api/trainees/{id}
        Task<Trainee?> updatedTraineeDetails = service.UpdateTraineeDetails(id, trainee);

        if(updatedTraineeDetails == null)
        {
            return NotFound(new { message = "Invalid Trainee Id" });
        }

        return Ok(updatedTraineeDetails.Result);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteTrainee(int id)
    {
        // DELETE /api/trainees/{id}
        Task<bool> isTraineeDeleted = service.DeleteTraineeDetails(id);
        if (isTraineeDeleted.Result == false)
        {
            return NotFound(new { message = "Invalid Trainee Data"});
        }
        return NoContent();
    }
}

