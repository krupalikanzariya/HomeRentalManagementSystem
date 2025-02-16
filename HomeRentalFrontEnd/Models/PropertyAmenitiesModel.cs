namespace HomeRentalFrontEnd.Models
{
    public class PropertyAmenitiesModel
    {
        public int? PropertyAmenityID { get; set; }
        public int PropertyID { get; set; }
        public string? Title { get; set; }
        public int AmenityID { get; set; }
        public string? Name { get; set; }

    }
}