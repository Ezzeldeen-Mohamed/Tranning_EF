using MeterViewMangement.Helpers;
using MeterViewMangement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace MeterViewMangement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        public AdminController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("api");
        }
        public IActionResult Dashboard()
        {
            return View("AdminBoard");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("https://localhost:7252/api/Auth/users");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<UserViewModel>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View("AllUsers", users);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                TempData["ErrorMessage"] = "You don't have permission to view users.";
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login", "Auth");
        }

        // 1. عرض صفحة تفاصيل اليوزر وتغيير الدور
        [HttpGet]
        public async Task<IActionResult> Details(string email)
        {
            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // هنجيب اليوزر بالـ Email أو لو عندك أكشن GetByEmail في الـ API
            var response = await _httpClient.GetAsync($"https://localhost:7252/api/Auth/users");
            var content = await response.Content.ReadAsStringAsync();
            var allUsers = JsonSerializer.Deserialize<List<UserViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var user = allUsers.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            var viewModel = new ChangeRoleViewModel
            {
                Email = user.Email,
                FullName = user.FullName,
                CurrentRole = user.Role,
                NewRole = user.Role // القيمة الافتراضية في الـ Dropdown
            };

            return View("UserDetails", viewModel);
        }

        // 2. تنفيذ تغيير الدور
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(ChangeRoleViewModel model)
        {
            var token = TokenStorage.Get(HttpContext);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var dto = new { Email = model.Email, NewRole = model.NewRole };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7252/api/Auth/ChangeRole", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Role updated for {model.FullName} successfully!";
                return RedirectToAction("Index");
            }

            var error = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = error;
            return RedirectToAction("Details", new { email = model.Email });
        }
    }
}
