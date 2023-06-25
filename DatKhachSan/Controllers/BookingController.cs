using DatKhachSan.Data;
using DatKhachSan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace DatKhachSan.Controllers
{
    public class BookingController : Controller
    {
        private readonly ILogger<BookingController> _logger;
        private readonly BookingContext _context;
        private readonly UserManager<ApiUser> _userManager;
        private readonly HttpClient _httpClient;
        public BookingController(HttpClient httpClient, ILogger<BookingController> logger, BookingContext context, UserManager<ApiUser> userManager)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7051/api/");// localhost:7051//payment.api.com
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        
        [Authorize(Roles = "Admin,Manager,Partner")]
        [HttpPost]
        public IActionResult GetBookingsWithRole(SearchModel searchModel)
        {
            string SearchString = searchModel.SearchString;
            if (User.IsInRole("Admin"))
            {
                var bookings = _context.Bookings.Where(b => b.CustomeID == SearchString || b.BookingID == SearchString || b.CustomerEmail == SearchString).ToList();
                return Json(bookings);
            }
            else if (User.IsInRole("Manager"))
            {
                var bookings = _context.Bookings.Where(b => b.CustomeID == SearchString || b.BookingID == SearchString || b.CustomerEmail == SearchString).ToList();
                return Json(bookings);
            }
            else if (User.IsInRole("Partner"))
            {
                var bookings = _context.Bookings.Where(b => b.BookingID == SearchString).ToList();
                return Json(bookings);
            }
            else
            {
                return BadRequest(new { success = false, message = "Lỗi không gửi được truy vấn. Vui lòng tải lại trang và thử lại." });
            }
            return BadRequest(new { success = false, message = "Lỗi không gửi được truy vấn. Vui lòng tải lại trang và thử lại." });
        }
        [Authorize]
        public IActionResult GetBookingsByCustomerId()
        {
            string userId = _userManager.GetUserId(User);

            // Truy vấn danh sách booking dựa trên ID người dùng
            var bookings = _context.Bookings.Where(b => b.CustomeID == userId).ToList();

            return View(bookings);
        }
        [Authorize(Roles ="Admin,Manager")]
        public IActionResult GetReportList()
        {
            var reports = _context.CustomerReports.ToList();

            return View(reports);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateWithPaymentUponCheckin(BookingViewModel BVmodel)
        {
            if (ModelState.IsValid)
            {
                if (BVmodel.PaymentMethod == "Payment-upon-checkin")
                {
                    string userId = _userManager.GetUserId(User);
                    Random random = new Random();
                    int randomNumber = random.Next(1, 101);
                    var booking = new Booking
                    {
                        CustomerName = BVmodel.CustomerName,
                        CustomeID = userId,
                        CustomerEmail = BVmodel.CustomerEmail,
                        CustomerPhone = BVmodel.CustomerPhone,
                        HotelName = BVmodel.HotelName,
                        HotelAddress = BVmodel.HotelAddress,
                        BookingDate = DateTime.Now,
                        CheckInDate = BVmodel.CheckInDate,
                        CheckOutDate = BVmodel.CheckOutDate,
                        RoomType = BVmodel.RoomType,
                        RoomNumber = randomNumber.ToString(),
                        NumberAdults = BVmodel.NumberAdults,
                        NumberChildren = BVmodel.NumberChildren,
                        PaymentMethod = BVmodel.PaymentMethod,
                        TotalAmount = BVmodel.TotalAmount,
                        BookingStatus = "Confirmed"
                    };
                    _context.Bookings.Add(booking);
                    _context.SaveChanges();
                    return Ok(new { success = true, message = "Đặt phòng thành công!" });
                }
                else {return BadRequest(new { success = false, message = "Yêu cầu đặt phòng không được chấp nhận. Vui lòng tải lại trang và thử lại." }); }
            }
            var modelErrors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();
            return BadRequest(new { success = false, message = modelErrors[0] });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateWithPaymentCreditCard(BookingViewModelWithCreditCard BVmodel)
        {
            if (ModelState.IsValid)
            {
                var CardInfo = new
                {
                    name = BVmodel.FullName,
                    cardNumber = BVmodel.CardNumber,
                    expirationYear = BVmodel.CardYear,
                    expirationMonth = BVmodel.CardMonth,
                    cvc = BVmodel.CardCvc
                };
                var requestContent = new
                {
                    email = BVmodel.Email,
                    name = BVmodel.FullName,
                    creditCard = CardInfo
                };
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = System.Text.Json.JsonSerializer.Serialize(requestContent, options);
                var requestContentString = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("Stripe/customer/add", requestContentString);
                //_logger.LogInformation(response.ToString());
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    dynamic apiResponse = JsonConvert.DeserializeObject(jsonString);
                    string CusIDformAPI = apiResponse.customerId;
                    string CusEmailfromAPI= apiResponse.email;
                    string userId = _userManager.GetUserId(User);
                    Random random = new Random();
                    int randomNumber = random.Next(1, 101);
                    var booking = new Booking
                    {
                        CustomerName = BVmodel.CustomerName,
                        CustomeID = userId,
                        CustomerEmail = BVmodel.CustomerEmail,
                        CustomerPhone = BVmodel.CustomerPhone,
                        HotelName = BVmodel.HotelName,
                        HotelAddress = BVmodel.HotelAddress,
                        BookingDate = DateTime.Now,
                        CheckInDate = BVmodel.CheckInDate,
                        CheckOutDate = BVmodel.CheckOutDate,
                        RoomType = BVmodel.RoomType,
                        RoomNumber = randomNumber.ToString(),
                        NumberAdults = BVmodel.NumberAdults,
                        NumberChildren = BVmodel.NumberChildren,
                        PaymentMethod = BVmodel.PaymentMethod,
                        TotalAmount = BVmodel.TotalAmount,
                        BookingStatus = "Completly-Payment"
                    };
                    var paymentbycard = new PaymentByCard
                    {
                        PayForID = booking.BookingID,
                        CustomerPayID = userId,
                        CustomerName = BVmodel.FullName,
                        CustomerEmail = CusEmailfromAPI,
                        CardNumber = BVmodel.CardNumber,
                        PaymentDate = DateTime.Now,
                        TotalAmount = booking.TotalAmount,
                        PaymentDescription = "Pay For Id: " + booking.BookingID,
                        PaymentStatus = "Success"
                    };
                    string[] amountarray = booking.TotalAmount.ToString().Split(',');
                    string amountString = "";
                    if (amountarray.Length >=2)
                    {
                        amountString= amountarray[0]+ (amountarray[1].Substring(0, 2));
                    }
                    else
                    {
                        amountString = booking.TotalAmount.ToString().Replace(",", "") + "00";
                    }
                    var PayInfo = new
                    {
                        customerId = CusIDformAPI,
                        receiptEmail = CusEmailfromAPI,
                        description = "Pay for "+ booking.BookingID,
                        currency = "USD",
                        amount = amountString
                    };
                    var json2 = System.Text.Json.JsonSerializer.Serialize(PayInfo, options);
                    var requestContentString2 = new StringContent(json2, Encoding.UTF8, "application/json");
                    var response2 = await _httpClient.PostAsync("Stripe/payment/add", requestContentString2);
                    if (response2.IsSuccessStatusCode)
                    {
                        var jsonString2 = await response2.Content.ReadAsStringAsync();
                        dynamic apiResponse2 = JsonConvert.DeserializeObject(jsonString2);
                        booking.PaymentMethod+= " <payID:"+apiResponse2.paymentId+">";
                        _context.Bookings.Add(booking);
                        paymentbycard.PaymentDescription += " AND StripePaymentId: " + apiResponse2.paymentId;
                        _context.PaymentByCard.Add(paymentbycard);
                        _context.SaveChanges();
                        return Ok(new { success = true, message = "Đặt phòng thành công!", PaymentId= apiResponse2.paymentId });
                    }
                    else if (response2.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        paymentbycard.PaymentStatus = "Failed";
                        if (paymentbycard.PaymentStatus == "Failed")
                        {
                            _context.PaymentByCard.Add(paymentbycard);
                            _context.SaveChanges();
                        }
                        return BadRequest(new { success = false, message = "Thanh toán không thành công hoặc bị từ chối!" });
                    }
                    else
                    {
                        paymentbycard.PaymentStatus = "Failed";
                        if (paymentbycard.PaymentStatus == "Failed")
                        {
                            _context.PaymentByCard.Add(paymentbycard);
                            _context.SaveChanges();
                        }
                        return BadRequest(new { success = false, message = "Lỗi xử lý! Vui lòng thử lại sau." });
                    }
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    // thong tin thẻ không được chấp nhận!
                    return BadRequest(new { success = false, message = "Thông tin thẻ Không chính xác hoặc không được chấp nhận" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Lỗi xử lý! Vui lòng thử lại sau." });
                }
            }
            var modelErrors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();
            return BadRequest(new { success = false, message = modelErrors[0] });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PaymentCreditCardFor(ModelPayCreditCardFor BVmodel)
        {
            if (ModelState.IsValid)
            {
                var CardInfo = new
                {
                    name = BVmodel.FullName,
                    cardNumber = BVmodel.CardNumber,
                    expirationYear = BVmodel.CardYear,
                    expirationMonth = BVmodel.CardMonth,
                    cvc = BVmodel.CardCvc
                };
                var requestContent = new
                {
                    email = BVmodel.Email,
                    name = BVmodel.FullName,
                    creditCard = CardInfo
                };
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = System.Text.Json.JsonSerializer.Serialize(requestContent, options);
                var requestContentString = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("Stripe/customer/add", requestContentString);
                //_logger.LogInformation(response.ToString());
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    dynamic apiResponse = JsonConvert.DeserializeObject(jsonString);
                    string CusIDformAPI = apiResponse.customerId;
                    string CusEmailfromAPI = apiResponse.email;
                    string userId = _userManager.GetUserId(User);
                    
                    var paymentbycard = new PaymentByCard
                    {
                        PayForID = BVmodel.PayForID,
                        CustomerPayID = userId,
                        CustomerName = BVmodel.FullName,
                        CustomerEmail = CusEmailfromAPI,
                        CardNumber = BVmodel.CardNumber,
                        PaymentDate = DateTime.Now,
                        TotalAmount = BVmodel.TotalAmount,
                        PaymentDescription = "Pay For Id: "+ BVmodel.PayForID,
                        PaymentStatus= "Success"
                    };
                    string[] amountarray = paymentbycard.TotalAmount.ToString().Split(',');
                    string amountString = "";
                    if (amountarray.Length >= 2)
                    {
                        amountString = amountarray[0] + (amountarray[1].Substring(0, 2));
                    }
                    else
                    {
                        amountString = paymentbycard.TotalAmount.ToString().Replace(",", "") + "00";
                    }
                    var PayInfo = new
                    {
                        customerId = CusIDformAPI,
                        receiptEmail = CusEmailfromAPI,
                        description = paymentbycard.PaymentDescription,
                        currency = "USD",
                        amount = amountString
                    };
                    var json2 = System.Text.Json.JsonSerializer.Serialize(PayInfo, options);
                    var requestContentString2 = new StringContent(json2, Encoding.UTF8, "application/json");
                    var response2 = await _httpClient.PostAsync("Stripe/payment/add", requestContentString2);
                    if (response2.IsSuccessStatusCode)
                    {
                        var jsonString2 = await response2.Content.ReadAsStringAsync();
                        dynamic apiResponse2 = JsonConvert.DeserializeObject(jsonString2);
                        paymentbycard.PaymentDescription += " AND StripePaymentId: "+ apiResponse2.paymentId;
                        _context.PaymentByCard.Add(paymentbycard);
                        _context.SaveChanges();
                        return Ok(new { success = true, message = "Thanh toán thành công!", PaymentId = apiResponse2.paymentId });
                    }
                    else if (response2.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        paymentbycard.PaymentStatus = "Failed";
                        if (paymentbycard.PaymentStatus == "Failed")
                        {
                            _context.PaymentByCard.Add(paymentbycard);
                            _context.SaveChanges();
                        }
                        return BadRequest(new { success = false, message = "Thanh toán không thành công hoặc bị từ chối!" });
                    }
                    else
                    {
                        paymentbycard.PaymentStatus = "Failed";
                        if (paymentbycard.PaymentStatus == "Failed")
                        {
                            _context.PaymentByCard.Add(paymentbycard);
                            _context.SaveChanges();
                        }
                        return BadRequest(new { success = false, message = "Lỗi xử lý! Vui lòng thử lại sau." });
                    }
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    // thong tin thẻ không được chấp nhận!
                    return BadRequest(new { success = false, message = "Thông tin thẻ Không chính xác hoặc không được chấp nhận" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Lỗi xử lý! Vui lòng thử lại sau." });
                }
            }
            var modelErrors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();
            return BadRequest(new { success = false, message = modelErrors[0] });
        }
        public class ModelPayCreditCardFor
        {
            public string PayForID { get; set; }
            public decimal TotalAmount { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string CardNumber { get; set; }
            public string CardMonth { get; set; }
            public string CardYear { get; set; }
            public string CardCvc { get; set; }

        }
        public class BookingViewModelWithCreditCard
        {
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerPhone { get; set; }
            public string HotelName { get; set; }
            public string HotelAddress { get; set; }
            public DateTime CheckInDate { get; set; }
            public DateTime CheckOutDate { get; set; }
            public string RoomType { get; set; }
            public int NumberAdults { get; set; }
            public int NumberChildren { get; set; }
            public string PaymentMethod { get; set; }
            public decimal TotalAmount { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string CardNumber { get; set; }
            public string CardMonth { get; set; }
            public string CardYear { get; set; }
            public string CardCvc { get; set; }

        }
        public class BookingViewModel
        {
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerPhone { get; set; }
            public string HotelName { get; set; }
            public string HotelAddress { get; set; }
            public DateTime CheckInDate { get; set; }
            public DateTime CheckOutDate { get; set; }
            public string RoomType { get; set; }
            public int NumberAdults { get; set; }
            public int NumberChildren { get; set; }
            public string PaymentMethod { get; set; }
            public decimal TotalAmount { get; set; }
        }
        public class SearchModel
        {
            public string SearchString { get; set;}
        }
    }
}
