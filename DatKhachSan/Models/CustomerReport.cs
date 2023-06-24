namespace DatKhachSan.Models
{
    public class CustomerReport
    {
        public string ReportID { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string Content { get; set; }
        public CustomerReport()
        {
            ReportID = Guid.NewGuid().ToString(); // Tạo ID ngẫu nhiên khi khởi tạo đối tượng Booking
        }
    }
}
