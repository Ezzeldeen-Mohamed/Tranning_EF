using MeterManagement.Application.DTOs.MeterDtos;
using MeterManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace MeterManagement.Application.Services.IService
{
    public interface IMeterService
    {
        Task<List<GetMeterDto>> GetAll();
        Task<GetMeterDto?> GetById(int id);
        Task Create(MeterDto dto);
        Task<List<string>> CreateBulk(List<MeterDto> dtos);
        Task<ImportResultDto> ImportFromExcel(IFormFile file);
        Task Update(int id, MeterDto dto);
        Task AssignMeterByEmail(AssignMeterDto dto);
        Task<List<GetMeterDto>> GetByUser(string userId);
        Task<List<GetMeterDto>> GetByStatus(MeterStatus status);
        Task InstallMeter(int meterId, string userId);
        Task Delete(int id);
        Task SoftDelete(int id);
    }
}
