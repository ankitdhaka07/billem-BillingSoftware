using domain;

namespace Infrastructure;

public interface ICompanySettingsRepository
{
    /// <summary>Returns the single settings row, seeding it from defaults if none exists.</summary>
    Task<CompanySettings> GetAsync();

    /// <summary>Persists the settings (updates the existing row, or inserts if missing).</summary>
    Task SaveAsync(CompanySettings settings);
}
