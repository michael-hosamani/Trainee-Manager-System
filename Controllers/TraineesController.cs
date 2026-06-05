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
    public ActionResult GetAllTrainees()
    {
        // GET /api/trainees
        List<Trainee> allTrainees = service.GetAllTrainees();
        return Ok(allTrainees);
    }

    [HttpGet("{id}")]
    public ActionResult GetTraineeById(int id)
    {
        // GET /api/trainees/{id}
        Trainee? trainee = service.GetTraineeById(id);
        if(trainee == null)
        {
            return NotFound(new { message = "Trainee not found" });
        }

        return Ok(trainee);
    }   

    [HttpPost]
    public ActionResult CreateTrainee(CreateTraineeRequest trainee)
    {
        // POST /api/trainees
        TraineeResponse traineeResponse = service.CreateTrainee(trainee);

        return Ok( new
        {
            newTrainee = traineeResponse
        });
    }

    [HttpPut("{id}")]
    public ActionResult UpdateTraineeDetails(int id, UpdateTraineeRequest trainee)
    {
        // PUT /api/trainees/{id}
        Trainee? updatedTraineeDetails = service.UpdateTraineeDetails(id, trainee);

        if(updatedTraineeDetails == null)
        {
            return NotFound(new { message = "Invalid Trainee Id" });
        }

        return Ok(updatedTraineeDetails);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteTrainee(int id)
    {
        // DELETE /api/trainees/{id}
        bool isTraineeDeleted = service.DeleteTraineeDetails(id);
        if (!isTraineeDeleted)
        {
            return NotFound(new { message = "Invalid Trainee Data"});
        }
        return NoContent();
    }
}