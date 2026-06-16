using Microsoft.AspNetCore.Mvc;

namespace TraineeManagementApi.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController: ControllerBase
{
    // GET /api/health route
    [HttpGet]
    public ActionResult Get(){
        return Ok(new { 
            status = "running", 
            application = "Trainee Management App", 
            timestamp = DateTime.Now 
        });
    }

}