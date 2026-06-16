using System.Data.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TraineeManagementApi.Models;
using TraineeManagementApi.Dto;
using TraineeManagementApi.Services;

namespace TraineeManagementApi.Services;

public class ReviewService: IReviewService 
{
    private readonly AppDbContext _db;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(AppDbContext db, ILogger<ReviewService> logger){
        _db = db;
        _logger = logger;
    }

    // This function returns the list of all the Reviews
    public async Task<List<Review>> GetAllReviews()
    {
        return await _db.Reviews.ToListAsync();
    }

    // This function fetches a Review based on its Id
    public async Task<Review?> GetReviewById(int id)
    {
        var result = await _db.Reviews.SingleOrDefaultAsync(t => t.Id == id);
        if(result == null)
        {
            _logger.LogWarning("Review not found with {id}", id);
            return null;
        }
        return result;
    }

    // This funciton creates a new Review
    public async Task<ReviewResponse?> CreateReview(CreateReviewRequest review)
    {
        Submission? findSubmission = await _db.Submissions.SingleOrDefaultAsync(t => t.Id == review.SubmissionId);
        if(findSubmission == null)
        {
            _logger.LogWarning("Submission not found with {id}", review.SubmissionId);
            return null;
        }

        Mentor? findMentor = await _db.Mentors.SingleOrDefaultAsync(t => t.Id == review.MentorId);
        if(findMentor == null)
        {
            _logger.LogWarning("Mentor not found with {id}", review.MentorId);
            return null;
        }

        Review newReview = new Review
        {
            SubmissionId = review.SubmissionId,
            MentorId = review.MentorId,
            Feedback = review.Feedback,
            Status = review.Status,
            Score = review.Score,
            ReviewedDate = review.ReviewedDate
        };

        await _db.Reviews.AddAsync(newReview);
        await _db.SaveChangesAsync();

        ReviewResponse reviewResponse = new ReviewResponse
        {   
            Id = newReview.Id,
            SubmissionId = newReview.SubmissionId,
            MentorId = newReview.MentorId,
            Feedback = newReview.Feedback,
            Status = newReview.Status,
            Score = newReview.Score,
            ReviewedDate = newReview.ReviewedDate
        };
        _logger.LogInformation("New Review created successfully");
        return reviewResponse;
    }
}