using MeterManagement.API.Models;
using MeterManagement.Domain.IRepo;
using MeterManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeterManagement.Infrastructure.Repo
{
    public class MeterRepository : IMeterRepository
    {
        private readonly AppDbContext _context;

        public MeterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Meter>> GetAll()
        {
            return await _context.Meters
                .Where(m => !m.IsDeleted)
                .ToListAsync();
        }

        public async Task<Meter?> GetById(int id)
        {
            return await _context.Meters
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }
        public async Task<Meter?> GetBySerial(string serialNumber)
        {
            return await _context.Meters
                .FirstOrDefaultAsync(m => m.SerialNumber == serialNumber && !m.IsDeleted);
        }
        public async Task Add(Meter meter)
        {
            await _context.Meters.AddAsync(meter);
        }

        public async Task AddRange(List<Meter> meters)
        {
            await _context.Meters.AddRangeAsync(meters);
        }

        public async Task<List<Meter>> GetBySerials(List<string> serials)
        {
            return await _context.Meters
                .Where(m => serials.Contains(m.SerialNumber))
                .ToListAsync();
        }

        public async Task Update(Meter meter)
        {
            _context.Meters.Update(meter);
        }

        public async Task Delete(Meter meter)
        {
            _context.Meters.Remove(meter);
        }
        public Task SoftDelete(Meter meter)
        {
            meter.IsDeleted = true;
            meter.DeletedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
