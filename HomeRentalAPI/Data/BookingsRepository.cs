using HomeRentalAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HomeRentalAPI.Data
{
    public class BookingsRepository
    {
        private readonly string _connectionString;

        public BookingsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public IEnumerable<BookingsModel> GetAll()
        {
            var bookings = new List<BookingsModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Bookings_GetAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bookings.Add(new BookingsModel
                    {
                        BookingID = Convert.ToInt32(reader["BookingID"]),
                        UserID = Convert.ToInt32(reader["UserID"]),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        CheckInDate = Convert.ToDateTime(reader["CheckInDate"]),
                        CheckOutDate = Convert.ToDateTime(reader["CheckOutDate"]),
                        Guests = Convert.ToInt32(reader["Guests"]),
                        TotalPrice = Convert.ToDecimal(reader["TotalPrice"])
                    });
                }
                return bookings;
            }
        }

        public BookingsModel GetByPK(int BookingID)
        {
            BookingsModel booking = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Bookings_GetByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@BookingID ", BookingID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    booking = new BookingsModel
                    {
                        BookingID = Convert.ToInt32(reader["BookingID"]),
                        UserID = Convert.ToInt32(reader["UserID"]),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        CheckInDate = Convert.ToDateTime(reader["CheckInDate"]),
                        CheckOutDate = Convert.ToDateTime(reader["CheckOutDate"]),
                        Guests = Convert.ToInt32(reader["Guests"]),
                        TotalPrice = Convert.ToDecimal(reader["TotalPrice"])
                    };
                }
            }
            return booking;
        }

        public bool Delete(int BookingID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Bookings_Delete", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@BookingID ", BookingID);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Insert(BookingsModel booking)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Bookings_Add", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = booking.UserID;
                cmd.Parameters.Add("@PropertyID", SqlDbType.Int).Value = booking.PropertyID;
                cmd.Parameters.Add("@CheckInDate", SqlDbType.DateTime).Value = booking.CheckInDate;
                cmd.Parameters.Add("@CheckOutDate", SqlDbType.DateTime).Value = booking.CheckOutDate;
                cmd.Parameters.Add("@Guests", SqlDbType.Int).Value = booking.Guests;
                cmd.Parameters.Add("@TotalPrice", SqlDbType.Decimal).Value = booking.TotalPrice;
                
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Update(BookingsModel booking)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Bookings_Update", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@BookingID", SqlDbType.Int).Value = booking.BookingID;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = booking.UserID;
                cmd.Parameters.Add("@PropertyID", SqlDbType.Int).Value = booking.PropertyID;
                cmd.Parameters.Add("@CheckInDate", SqlDbType.DateTime).Value = booking.CheckInDate;
                cmd.Parameters.Add("@CheckOutDate", SqlDbType.DateTime).Value = booking.CheckOutDate;
                cmd.Parameters.Add("@Guests", SqlDbType.Int).Value = booking.Guests;
                cmd.Parameters.Add("@TotalPrice", SqlDbType.Decimal).Value = booking.TotalPrice;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        public BookingsModel GetBookingsByUser(int UserID)
        {
            BookingsModel booking = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Bookings_GetBookingsByUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserID", UserID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    booking = new BookingsModel
                    {
                        BookingID = Convert.ToInt32(reader["BookingID"]),
                        UserID = Convert.ToInt32(reader["UserID"]),
                        //FirstName = reader["FirstName"].ToString(),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        //Title = reader["Title"].ToString(),
                        CheckInDate = Convert.ToDateTime(reader["CheckInDate"]),
                        CheckOutDate = Convert.ToDateTime(reader["CheckOutDate"]),
                        Guests = Convert.ToInt32(reader["Guests"]),
                        TotalPrice = Convert.ToDecimal(reader["TotalPrice"])
                    };
                }
            }

            return booking;
        }
        public IEnumerable<UserDropDownModel> GetUsers()
        {
            var users = new List<UserDropDownModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Users_DropDown", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new UserDropDownModel
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        UserName = reader["UserName"].ToString()
                    });
                }
            }
            return users;
        }
        public IEnumerable<PropertiesDropDownModel> GetProperties()
        {
            var properties = new List<PropertiesDropDownModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Properties_DropDown", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    properties.Add(new PropertiesDropDownModel
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Title = reader["Title"].ToString()
                    });
                }
            }
            return properties;
        }
    }
}