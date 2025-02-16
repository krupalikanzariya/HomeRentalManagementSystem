namespace HomeRentalFrontEnd.Models
{
    public class ReviewsModel
    {
        public int? ReviewID { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
        public int PropertyID { get; set; }
        public string? Title { get; set; }
        public int Rating { get; set; }
        public String? Comment { get; set; } = null;
    }
}
