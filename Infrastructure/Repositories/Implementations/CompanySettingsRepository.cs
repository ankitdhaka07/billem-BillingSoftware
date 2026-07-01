using domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class CompanySettingsRepository : ICompanySettingsRepository
{
    private readonly AppDbContext _db;

    public CompanySettingsRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CompanySettings> GetAsync()
    {
        var existing = await _db.CompanySettings.AsNoTracking().FirstOrDefaultAsync();
        if (existing != null) return existing;

        // First run: seed a row from the built-in defaults.
        var seed = Seller.ToSettings();
        seed.Id = Guid.NewGuid();
        _db.CompanySettings.Add(seed);
        await _db.SaveChangesAsync();
        return seed;
    }

    public async Task SaveAsync(CompanySettings settings)
    {
        var existing = await _db.CompanySettings.FirstOrDefaultAsync();
        if (existing == null)
        {
            if (settings.Id == Guid.Empty) settings.Id = Guid.NewGuid();
            _db.CompanySettings.Add(settings);
        }
        else
        {
            existing.Name = settings.Name;
            existing.Address = settings.Address;
            existing.PhoneNumbers = settings.PhoneNumbers;
            existing.GstNo = settings.GstNo;
            existing.PANNo = settings.PANNo;
            existing.TINNo = settings.TINNo;
            existing.BankName = settings.BankName;
            existing.AccountNumber = settings.AccountNumber;
            existing.IFSCCode = settings.IFSCCode;
            existing.BranchLocation = settings.BranchLocation;
            existing.LogoPath = settings.LogoPath;
            existing.CgstPercentage = settings.CgstPercentage;
            existing.SgstPercentage = settings.SgstPercentage;
            existing.TermsAndConditions = settings.TermsAndConditions;
        }

        await _db.SaveChangesAsync();
    }
}
