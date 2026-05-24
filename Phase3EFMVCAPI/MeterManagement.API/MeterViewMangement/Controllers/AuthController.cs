using MeterViewMangement.Helpers;
using MeterViewMangement.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace MeterViewMangement.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        public AuthController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("api");
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin")) return RedirectToAction("Dashboard", "Admin");
                if (User.IsInRole("Agent")) return RedirectToAction("Dashboard", "Agent");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> LoginFun(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Login", model);

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("https://localhost:7252/api/Auth/login", content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Login failed: Email or Password incorrect");
                return View("Login", model);
            }

            var tokenObj = JsonSerializer.Deserialize<JsonElement>(result);
            var token = tokenObj.GetProperty("data").GetString();

            TokenStorage.Save(HttpContext, token);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = jwtToken.Claims.ToList();

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            var roles = claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                              .Select(c => c.Value).ToList();

            if (roles.Contains("Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else if (roles.Contains("Agent"))
            {
                return RedirectToAction("Dashboard", "Agent");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }
        [HttpPost]
        public async Task<IActionResult> RegisterFun(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Register", model);

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("https://localhost:7252/api/Auth/register", content);

            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var errorObj = JsonSerializer.Deserialize<JsonElement>(result);

                    if (errorObj.TryGetProperty("errors", out var errors))
                    {
                        foreach (var err in errors.EnumerateArray())
                        {
                            ModelState.AddModelError("", err.GetString());
                        }
                    }
                    else if (errorObj.TryGetProperty("message", out var msg))
                    {
                        ModelState.AddModelError("", msg.GetString());
                    }
                    else
                    {
                        ModelState.AddModelError("", "Registration failed");
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "Registration failed");
                }

                return View("Register", model);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TokenStorage.Clear(HttpContext);

            return RedirectToAction("Login", "Auth");
        }
    }
}

