using MeterManagement.API.Models;
using MeterManagement.Application.DTOs.MeterDtos;
using MeterManagement.Application.Services.IService;
using MeterManagement.Domain.Enums;
using MeterManagement.Domain.IRepo;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace MeterManagement.Application.Services.Services
{
    public class MeterService : IMeterService
    {
        private readonly IMeterRepository _repo;

        public MeterService(IMeterRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<GetMeterDto>> GetAll()
        {
            var meters = await _repo.GetAll();

            return meters.Select(m => new GetMeterDto
            {
                Id = m.Id,
                SerialNumber = m.SerialNumber,
                Status = m.Status.ToString()
            }).ToList();
        }

        public async Task<GetMeterDto?> GetById(int id)
        {
            var meter = await _repo.GetById(id);

            if (meter == null)
                return null;

            return new GetMeterDto
            {
                Id = meter.Id,
                SerialNumber = meter.SerialNumber,
                Status = meter.Status.ToString()
            };
        }

        public async Task Create(MeterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SerialNumber))
                throw new Exception("Serial Number is required");

            var existingMeter = await _repo.GetBySerial(dto.SerialNumber);

            if (existingMeter != null)
                throw new Exception("Serial Number already exists");

            var meter = new Meter
            {
                SerialNumber = dto.SerialNumber,
                Status = MeterStatus.OnStock,

            };


            await _repo.Add(meter);
            await _repo.Save();
        }

        public async Task<List<string>> CreateBulk(List<MeterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                throw new Exception("No meters provided");

            var serials = dtos.Select(x => x.SerialNumber).ToList();

            var existingMeters = await _repo.GetBySerials(serials);
            var existingSerials = existingMeters.Select(x => x.SerialNumber).ToHashSet();

            var validMeters = dtos
                .Where(x => !existingSerials.Contains(x.SerialNumber))
                .Select(dto => new Meter
                {
                    SerialNumber = dto.SerialNumber,
                    Status = MeterStatus.OnStock
                })
                .ToList();

            var rejected = dtos
                .Where(x => existingSerials.Contains(x.SerialNumber))
                .Select(x => x.SerialNumber)
                .ToList();

            if (validMeters.Any())
            {
                await _repo.AddRange(validMeters);
                await _repo.Save();
            }

            return rejected;
        }

        public async Task<ImportResultDto> ImportFromExcel(IFormFile file)
        {
            var result = new ImportResultDto();
            var meters = new List<Meter>();
            var errors = new List<string>();

            if (file == null || file.Length == 0)
                throw new Exception("File is empty");

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var sheet = package.Workbook.Worksheets.FirstOrDefault();

                    if (sheet == null)
                        throw new Exception("No worksheet found"); var rowCount = sheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var serial = sheet.Cells[row, 1].Value?.ToString();

                        if (string.IsNullOrWhiteSpace(serial))
                        {
                            errors.Add($"Row {row}: Empty serial");
                            continue;
                        }

                        meters.Add(new Meter
                        {
                            SerialNumber = serial.Trim(),
                            Status = MeterStatus.OnStock
                        });
                    }
                }
            }

            // 🔍 check duplicates in DB
            var serials = meters.Select(m => m.SerialNumber).ToList();
            var existing = await _repo.GetBySerials(serials);

            var existingSerials = existing.Select(x => x.SerialNumber).ToHashSet();

            var validMeters = new List<Meter>();

            foreach (var meter in meters)
            {
                if (existingSerials.Contains(meter.SerialNumber))
                {
                    errors.Add($"Duplicate: {meter.SerialNumber}");
                    continue;
                }

                validMeters.Add(meter);
            }

            // 💾 save valid only
            if (validMeters.Any())
            {
                await _repo.AddRange(validMeters);
                await _repo.Save();
            }

            result.SuccessCount = validMeters.Count;
            result.FailedCount = errors.Count;
            result.Errors = errors;

            return result;
        }
        public async Task Update(int id, MeterDto dto)
        {
            var meter = await _repo.GetById(id);

            if (meter == null)
                return;

            meter.SerialNumber = dto.SerialNumber;

            await _repo.Update(meter);
            await _repo.Save();
        }

        public async Task Delete(int id)
        {
            var meter = await _repo.GetById(id);

            if (meter == null)
                return;

            await _repo.Delete(meter);
            await _repo.Save();
        }
        public async Task SoftDelete(int id)
        {
            var meter = await _repo.GetById(id);

            if (meter == null)
                return;

            await _repo.SoftDelete(meter);
            await _repo.Save();
        }
    }
}
