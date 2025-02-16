namespace HomeRentalFrontEnd.Models
{
    public class BookingsModel
    {
        public int? BookingID { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }

        public int PropertyID { get; set; }
        public string? Title { get; set; }
        public DateTime CheckInDate { get; set; } = DateTime.Now;
        public DateTime CheckOutDate { get; set; } = DateTime.Now;
        public int Guests { get; set; }
        public decimal TotalPrice { get; set; }
    }
}