using MeterManagement.Application.DTOs.MeterDtos;
using MeterManagement.Application.Services.IService;
using MeterManagement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MeterManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetersController : ControllerBase
    {
        private readonly IMeterService _service;

        public MetersController(IMeterService service)
        {
            _service = service;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var meter = await _service.GetById(id);
            return Ok(meter);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(MeterDto dto)
        {
            await _service.Create(dto);
            return Ok("Meter Created");
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk(List<MeterDto> dtos)
        {
            var rejected = await _service.CreateBulk(dtos);

            return Ok(new
            {
                message = "Bulk processed",
                rejectedSerials = rejected
            });
        }

        [Authorize]
        [HttpGet("my-meters")]
        public async Task<IActionResult> GetMyMeters()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _service.GetByUser(userId);

            return Ok(result);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            var result = await _service.ImportFromExcel(file);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin)]

        public async Task<IActionResult> Update(int id, MeterDto dto)
        {
            await _service.Update(id, dto);
            return Ok("Updated");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]

        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok("Deleted");
        }
        [HttpDelete("Soft/{id}")]
        [Authorize(Roles = Roles.Admin)]

        public async Task<IActionResult> SoftDelete(int id)
        {
            await _service.SoftDelete(id);
            return Ok("Deleted");
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("assign-meter")]
        public async Task<IActionResult> AssignByEmail(AssignMeterDto dto)
        {
            await _service.AssignMeterByEmail(dto);
            return Ok("Meter assigned successfully");

        }
        [Authorize(Roles = Roles.Admin)]
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(MeterStatus status)
        {
            var result = await _service.GetByStatus(status);
            return Ok(result);
        }
        [Authorize(Roles = Roles.Agent)]
        [HttpPost("install")]
        public async Task<IActionResult> Install(InstallMeterDto dto)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            await _service.InstallMeter(dto.MeterId, userId);
            return Ok(new
            {
                message = "Installed successfully",
                meterId = dto.MeterId
            });
        }



    }
}
