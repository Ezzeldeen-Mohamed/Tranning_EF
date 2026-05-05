using MeterViewMangement.Helpers;
using MeterViewMangement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace MeterViewMangement.Controllers
{

    [Authorize(Roles = "Agent")]
    public class AgentController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AgentController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IActionResult Dashboard()
        {
            return View("AgentBoard");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Install(int meterId)
        {
            var token = TokenStorage.Get(HttpContext);
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var dto = new { MeterId = meterId };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7252/api/Meters/install", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Meter has been installed successfully!";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = "Installation failed: " + error;
            }

            return RedirectToAction("MyMeters");
        }
    }
}
