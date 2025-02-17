using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using HomeRentalFrontEnd.Models;

namespace HomeRentalFrontEnd.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;

        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = new Dashboard
            {
                Counts = new List<DashboardCounts>(),
                RecentBookings = new List<RecentBooking>(),
                TopRatedProperties = new List<TopRatedProperty>(),
                TopHosts = new List<TopHost>(),
                NavigationLinks = new List<QuickLinks>()
            };

            using (var connection = new SqlConnection(this._configuration.GetConnectionString("ConnectionString")))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("usp_GetDashboardData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            // Fetch counts
                            while (await reader.ReadAsync())
                            {
                                dashboardData.Counts.Add(new DashboardCounts
                                {
                                    Metric = reader["Metric"].ToString(),
                                    Value = Convert.ToInt32(reader["Value"])
                                });
                            }

                            // Fetch recent bookings
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    dashboardData.RecentBookings.Add(new RecentBooking
                                    {
                                        BookingID = Convert.ToInt32(reader["BookingID"]),
                                        UserName = reader["UserName"].ToString(),
                                        PropertyTitle = reader["PropertyTitle"].ToString(),
                                        CheckInDate = Convert.ToDateTime(reader["CheckInDate"]),
                                        CheckOutDate = Convert.ToDateTime(reader["CheckOutDate"]),
                                        TotalPrice = Convert.ToDecimal(reader["TotalPrice"])
                                    });
                                }
                            }

                            // Fetch top-rated properties
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    dashboardData.TopRatedProperties.Add(new TopRatedProperty
                                    {
                                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                                        Title = reader["Title"].ToString(),
                                        AvgRating = Convert.ToDecimal(reader["AvgRating"]),
                                        City = reader["City"].ToString(),
                                        Country = reader["Country"].ToString()
                                    });
                                }
                            }

                            // Fetch top hosts
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    dashboardData.TopHosts.Add(new TopHost
                                    {
                                        HostID = Convert.ToInt32(reader["HostID"]),
                                        HostName = reader["HostName"].ToString(),
                                        TotalProperties = Convert.ToInt32(reader["TotalProperties"])
                                    });
                                }
                            }
                        }
                    }
                }
            }

            dashboardData.NavigationLinks = new List<QuickLinks>
            {
                new QuickLinks {ActionMethodName = "Index", ControllerName="Home", LinkName="Home" },
                new QuickLinks {ActionMethodName = "AdminPropertiesList", ControllerName="Properties", LinkName="All Properties" },
                new QuickLinks {ActionMethodName = "BookingsList", ControllerName="Bookings", LinkName="Bookings" },
                new QuickLinks {ActionMethodName = "ReviewsList", ControllerName="Reviews", LinkName="Reviews" },
                new QuickLinks {ActionMethodName = "UsersList", ControllerName="Users", LinkName="Users" }
            };

            var model = new Dashboard
            {
                Counts = dashboardData.Counts,
                RecentBookings = dashboardData.RecentBookings,
                TopRatedProperties = dashboardData.TopRatedProperties,
                TopHosts = dashboardData.TopHosts,
                NavigationLinks = dashboardData.NavigationLinks
            };

            return View("Index", model);
        }
    }
}
