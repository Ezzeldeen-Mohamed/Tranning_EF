using MeterManagement.Application.Common;
using MeterManagement.Application.DTOs.MeterDtos;
using MeterManagement.Application.Exceptions;
using MeterManagement.Application.Resources;
using MeterManagement.Application.Services.IService;
using MeterManagement.Domain.Enums;
using MeterManagement.Domain.IRepo;
using MeterManagement.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace MeterManagement.Application.Services.Services
{
    /// <summary>
    /// Handles business logic related to meters.
    /// </summary>
    public class MeterService : IMeterService
    {
        private readonly IMeterRepository _repo;
        private readonly IAuthService _authService;
        private readonly ILogger<MeterService> _logger;
        private readonly ILocalizationService _localizer;

        public MeterService(
            IMeterRepository repo,
            IAuthService authService,
            ILogger<MeterService> logger,
            ILocalizationService localizer)
        {
            _repo = repo;
            _authService = authService;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<BaseResponse<PagedResult<GetMeterDto>>> GetAll(MeterQueryParameters query)
        {
            var queryable = _repo.GetQueryable();

            if (!string.IsNullOrWhiteSpace(query.SerialNumber))
            {
                queryable = queryable.Where(x =>
                    x.SerialNumber.Contains(query.SerialNumber));
            }

            if (!string.IsNullOrWhiteSpace(query.Status) &&
                Enum.TryParse<MeterStatus>(query.Status, true, out var status))
            {
                queryable = queryable.Where(x => x.Status == status);
            }

            var totalCount = await queryable.CountAsync();

            var meters = await queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new GetMeterDto
                {
                    Id = x.Id,
                    SerialNumber = x.SerialNumber,
                    Status = x.Status.ToString()
                })
                .ToListAsync();

            var result = new PagedResult<GetMeterDto>
            {
                Items = meters,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };

            return BaseResponse<PagedResult<GetMeterDto>>
                .Success(result);
        }

        public async Task<BaseResponse<GetMeterDto>> GetById(int id)
        {
            var meter = await _repo.GetById(id);

            if (meter == null)
            {
                throw new BusinessException(
                    _localizer.GetString("MeterNotFound"),
                    StatusCodes.Status404NotFound);
            }

            var dto = new GetMeterDto
            {
                Id = meter.Id,
                SerialNumber = meter.SerialNumber,
                Status = meter.Status.ToString()
            };

            return BaseResponse<GetMeterDto>.Success(dto);
        }

        public async Task<BaseResponse<bool>> Create(MeterDto dto)
        {
            var existingMeter = await _repo.GetBySerial(dto.SerialNumber);

            if (existingMeter != null)
            {
                throw new BusinessException(
                    "Serial number already exists",
                    StatusCodes.Status409Conflict);
            }

            var meter = new Meter
            {
                SerialNumber = dto.SerialNumber,
                Status = MeterStatus.OnStock
            };

            await _repo.Add(meter);
            await _repo.Save();

            _logger.LogInformation(
                "Meter created successfully: {Serial}",
                dto.SerialNumber);

            return BaseResponse<bool>
                .Success(true, "Meter created successfully");
        }

        public async Task<BaseResponse<List<string>>> CreateBulk(List<MeterDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                throw new BusinessException(
                    "No meters provided",
                    StatusCodes.Status400BadRequest);
            }

            var serials = dtos.Select(x => x.SerialNumber).ToList();

            var existingMeters = await _repo.GetBySerials(serials);

            var existingSerials = existingMeters
                .Select(x => x.SerialNumber)
                .ToHashSet();

            var validMeters = dtos
                .Where(x => !existingSerials.Contains(x.SerialNumber))
                .Select(x => new Meter
                {
                    SerialNumber = x.SerialNumber,
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

            return BaseResponse<List<string>>
                .Success(rejected, "Bulk operation completed");
        }

        public async Task<BaseResponse<ImportResultDto>> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BusinessException(
                    "File is empty",
                    StatusCodes.Status400BadRequest);
            }

            var result = new ImportResultDto();

            using var stream = new MemoryStream();

            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);

            var sheet = package.Workbook.Worksheets.FirstOrDefault();

            if (sheet == null)
            {
                throw new BusinessException(
                    "Worksheet not found",
                    StatusCodes.Status400BadRequest);
            }

            var meters = new List<Meter>();
            var errors = new List<string>();

            for (int row = 2; row <= sheet.Dimension.Rows; row++)
            {
                var serial = sheet.Cells[row, 1].Text?.Trim();

                if (string.IsNullOrWhiteSpace(serial))
                {
                    errors.Add($"Row {row}: Empty serial");
                    continue;
                }

                meters.Add(new Meter
                {
                    SerialNumber = serial,
                    Status = MeterStatus.OnStock
                });
            }

            var serials = meters.Select(x => x.SerialNumber).ToList();

            var existing = await _repo.GetBySerials(serials);

            var existingSerials = existing
                .Select(x => x.SerialNumber)
                .ToHashSet();

            var validMeters = meters
                .Where(x => !existingSerials.Contains(x.SerialNumber))
                .ToList();

            foreach (var duplicate in existingSerials)
            {
                errors.Add($"Duplicate: {duplicate}");
            }

            if (validMeters.Any())
            {
                await _repo.AddRange(validMeters);
                await _repo.Save();
            }

            result.SuccessCount = validMeters.Count;
            result.FailedCount = errors.Count;
            result.Errors = errors;

            return BaseResponse<ImportResultDto>
                .Success(result);
        }

        public async Task<BaseResponse<bool>> Update(int id, MeterDto dto)
        {
            var meter = await _repo.GetById(id);

            if (meter == null)
            {
                throw new BusinessException(
                    "Meter not found",
                    StatusCodes.Status404NotFound);
            }

            meter.SerialNumber = dto.SerialNumber;

            await _repo.Update(meter);
            await _repo.Save();

            return BaseResponse<bool>
                .Success(true, "Meter updated successfully");
        }

        public async Task<BaseResponse<bool>> Delete(int id)
        {
            var meter = await _repo.GetById(id);

            if (meter == null)
            {
                throw new BusinessException(
                    "Meter not found",
                    StatusCodes.Status404NotFound);
            }

            await _repo.Delete(meter);
            await _repo.Save();

            return BaseResponse<bool>
                .Success(true, "Meter deleted successfully");
        }

        public async Task<BaseResponse<bool>> SoftDelete(int id)
        {
            var meter = await _repo.GetById(id);

            if (meter == null)
            {
                throw new BusinessException(
                    "Meter not found",
                    StatusCodes.Status404NotFound);
            }

            await _repo.SoftDelete(meter);
            await _repo.Save();

            return BaseResponse<bool>
                .Success(true, "Meter soft deleted successfully");
        }

        public async Task<BaseResponse<bool>> Restore(int id)
        {
            var meter = await _repo.GetById(id);

            if (meter == null)
            {
                throw new BusinessException(
                    "Meter not found",
                    StatusCodes.Status404NotFound);
            }

            await _repo.Restore(meter);
            await _repo.Save();

            return BaseResponse<bool>
                .Success(true, "Meter restored successfully");
        }

        public async Task<BaseResponse<bool>> AssignMeterByEmail(AssignMeterDto dto)
        {
            var user = await _authService.GetByEmail(dto.Email);

            if (user == null)
            {
                throw new BusinessException(
                    "User not found",
                    StatusCodes.Status404NotFound);
            }

            var meter = await _repo.GetById(dto.MeterId);

            if (meter == null)
            {
                throw new BusinessException(
                    "Meter not found",
                    StatusCodes.Status404NotFound);
            }

            if (meter.Status != MeterStatus.OnStock)
            {
                throw new BusinessException(
                    "Meter already assigned",
                    StatusCodes.Status422UnprocessableEntity);
            }

            var roles = await _authService.GetRoles(user.Data.Id);

            if (!roles.Contains(Roles.Agent))
            {
                throw new BusinessException(
                    "User is not an agent",
                    StatusCodes.Status403Forbidden);
            }

            meter.UserId = user.Data.Id;
            meter.Status = MeterStatus.Assigned;

            await _repo.Update(meter);
            await _repo.Save();

            return BaseResponse<bool>
                .Success(true, "Meter assigned successfully");
        }

        public async Task<BaseResponse<List<GetMeterDto>>> GetByUser(string userId)
        {
            var meters = await _repo.GetByUserId(userId);

            var result = meters.Select(x => new GetMeterDto
            {
                Id = x.Id,
                SerialNumber = x.SerialNumber,
                Status = x.Status.ToString()
            }).ToList();

            return BaseResponse<List<GetMeterDto>>
                .Success(result);
        }

        public async Task<BaseResponse<List<GetMeterDto>>> GetByStatus(MeterStatus status)
        {
            var meters = await _repo.GetByStatus(status);

            var result = meters.Select(x => new GetMeterDto
            {
                Id = x.Id,
                SerialNumber = x.SerialNumber,
                Status = x.Status.ToString()
            }).ToList();

            return BaseResponse<List<GetMeterDto>>
                .Success(result);
        }

        public async Task<BaseResponse<bool>> InstallMeter(int meterId, string userId)
        {
            var meter = await _repo.GetById(meterId);

            if (meter == null)
            {
                throw new BusinessException(
                    "Meter not found",
                    StatusCodes.Status404NotFound);
            }

            if (meter.Status != MeterStatus.Assigned)
            {
                throw new BusinessException(
                    "Meter must be assigned first",
                    StatusCodes.Status422UnprocessableEntity);
            }

            if (meter.UserId != userId)
            {
                throw new BusinessException(
                    "Unauthorized installation",
                    StatusCodes.Status403Forbidden);
            }

            meter.Status = MeterStatus.Installed;

            await _repo.Update(meter);
            await _repo.Save();

            return BaseResponse<bool>
                .Success(true, "Meter installed successfully");
        }
    }
}