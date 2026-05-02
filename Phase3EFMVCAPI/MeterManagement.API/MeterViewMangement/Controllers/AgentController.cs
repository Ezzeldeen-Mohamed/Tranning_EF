using MeterViewMangement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeterViewMangement.Controllers
{

    [Authorize(Roles = "Agent")] // للأيجنب فقط
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

        // عرض العدادات الخاصة بالموظف الحالي فقط
        public async Task<IActionResult> MyMeters()
        {
            var client = _clientFactory.CreateClient("MyAPI");
            // الـ API دي اللي أنت عاملها: api/Meters/my-meters
            var response = await client.GetAsync("api/Meters/my-meters");

            if (response.IsSuccessStatusCode)
            {
                var meters = await response.Content.ReadFromJsonAsync<List<MeterViewModel>>();
                return View(meters);
            }

            return View(new List<MeterViewModel>());
        }

        // صفحة الـ Install (فورم بياخد بيانات العداد والعميل)
        [HttpGet]
        public IActionResult Install(int meterId)
        {
            var model = new InstallMeterViewModel { MeterId = meterId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmInstall(InstallMeterViewModel model)
        {
            var client = _clientFactory.CreateClient("MyAPI");
            var response = await client.PostAsJsonAsync("api/Meters/install", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Meter installed successfully!";
                return RedirectToAction("MyMeters");
            }

            ModelState.AddModelError("", "Failed to complete installation.");
            return View("Install", model);
        }
    }
}
