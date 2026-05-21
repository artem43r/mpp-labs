using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data;
using StudyPlanner.Models;

namespace StudyPlanner.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly ApplicationDbContext _context;

        public AssignmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Assignment>> GetUserAssignmentsAsync(int userId)
        {
            return await _context.Assignments
                .Include(a => a.Subject)
                .Include(a => a.AssignmentTags)
                    .ThenInclude(at => at.Tag)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Assignment?> GetAssignmentByIdAsync(int id, int userId)
        {
            return await _context.Assignments
                .Include(a => a.Subject)
                .Include(a => a.AssignmentTags)
                    .ThenInclude(at => at.Tag)
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        }

        public async Task<Assignment> CreateAssignmentAsync(
            Assignment assignment,
            int userId,
            int[] selectedTags)
        {
            assignment.UserId = userId;
            assignment.CreatedAt = DateTime.UtcNow;

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            if (selectedTags != null && selectedTags.Any())
            {
                foreach (var tagId in selectedTags)
                {
                    _context.AssignmentTags.Add(new AssignmentTag
                    {
                        AssignmentId = assignment.Id,
                        TagId = tagId
                    });
                }

                await _context.SaveChangesAsync();
            }

            return assignment;
        }

        public async Task<Assignment?> UpdateAssignmentAsync(
            Assignment assignment,
            int userId,
            int[] selectedTags)
        {
            var existingAssignment = await _context.Assignments
                .Include(a => a.AssignmentTags)
                .FirstOrDefaultAsync(a => a.Id == assignment.Id && a.UserId == userId);

            if (existingAssignment == null)
                return null;

            existingAssignment.Title = assignment.Title;
            existingAssignment.Description = assignment.Description;
            existingAssignment.Deadline = assignment.Deadline;
            existingAssignment.Status = assignment.Status;
            existingAssignment.Priority = assignment.Priority;
            existingAssignment.SubjectId = assignment.SubjectId;
            existingAssignment.Recurrence = assignment.Recurrence;

            existingAssignment.AssignmentTags.Clear();

            if (selectedTags != null && selectedTags.Any())
            {
                foreach (var tagId in selectedTags)
                {
                    existingAssignment.AssignmentTags.Add(new AssignmentTag
                    {
                        AssignmentId = assignment.Id,
                        TagId = tagId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return existingAssignment;
        }

        public async Task<bool> DeleteAssignmentAsync(int id, int userId)
        {
            var assignment = await _context.Assignments
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (assignment == null)
                return false;

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}