namespace HomeRentalAPI.Models
{
    public class ReviewsModel
    {
        public int? ReviewID { get; set; }
        public int UserID { get; set; }
        public int PropertyID { get; set; }
        public int Rating { get; set; }
        public String? Comment { get; set; } = null;
    }
}
