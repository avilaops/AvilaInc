using Manager.Core.Entities.Companies;

namespace Manager.Infrastructure.Repositories;

public interface ICompanyRepository : IRepository<Company>
{
    Task<Company?> GetByGooglePlaceIdAsync(string googlePlaceId);
    Task<IEnumerable<Company>> GetBySourceAsync(string source);
    Task<IEnumerable<Company>> SearchAsync(string query, string? source = null);
    Task<bool> ExistsByGooglePlaceIdAsync(string googlePlaceId);
}