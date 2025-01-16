using Microsoft.Extensions.Hosting;

namespace HomeRentalAPI.Models
{
    public class PropertiesModel
    {
        public int? PropertyID { get; set; }
        public int HostID { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }

    }
}