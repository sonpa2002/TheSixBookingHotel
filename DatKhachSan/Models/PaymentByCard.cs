namespace DatKhachSan.Models
{
    public class PaymentByCard
    {
        public string PaymentID { get; set; }
        public string PayForID { get; set; }
        public string CustomerPayID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CardNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentDescription { get; set; }
        public string PaymentStatus { get; set; }
        public PaymentByCard()
        {
            PaymentID = Guid.NewGuid().ToString(); // Tạo ID ngẫu nhiên khi khởi tạo đối tượng Booking
        }
    }
}
