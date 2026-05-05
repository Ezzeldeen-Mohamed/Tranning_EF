using MeterViewMangement.Helpers;
using MeterViewMangement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;
using System.Text.Json;


namespace MeterViewMangement.Controllers
{
    public class MeterController : Controller
    {
        private readonly HttpClient _httpClient;

        public MeterController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("api");
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, string serialNumber = "", string status = "")
        {
            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var url = $"https://localhost:7252/api/Meters?pageNumber={pageNumber}&pageSize=10&serialNumber={serialNumber}&status={status}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var pagedData = JsonSerializer.Deserialize<PagedMetersViewModel>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                pagedData.SerialFilter = serialNumber;
                pagedData.StatusFilter = status;

                return View("AllMeters", pagedData);
            }

            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult GetMeter(int id)
        {
            return RedirectToAction("Details", new { id });
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"https://localhost:7252/api/Meters/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Could not retrieve meter details. Please try again.";
                return RedirectToAction("GetAll");
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var meter = JsonSerializer.Deserialize<MeterViewModel>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View("MeterDetails", meter);
            }

            return RedirectToAction("GetAll");
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MeterCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7252/api/Meters/add-meter", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Done!";
                return RedirectToAction("GetAll");
            }
            if (!response.IsSuccessStatusCode)
            {
                var errors = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = errors;
                return RedirectToAction("Create");
            }
            var errorMsg = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", errorMsg);

            return View(model);
        }


        [HttpGet]
        public IActionResult CreateBulk() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBulk(MeterCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var serialsList = model.SerialNumber
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new { SerialNumber = s.Trim() })
                .ToList();

            if (!serialsList.Any())
            {
                ModelState.AddModelError("", "No valid serial numbers found.");
                return View(model);
            }

            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(serialsList);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7252/api/Meters/bulk", content);

            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                TempData["SuccessMessage"] = "Bulk processing completed!";
                return RedirectToAction("GetAll");
            }

            ModelState.AddModelError("", "Error communicating with API.");
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"https://localhost:7252/api/Meters/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var meter = JsonSerializer.Deserialize<MeterCreateViewModel>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(meter);
            }

            TempData["ErrorMessage"] = "Meter not found.";
            return RedirectToAction("GetAll");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, MeterCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"https://localhost:7252/api/Meters/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Meter updated successfully!";
                return RedirectToAction("GetAll");
            }

            var errorMsg = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"API Error: {errorMsg}");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid Excel file.";
                return RedirectToAction("GetAll");
            }

            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);

            content.Add(fileContent, "file", file.FileName);

            // 2. إرسال الـ Request
            var response = await _httpClient.PostAsync("https://localhost:7252/api/Meters/import-excel", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ImportResultDto>();
                if (result.Errors != null && result.Errors.Any())
                {
                    TempData["ImportErrors"] = result.Errors.ToArray();
                }

                TempData["SuccessMessage"] = $"Import Done! Success: {result.SuccessCount}, Failed: {result.FailedCount}";

                if (result.Errors.Any())
                    TempData["ImportErrors"] = result.Errors;

                return RedirectToAction("GetAll");
            }
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ImportResultDto>();
                TempData["SuccessMessage"] = $"Imported {result.SuccessCount} meters successfully.";

                if (result.FailedCount > 0)
                {
                    TempData["ImportErrors"] = string.Join("|", result.Errors);
                }
                return RedirectToAction("GetAll");
            }

            TempData["ErrorMessage"] = "Failed to upload file. Check API logs.";
            return RedirectToAction("GetAll");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmSoftDelete(int id)
        {
            var meter = await GetMeterById(id);
            if (meter == null)
            {
                TempData["ErrorMessage"] = "Meter not found or already deleted.";
                return RedirectToAction("GetAll");
            }
            return View(meter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExecuteSoftDelete(int id)
        {
            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"https://localhost:7252/api/Meters/Soft/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Meter moved to archive (Soft Deleted).";
            else
                TempData["ErrorMessage"] = "API Error: Could not deactivate meter.";

            return RedirectToAction("GetAll");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var meter = await GetMeterById(id);
            if (meter == null) return NotFound();
            return View(meter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExecuteDelete(int id)
        {
            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"https://localhost:7252/api/Meters/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Meter permanently removed.";
            else
                TempData["ErrorMessage"] = "API Error: Permanent delete failed.";

            return RedirectToAction("GetAll");
        }
        private async Task<DeleteMeterViewModel> GetMeterById(int id)
        {
            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // بننادي الـ API
            var response = await _httpClient.GetAsync($"https://localhost:7252/api/Meters/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DeleteMeterViewModel>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return null;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // الـ API بيتوقع POST
            var response = await _httpClient.PostAsync($"https://localhost:7252/api/Meters/Restore/{id}", null);

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Meter restored successfully!";
            else
                TempData["ErrorMessage"] = "Could not restore meter. Check if it exists.";

            return RedirectToAction("GetAll");
        }


        [HttpGet]
        public async Task<IActionResult> Assign(int id, string serialNumber)
        {
            var model = new AssignMeterViewModel
            {
                MeterId = id,
                SerialNumber = serialNumber
            };
            return View("~/Views/Admin/Assign.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignMeterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var dto = new { MeterId = model.MeterId, Email = model.Email };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7252/api/Meters/assign-meter", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Meter assigned to agent successfully!";
                return RedirectToAction("GetAll");
            }

            var errorMsg = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = errorMsg;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyMeters()
        {
            var token = TokenStorage.Get(HttpContext);
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://localhost:7252/api/Meters/my-meters");

            List<GetAssignedMetersViewModel> meters = new();

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                meters = JsonSerializer.Deserialize<List<GetAssignedMetersViewModel>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to load your meters.";
            }

            return View("~/Views/Agent/MyMeters.cshtml", meters);
        }

        public async Task<List<GetAssignedMetersViewModel>> GetMyMetersAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("https://localhost:7252/api/Meters/my-meters");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<GetAssignedMetersViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return new List<GetAssignedMetersViewModel>();
        }
    }
}
