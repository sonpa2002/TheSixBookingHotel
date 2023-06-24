using DatKhachSan.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text;
using System.Xml.Linq;

namespace DatKhachSan.Controllers
{
    public class StripeViewAPI : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public StripeViewAPI(HttpClient httpClient, ILogger<HomeController> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://payment.api.com:80/api/");// localhost:7051//payment.api.com
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
    //    [HttpPost]
    //    public async Task<IActionResult> Register(RegisterViewModel model)
    //    {
    //        if (ModelState.IsValid)
    //        {

    //            var user = new ApiUser { UserName = model.Email, Email = model.Email };
    //            var result = await _userManager.CreateAsync(user, model.Password);

    //            if (result.Succeeded)
    //            {
    //                // Đăng ký thành công
    //                return Ok(new { success = true });
    //            }
    //            else
    //            {
    //                // Xử lý lỗi đăng ký
    //                return BadRequest(new { success = false, message = "Lỗi vô định. Vui lòng thử lại sau." });
    //            }
    //        }

    //        // Dữ liệu không hợp lệ
    //        var modelErrors = ModelState.Values.SelectMany(v => v.Errors)
    //                                  .Select(e => e.ErrorMessage)
    //                                  .ToList();
    //        return BadRequest(new { success = false, message = modelErrors[0] });
    //    }
    //    public async Task<IActionResult> Logout()
    //    {
    //        await _signInManager.SignOutAsync();
    //        _logger.LogInformation("User logged out.");
    //        return RedirectToAction("Index", "Home");
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> Login(LoginViewModel model)
    //    {
    //        var requestContent = new
    //        {
    //            email = model.Email,
    //            password = model.Password,
    //            rememberMe = model.RememberMe
    //        };
    //        var options = new JsonSerializerOptions
    //        {
    //            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //        };
    //        var json = System.Text.Json.JsonSerializer.Serialize(requestContent, options);
    //        var requestContentString = new StringContent(json, Encoding.UTF8, "application/json");
    //        var response = await _httpClient.PostAsync("Account/Login", requestContentString);
    //        if (response.IsSuccessStatusCode)
    //        {
    //            //_logger.LogInformation(response.ToString());
    //            var cookies = response.Headers.GetValues("Set-Cookie");
    //            foreach (var cookie in cookies)
    //            {
    //                Response.Headers.Append("Set-Cookie", cookie.ToString());
    //            }
    //            return Content(string.Empty);
    //        }

    //        // Xử lý lỗi
    //        Console.WriteLine("gui yeu cau dang nhap that bai roi huhu");
    //        var errorMessage = await response.Content.ReadAsStringAsync();
    //        return BadRequest(errorMessage);
    //    }
    //    public class RegisterViewModel
    //    {
    //        [Required]
    //        [EmailAddress]
    //        [Display(Name = "Email")]
    //        public string Email { get; set; }

    //        [Required]
    //        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    //        [DataType(DataType.Password)]
    //        [Display(Name = "Password")]
    //        public string Password { get; set; }

    //        [DataType(DataType.Password)]
    //        [Display(Name = "Confirm password")]
    //        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    //        public string ConfirmPassword { get; set; }
    //    }

    //    public class LoginViewModel
    //    {
    //        public string Email { get; set; }
    //        public string Password { get; set; }
    //        public bool RememberMe { get; set; }
    //    }
    }
}
