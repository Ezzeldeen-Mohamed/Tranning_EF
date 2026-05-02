using MeterManagement.API.Models;
using MeterManagement.Domain.Enums;

namespace MeterManagement.Domain.IRepo
{
    public interface IMeterRepository
    {
        Task<List<Meter>> GetAll();
        Task<Meter?> GetById(int id); // ? momken id mykon4 mwgood fyrg3ly exption  
        Task<Meter?> GetBySerial(string serialNumber);
        Task<List<Meter>> GetBySerials(List<string> serials);
        Task Add(Meter meter);
        Task AddRange(List<Meter> meters);
        Task Update(Meter meter);
        Task<List<Meter>> GetByUserId(string userId);
        Task<List<Meter>> GetByStatus(MeterStatus status);
        Task Delete(Meter meter);
        Task SoftDelete(Meter meter);
        Task Save();
    }
}
