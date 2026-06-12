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
public class MentorsController: ControllerBase 
{
    private IMentorService _service;

    public MentorsController(IMentorService service)
    {
        _service = service;
    }

    // GET /api/mentors
    [HttpGet]
    public async Task<ActionResult> GetAllMentors()
    {
        var MentorData = await _service.GetAllMentors();
        return Ok(MentorData);
    }

    // GET /api/mentors/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetMentorById(int id)
    {
        Mentor? Mentor = await _service.GetMentorById(id);
        if(Mentor == null)
        {
            return NotFound(new { message = "Mentor not found" });
        }

        return Ok(Mentor);
    }   

    // POST /api/mentors
    [HttpPost]
    public async Task<ActionResult> CreateMentor(CreateMentorRequest Mentor)
    {
        MentorResponse MentorResponse = await _service.CreateMentor(Mentor);

        return Ok(MentorResponse);
    }

    // PUT /api/mentors/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateMentorDetails(int id, UpdateMentorRequest Mentor)
    {
        Mentor? updatedMentorDetails = await _service.UpdateMentorDetails(id, Mentor);

        if(updatedMentorDetails == null)
        {
            return NotFound(new { message = "Invalid Mentor Id" });
        }

        return Ok(updatedMentorDetails);
    }

    // DELETE /api/mentors/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMentor(int id)
    {
        bool isMentorDeleted = await _service.DeleteMentorDetails(id);
        if (isMentorDeleted == false)
        {
            return NotFound(new { message = "Invalid Mentor Data"});
        }
        return NoContent();
    }
}