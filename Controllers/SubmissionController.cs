using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using TraineeManagementApi.Models;
using TraineeManagementApi.Services;
using TraineeManagementApi.Dto;

namespace TraineeManagementApi.Contollers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SubmissionsController: ControllerBase 
{
    private ISubmissionService _service;

    public SubmissionsController(ISubmissionService service)
    {
        _service = service;
    }

    // GET /api/Submissions
    [HttpGet]
    public async Task<ActionResult> GetAllSubmissions()
    {
        var submissionData = await _service.GetAllSubmissions();
        return Ok(submissionData);
    }

    // GET /api/Submissions/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetSubmissionById(int id)
    {
        Submission? submission = await _service.GetSubmissionById(id);
        if(submission == null)
        {
            return NotFound(new { message = "Submission not found" });
        }

        return Ok(submission);
    }   

    // POST /api/Submissions
    [HttpPost]
    public async Task<ActionResult> CreateSubmission(CreateSubmissionRequest submission)
    {
        SubmissionResponse SubmissionResponse = await _service.CreateSubmission(submission);

        return Ok(SubmissionResponse);
    }
}