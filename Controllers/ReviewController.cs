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
public class ReviewsController: ControllerBase 
{
    private IReviewService _service;

    public ReviewsController(IReviewService service)
    {
        _service = service;
    }

    // GET /api/Reviews
    [HttpGet]
    public async Task<ActionResult> GetAllReviews()
    {
        var reviewData = await _service.GetAllReviews();
        return Ok(reviewData);
    }

    // GET /api/Reviews/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetReviewById(int id)
    {
        Review? review = await _service.GetReviewById(id);
        if(review == null)
        {
            return NotFound(new { message = "Review not found" });
        }

        return Ok(review);
    }   

    // POST /api/Reviews
    [HttpPost]
    public async Task<ActionResult> CreateReview(CreateReviewRequest review)
    {
        ReviewResponse reviewResponse = await _service.CreateReview(review);

        return Ok(reviewResponse);
    }
}