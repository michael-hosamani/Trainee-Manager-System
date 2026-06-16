using System.Data.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TraineeManagementApi.Models;
using TraineeManagementApi.Dto;
using TraineeManagementApi.Services;

namespace TraineeManagementApi.Services;

public class MentorService: IMentorService 
{
    private readonly AppDbContext _db;
    private readonly ILogger<MentorService> _logger;

    public MentorService(AppDbContext db, ILogger<MentorService> logger){
        _db = db;
        _logger = logger;
    }

    // This function returns the list of all the Mentors
    public async Task<List<Mentor>> GetAllMentors()
    {
        return await _db.Mentors
                        .Include(m => m.TaskAssignments)
                            .ThenInclude(t => t.Submissions)
                        .Include(m => m.Reviews)
                        .ToListAsync();
    }

    // This function fetches a Mentor based on its Id
    public async Task<Mentor?> GetMentorById(int id)
    {
        var result = await _db.Mentors
                                .Include(m => m.TaskAssignments)
                                    .ThenInclude(t => t.Submissions)
                                .Include(m => m.Reviews)
                                .SingleOrDefaultAsync(t => t.Id == id);
        if(result == null)
        {
            _logger.LogWarning("Mentor not found with {id}", id);
            return null;
        }

        return result;
    }

    // This funciton creates a new Mentor
    public async Task<MentorResponse> CreateMentor(CreateMentorRequest mentor)
    {

        Mentor newMentor = new Mentor
        {
            FirstName = mentor.FirstName,
            LastName = mentor.LastName,
            Email = mentor.Email,
            Status = mentor.Status,
            Expertise = mentor.Expertise,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };

        await _db.Mentors.AddAsync(newMentor);
        await _db.SaveChangesAsync();

        MentorResponse MentorResponse = new MentorResponse
        {   
            Id = newMentor.Id,
            FirstName = newMentor.FirstName,
            LastName = newMentor.LastName,
            Email = newMentor.Email,
            Status = newMentor.Status,
            Expertise = newMentor.Expertise,
            CreatedDate = newMentor.CreatedDate,
            UpdatedDate = newMentor.UpdatedDate
        };
        _logger.LogInformation("New Mentor created successfully");
        return MentorResponse;
    }

    // This function fetches the Mentor based on its Id and updates certain fields entered through the body
    public async Task<Mentor?> UpdateMentorDetails(int id, UpdateMentorRequest mentor)
    {
        Mentor? findMentor = await _db.Mentors.SingleOrDefaultAsync(t => t.Id == id);
        if(findMentor == null)
        {
            _logger.LogWarning("Mentor not found with {id}", id);
            return null;
        }

        if(mentor.FirstName != null)
            findMentor.FirstName = mentor.FirstName;
        
        if(mentor.LastName != null)
            findMentor.LastName = mentor.LastName;

        if(mentor.Email != null)
            findMentor.Email = mentor.Email;

        if(mentor.Expertise != null)
            findMentor.Expertise = mentor.Expertise;

        if(mentor.Status != null)
            findMentor.Status = mentor.Status.Value;

        findMentor.UpdatedDate = DateTime.Now;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Mentor updated successfully");

        return findMentor;
    }

    // This function fetches by Id and deletes a Mentor
    public async Task<bool> DeleteMentorDetails(int id)
    {
        Mentor? Mentor = await _db.Mentors.SingleOrDefaultAsync(t => t.Id == id);
        if(Mentor == null)
        {
            _logger.LogWarning("Mentor not found with {id}", id);
            return false;
        }

        _db.Mentors.Remove(Mentor);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Mentor deleted successfully");

        return true;
    }
}