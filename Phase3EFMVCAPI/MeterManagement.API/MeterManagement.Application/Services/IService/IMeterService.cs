using MeterManagement.Application.DTOs.MeterDtos;
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
        Task Delete(int id);
        Task SoftDelete(int id);
    }
}
