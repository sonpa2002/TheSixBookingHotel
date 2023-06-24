namespace DatKhachSan.Models
{
    public class Booking
    {
        public string BookingID { get; set; }
        public string CustomerName { get; set; }
        public string CustomeID { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string RoomType { get; set; }
        public string RoomNumber { get; set; }
        public int NumberAdults { get; set; }
        public int NumberChildren { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public string BookingStatus { get; set; }
        public Booking()
        {
            BookingID = Guid.NewGuid().ToString(); // Tạo ID ngẫu nhiên khi khởi tạo đối tượng Booking
        }
    }
}
