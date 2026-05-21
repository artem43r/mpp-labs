using StudyPlanner.Models;

namespace StudyPlanner.Services
{
    public interface ISubjectService
    {
        Task<IEnumerable<Subject>> GetUserSubjectsAsync(int userId);
        Task<Subject?> GetSubjectByIdAsync(int id, int userId);
        Task<Subject> CreateSubjectAsync(Subject subject, int userId);
        Task<Subject?> UpdateSubjectAsync(Subject subject, int userId);
        Task<bool> DeleteSubjectAsync(int id, int userId);
    }
}