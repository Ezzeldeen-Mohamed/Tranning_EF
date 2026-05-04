using MeterManagement.API.Models;
using MeterManagement.Application.DTOs.MeterDtos;
using MeterManagement.Application.Exceptions;
using MeterManagement.Application.Services.IService;
using MeterManagement.Domain.Enums;
using MeterManagement.Domain.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace MeterManagement.Application.Services.Services
{
    public class MeterService : IMeterService
    {
        private readonly IMeterRepository _repo;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<MeterService> _logger;
        private readonly ILocalizationService _localize;
        private readonly IMemoryCache _cache;
        public MeterService(IMeterRepository repo, UserManager<User> userManager, ILogger<MeterService> logger, ILocalizationService localize, IMemoryCache cache)
        {
            _repo = repo;
            _userManager = userManager;
            _logger = logger;
            _localize = localize;
            _cache = cache;
        }

        public async Task<PagedResult<GetMeterDto>> GetAll(MeterQueryParameters query)
        {
            // 1. هنجيب الـ IQueryable من الـ Repo عشان ما ننفذش الكويري دلوقتي
            var queryable = _repo.GetQueryable();

            // 2. تطبيق الـ Filtering
            if (!string.IsNullOrEmpty(query.SerialNumber))
            {
                queryable = queryable.Where(m => m.SerialNumber.Contains(query.SerialNumber));
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                if (Enum.TryParse<MeterStatus>(query.Status, out var statusEnum))
                {
                    queryable = queryable.Where(m => m.Status == statusEnum);
                }
            }

            // 3. حساب العدد الكلي قبل الـ Pagination
            var totalCount = await queryable.CountAsync();

            // 4. تطبيق الـ Pagination (Skip & Take)
            var meters = await queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(m => new GetMeterDto
                {
                    Id = m.Id,
                    SerialNumber = m.SerialNumber,
                    Status = m.Status.ToString()
                })
                .ToListAsync();

            return new PagedResult<GetMeterDto>
            {
                Items = meters,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }
        public async Task InstallMeter(int meterId, string userId)
        {
            var meter = await _repo.GetById(meterId);

            if (meter == null)
            {
                _logger.LogWarning("Meter not found: {MeterId}", meterId);
                throw new BusinessException(_localize.GetString("GeneralError"), 404);
            }

            if (meter.Status != MeterStatus.Assigned)
            {
                _logger.LogWarning("Meter not assigned: {MeterId}", meterId);
                throw new BusinessException("Meter must be assigned first");
            }

            if (meter.UserId != userId)
            {
                _logger.LogWarning("Unauthorized installation attempt: {MeterId} by User: {UserId}", meterId, userId);
                throw new BusinessException("Not allowed");
            }

            meter.Status = MeterStatus.Installed;

            await _repo.Update(meter);
            await _repo.Save();
            _logger.LogInformation("Meter installed {MeterId}", meterId);
        }
        public async Task<GetMeterDto?> GetById(int id)
        {
            var meter = await _repo.GetById(id);

            if (meter == null)
            {
                _logger.LogWarning("Meter not found: {MeterId}", id);
                throw new BusinessException(_localize.GetString("MeterNotFound"), 404);
            }

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
            {
                _logger.LogWarning("Serial Number is required");
                throw new BusinessException("Serial Number is required");
            }

            var existingMeter = await _repo.GetBySerial(dto.SerialNumber);

            if (existingMeter != null)
            {
                _logger.LogWarning("Serial Number already exists: {SerialNumber}", dto.SerialNumber);
                throw new BusinessException("Serial Number already exists");
            }

            var meter = new Meter
            {
                SerialNumber = dto.SerialNumber,
                Status = MeterStatus.OnStock,

            };


            await _repo.Add(meter);
            await _repo.Save();
            _logger.LogInformation("Meter created {Serial}", dto.SerialNumber);
        }

        public async Task<List<string>> CreateBulk(List<MeterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                _logger.LogWarning("No meters provided for bulk creation");
                throw new BusinessException("No meters provided");
            }

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
                _logger.LogInformation("Bulk meters created successfully. Total meters: {MeterCount}", validMeters.Count);
            }
            _logger.LogInformation("Bulk meter creation completed. Rejected meters due to duplicates: {RejectedCount}", rejected.Count);
            return rejected;
        }

        public async Task<ImportResultDto> ImportFromExcel(IFormFile file)
        {
            var result = new ImportResultDto();
            var meters = new List<Meter>();
            var errors = new List<string>();

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Uploaded file is empty");
                throw new BusinessException(_localize.GetString("FileIsEmpty"));
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var sheet = package.Workbook.Worksheets.FirstOrDefault();

                    if (sheet == null)
                    {
                        _logger.LogWarning("No worksheet found in the uploaded Excel file");
                        throw new BusinessException(_localize.GetString("NoWorksheetFound"));
                    }

                    var rowCount = sheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var serial = sheet.Cells[row, 1].Value?.ToString();

                        if (string.IsNullOrWhiteSpace(serial))
                        {
                            errors.Add($"Row {row}: Empty serial");
                            _logger.LogWarning("Row {Row}: Empty serial number", row);
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
                    _logger.LogWarning("Duplicate serial number found in database: {SerialNumber}", meter.SerialNumber);
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
                _logger.LogInformation("Bulk meters created successfully. Total meters: {MeterCount}", validMeters.Count);
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
            {
                _logger.LogWarning("Meter not found for update: {MeterId}", id);
                throw new BusinessException(_localize.GetString("MeterNotFound"), 404);

            }
            meter.SerialNumber = dto.SerialNumber;

            await _repo.Update(meter);
            await _repo.Save();
            _logger.LogInformation("Meter updated: {MeterId}", id);
        }

        public async Task AssignMeterByEmail(AssignMeterDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", dto.Email);
                throw new BusinessException(_localize.GetString("UserNotFound"), 404);
            }

            var meter = await _repo.GetById(dto.MeterId);

            if (meter == null)
            {
                _logger.LogWarning("Meter not found for assignment: {MeterId}", dto.MeterId);
                throw new BusinessException(_localize.GetString("MeterNotFound"), 404);
            }

            if (meter.Status != MeterStatus.OnStock)
            {
                _logger.LogWarning("Meter not available for assignment: {MeterId}", dto.MeterId);
                throw new BusinessException(_localize.GetString("MeterAlreadyAssigned"));
            }


            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Agent"))
            {
                _logger.LogWarning("User is not an agent: {Email}", dto.Email);
                throw new BusinessException(_localize.GetString("UserNotAgent"));
            }

            meter.UserId = user.Id;
            meter.Status = MeterStatus.Assigned;

            await _repo.Update(meter);
            await _repo.Save();
            _logger.LogInformation("Meter {MeterId} assigned to {Email}", dto.MeterId, dto.Email);

        }
        public async Task<List<GetMeterDto>> GetByStatus(MeterStatus status)
        {
            var meters = await _repo.GetByStatus(status);

            return meters.Select(m => new GetMeterDto
            {
                Id = m.Id,
                SerialNumber = m.SerialNumber,
                Status = m.Status.ToString()
            }).ToList();
        }
        public async Task<List<GetMeterDto>> GetByUser(string userId)
        {
            var meters = await _repo.GetByUserId(userId);

            return meters.Select(m => new GetMeterDto
            {
                Id = m.Id,
                SerialNumber = m.SerialNumber,
                Status = m.Status.ToString()
            }).ToList();
        }
        public async Task Delete(int id)
        {
            var meter = await _repo.GetById(id);

            if (meter == null)
            {
                _logger.LogWarning("Meter not found for deletion: {MeterId}", id);
                throw new BusinessException(_localize.GetString("MeterNotFound"), 404);
            }

            await _repo.Delete(meter);
            await _repo.Save();
        }

        public async Task Restore(int id)
        {
            var meter = await _repo.GetById(id);
            if (meter == null) throw new BusinessException("Meter Not Found");

            await _repo.Restore(meter);
            await _repo.Save();
        }
        public async Task SoftDelete(int id)
        {
            var meter = await _repo.GetById(id);

            if (meter == null)
            {
                _logger.LogWarning("Meter not found for soft deletion: {MeterId}", id);
                throw new BusinessException(_localize.GetString("MeterNotFound"), 404);
            }

            await _repo.SoftDelete(meter);
            await _repo.Save();
            _logger.LogInformation("Meter soft deleted: {MeterId}", id);
        }
    }
}
