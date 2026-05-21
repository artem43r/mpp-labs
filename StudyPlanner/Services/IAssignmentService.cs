using StudyPlanner.Models;

namespace StudyPlanner.Services
{
    public interface IAssignmentService
    {
        Task<IEnumerable<Assignment>> GetUserAssignmentsAsync(int userId);
        Task<Assignment?> GetAssignmentByIdAsync(int id, int userId);
        Task<Assignment> CreateAssignmentAsync(Assignment assignment, int userId, int[] selectedTags);
        Task<Assignment?> UpdateAssignmentAsync(Assignment assignment, int userId, int[] selectedTags);
        Task<bool> DeleteAssignmentAsync(int id, int userId);
    }
}