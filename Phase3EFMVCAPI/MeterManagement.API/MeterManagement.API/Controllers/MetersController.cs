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
        public async Task<IActionResult> GetAll([FromQuery] MeterQueryParameters query)
        {
            var response = await _service.GetAll(query);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _service.GetById(id);

            return Ok(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(MeterDto dto)
        {
            var response = await _service.Create(dto);

            return Created(string.Empty, response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk(List<MeterDto> dtos)
        {
            var response = await _service.CreateBulk(dtos);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("my-meters")]
        public async Task<IActionResult> GetMyMeters()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _service.GetByUser(userId!);

            return Ok(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            var response = await _service.ImportFromExcel(file);

            return Ok(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MeterDto dto)
        {
            var response = await _service.Update(id, dto);

            return Ok(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);

            return NoContent();
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("soft/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var response = await _service.SoftDelete(id);

            return Ok(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var response = await _service.Restore(id);

            return Ok(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("assign-meter")]
        public async Task<IActionResult> AssignMeter(AssignMeterDto dto)
        {
            var response = await _service.AssignMeterByEmail(dto);

            return Ok(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(MeterStatus status)
        {
            var response = await _service.GetByStatus(status);

            return Ok(response);
        }

        [Authorize(Roles = Roles.Agent)]
        [HttpPost("install")]
        public async Task<IActionResult> Install(InstallMeterDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _service.InstallMeter(dto.MeterId, userId!);

            return Ok(response);
        }
    }
}