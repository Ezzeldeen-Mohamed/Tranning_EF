using MeterManagement.API.Models;

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
        Task Delete(Meter meter);
        Task SoftDelete(Meter meter);
        Task Save();
    }
}
