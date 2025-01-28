namespace HomeRentalFrontEnd.Models
{
    public class BookingsModel
    {
        public int? BookingID { get; set; }
        public int UserID { get; set; }
        public int PropertyID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Guests { get; set; }
        public decimal TotalPrice { get; set; }
    }
}