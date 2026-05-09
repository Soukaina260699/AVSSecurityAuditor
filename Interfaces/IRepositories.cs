using AVSSecurityAuditor.Models;

namespace AVSSecurityAuditor.Interfaces
{
    public interface IAsvsRepository
    {
        Task<IEnumerable<AsvsChapter>> GetAllChaptersAsync();
        Task<AsvsChapter?> GetChapterByIdAsync(int id);
        Task<IEnumerable<AsvsRequirement>> GetAllRequirementsAsync();
        Task<IEnumerable<AsvsRequirement>> GetRequirementsByChapterAsync(int chapterId);
        Task<AsvsRequirement?> GetRequirementByIdAsync(int id);
        Task AddChapterAsync(AsvsChapter chapter);
        Task AddRequirementAsync(AsvsRequirement requirement);
        Task UpdateRequirementAsync(AsvsRequirement requirement);
        Task DeleteRequirementAsync(int id);
        Task<bool> SaveChangesAsync();
    }

    public interface IAssessmentRepository
    {
        Task<IEnumerable<Assessment>> GetUserAssessmentsAsync(string userId);
        Task<Assessment?> GetAssessmentByIdAsync(int id);
        Task<Assessment?> GetAssessmentWithItemsAsync(int id);
        Task AddAssessmentAsync(Assessment assessment);
        Task UpdateAssessmentAsync(Assessment assessment);
        Task DeleteAssessmentAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
