using MeterManagement.Domain.Enums;
using MeterManagement.Domain.Models;

namespace MeterManagement.Domain.IRepo
{
    public interface IMeterRepository
    {
        Task<List<Meter>> GetAll();
        Task<Meter?> GetById(int id);
        Task<Meter?> GetBySerial(string serialNumber);
        Task<List<Meter>> GetBySerials(List<string> serials);
        Task Add(Meter meter);
        Task AddRange(List<Meter> meters);
        Task Update(Meter meter);
        Task<List<Meter>> GetByUserId(string userId);
        Task<List<Meter>> GetByStatus(MeterStatus status);
        Task Restore(Meter meter);
        IQueryable<Meter> GetQueryable();
        Task Delete(Meter meter);
        Task SoftDelete(Meter meter);
        Task Save();
    }
}
