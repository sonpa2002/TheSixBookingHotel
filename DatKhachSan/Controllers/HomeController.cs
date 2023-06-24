using DatKhachSan.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Data.SqlTypes;
using Microsoft.CodeAnalysis;
using DatKhachSan.Data;
using Microsoft.AspNetCore.Identity;
using static DatKhachSan.Controllers.AccountController;
using Microsoft.EntityFrameworkCore;

namespace DatKhachSan.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookingContext _context;
        private readonly UserManager<ApiUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey= "AIzaSyBEDz8E-0blzkjsSDN-F-IfHr8Z1zIFem0";//"AIzaSyDpuWp7xh_ScEGkK4TuT_P3DkIi6Ywg0mQ";

        public HomeController(BookingContext context, HttpClient httpClient, ILogger<HomeController> logger, UserManager<ApiUser> userManager)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7051/api/");// localhost:7051//payment.api.com
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> GetHotels(string id)
        {
            string[] search_infor = id.Split(';');
            ViewData["search_by_text"] = search_infor[0];
            ViewData["NgayNhanPhong"] = search_infor[1];
            ViewData["NgayTraPhong"] = search_infor[2];
            ViewData["SoNguoiLon"] = search_infor[3];
            ViewData["SoTreEm"] = search_infor[4];
            ViewData["TinhOrTp"] = search_infor[5];

            string apiUrl = "https://maps.googleapis.com/maps/api/place/textsearch/json";
            string query = "hotels in Ho Chi Minh City";
            if(!string.IsNullOrEmpty(id))
            {
                query = "hotels in "+ search_infor[search_infor.Length-1];
            }
            string requestUrl = $"{apiUrl}?query={Uri.EscapeDataString(query)}&key={_apiKey}";
            if(search_infor[search_infor.Length - 1].IndexOf("location")!=-1)
            {
                string[] Location= search_infor[search_infor.Length - 1].Split("location");
                apiUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";
                string ViTri = Location[0] + "," + Location[1];
                string radius = "10000"; 
                string type = "lodging";
                requestUrl = $"{apiUrl}?location={Uri.EscapeDataString(ViTri)}&radius={Uri.EscapeDataString(radius)}&type={Uri.EscapeDataString(type)}&key={_apiKey}";
            }
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var response = await _httpClient.SendAsync(request);
            //_logger.LogInformation(response.ToString());
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                //_logger.LogInformation(jsonString.ToString());
                var apiResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);
                ViewData["NextPageToken"] = apiResponse.next_page_token;
                var hotels = new List<dynamic>(apiResponse.results);
                return View(hotels);
            }
            else
            {
                return View();
            }
        }
        public async Task<JsonResult> GetHotelsWithnextPageToken(string nextPageToken = "")
        {
            string apiUrl = "https://maps.googleapis.com/maps/api/place/textsearch/json";
            string query = "hotels in Ho Chi Minh City";
            string requestUrl = $"{apiUrl}?query={Uri.EscapeDataString(query)}&key={_apiKey}";
            if (!string.IsNullOrEmpty(nextPageToken))
            {
                requestUrl = $"{apiUrl}?pagetoken={nextPageToken}&key={_apiKey}";
            }
            else
            {
                return Json(null);
            }
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);
                string NextPageToken = apiResponse.next_page_token;
                if (!string.IsNullOrEmpty(NextPageToken))
                {
                    NextPageToken = NextPageToken;
                }
                else
                {
                    NextPageToken = "";
                }
                var json = new
                {
                    nextpagetoken = NextPageToken,
                    data = jsonString
                };
                return Json(json);
            }
            else
            {
                return Json(null);
            }
        }
        public async Task<IActionResult> GetInfoHotel(string id)
        {
            string[] search_infor = id.Split(';');
            string placeId = search_infor[0];
            ViewData["search_by_text"] = search_infor[1];
            ViewData["NgayNhanPhong"] = search_infor[2];
            ViewData["NgayTraPhong"] = search_infor[3];
            ViewData["SoNguoiLon"] = search_infor[4];
            ViewData["SoTreEm"] = search_infor[5];

            string apiUrl = "https://maps.googleapis.com/maps/api/place/details/json";
            string requestUrl = $"{apiUrl}?place_id={placeId}&key={_apiKey}";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                //_logger.LogInformation(jsonString.ToString());
                dynamic apiResponse = JsonConvert.DeserializeObject(jsonString);
                return View(apiResponse);
            }
            else
            {
                return View();
            }
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
            base.Dispose(disposing);
        }
        //
        [Authorize]
        public async Task<string> TestCookie()
        {
            string cookieValue = Request.Cookies[".AspNetCore.Identity.Application"];
            if (cookieValue == null)
                cookieValue = "null ne";
            // Trả về giá trị cookie
            return cookieValue;
        }
        [HttpPost]
        public async Task<IActionResult> AddReport(CustomerReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customerReport = new CustomerReport
                {
                    CustomerID = "Not-logged-in",
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    Content = model.Content
                };
                if(User.Identity.IsAuthenticated)
                {
                    string userId = _userManager.GetUserId(User);
                    customerReport.CustomerID = userId;
                }
                // Tiến hành lưu đối tượng vào cơ sở dữ liệu
                _context.CustomerReports.Add(customerReport);
                _context.SaveChanges();
                return Ok(new { success = true });
            }

            // Dữ liệu không hợp lệ
            var modelErrors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();
            return BadRequest(new { success = false, message = modelErrors[0] });
        }
        public class CustomerReportViewModel
        {
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerPhone { get; set; }
            public string Content { get; set; }
        }
        //public class RegisterViewModel
        //{
        //    [Required]
        //    [EmailAddress]
        //    [Display(Name = "Email")]
        //    public string Email { get; set; }

        //    [Required]
        //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        //    [DataType(DataType.Password)]
        //    [Display(Name = "Password")]
        //    public string Password { get; set; }

        //    [DataType(DataType.Password)]
        //    [Display(Name = "Confirm password")]
        //    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //    public string ConfirmPassword { get; set; }
        //}

        //public class LoginViewModel
        //{
        //    [Required]
        //    [EmailAddress]
        //    public string Email { get; set; }

        //    [Required]
        //    [DataType(DataType.Password)]
        //    public string Password { get; set; }

        //    [Display(Name = "Remember me?")]
        //    public bool RememberMe { get; set; }
        //}
        //
        //public async Task<IActionResult> Login()
        //{
        //    var requestContent = new
        //    {
        //        email = "sonll20@gmail.com",
        //        password = "123Abc;",
        //        rememberMe = true
        //    };
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //    };
        //    var json = System.Text.Json.JsonSerializer.Serialize(requestContent, options);
        //    var requestContentString = new StringContent(json, Encoding.UTF8, "application/json");
        //    var response = await _httpClient.PostAsync("Account/Login", requestContentString);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        //_logger.LogInformation(response.ToString());
        //        var cookies = response.Headers.GetValues("Set-Cookie");
        //        foreach (var cookie in cookies)
        //        {
        //            Response.Headers.Append("Set-Cookie", cookie.ToString());
        //        }
        //        return Content(string.Empty);
        //    }

        //    // Xử lý lỗi
        //    Console.WriteLine("gui yeu cau dang nhap that bai roi huhu");
        //    var errorMessage = await response.Content.ReadAsStringAsync();
        //    return BadRequest(errorMessage);
        //}
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}