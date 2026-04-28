using MeterManagement.Application.DTOs.MeterDtos;
using MeterManagement.Application.Services.IService;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var meter = await _service.GetById(id);

            if (meter == null)
                return NotFound();

            return Ok(meter);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MeterDto dto)
        {
            await _service.Create(dto);
            return Ok("Meter Created");
        }
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
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            var result = await _service.ImportFromExcel(file);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MeterDto dto)
        {
            await _service.Update(id, dto);
            return Ok("Updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.Delete(id);
            return Ok("Deleted");
        }
        [HttpDelete("Soft/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _service.SoftDelete(id);
            return Ok("Deleted");
        }
    }
}
