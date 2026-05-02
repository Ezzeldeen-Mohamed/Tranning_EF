using MeterViewMangement.Helpers;
using MeterViewMangement.Models;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetAll()
        {
            // الحصول على التوكن المحفوظ
            var token = TokenStorage.Get(HttpContext);

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("https://localhost:7252/api/Meters"); // عنوان الـ API بتاعك

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var meters = JsonSerializer.Deserialize<List<MeterViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View("AllMeters", meters); // صفحة الـ View اللي هتعرض الجدول
            }

            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult GetMeter(int id)
        {
            return RedirectToAction("Details", new { id }); // صفحة الـ View اللي هتحتوي على الفورم
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
    }
}
