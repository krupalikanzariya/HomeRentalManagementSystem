namespace HomeRentalFrontEnd.Models
{
        public class Dashboard
        {
            public List<DashboardCounts> Counts { get; set; }
            public List<RecentBooking> RecentBookings { get; set; }
            public List<TopRatedProperty> TopRatedProperties { get; set; }
            public List<TopHost> TopHosts { get; set; }
            public List<QuickLinks> NavigationLinks { get; set; }
        }

        public class DashboardCounts
        {
            public string Metric { get; set; }
            public int Value { get; set; }
        }

        public class RecentBooking
        {
            public int BookingID { get; set; }
            public string UserName { get; set; }
            public string PropertyTitle { get; set; }
            public DateTime CheckInDate { get; set; }
            public DateTime CheckOutDate { get; set; }
            public decimal TotalPrice { get; set; }
        }

        public class TopRatedProperty
        {
            public int PropertyID { get; set; }
            public string Title { get; set; }
            public decimal AvgRating { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
        }

        public class TopHost
        {
            public int HostID { get; set; }
            public string HostName { get; set; }
            public int TotalProperties { get; set; }
        }

        public class QuickLinks
        {
            public string ActionMethodName { get; set; }
            public string ControllerName { get; set; }
            public string LinkName { get; set; }
        }
    }

