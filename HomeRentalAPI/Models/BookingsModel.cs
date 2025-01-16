namespace HomeRentalAPI.Models
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
        //public string? FirstName { get; set; }
        //public string? Title { get; set; }
    }
}