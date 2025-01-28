using HomeRentalAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace HomeRentalAPI.Data
{
    public class PropertiesRepository
    {
        private readonly string _connectionString;

        public PropertiesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public IEnumerable<PropertiesModel> GetAll()
        {
            var properties = new List<PropertiesModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Properties_GetAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    properties.Add(new PropertiesModel
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        HostID = Convert.ToInt32(reader["HostID"]),
                        Title = reader["Title"].ToString(),
                        Description = reader["Description"].ToString(),
                        Address = reader["Address"].ToString(),
                        City = reader["City"].ToString(),
                        State = reader["State"].ToString(),
                        Country = reader["Country"].ToString(),
                        PricePerNight = Convert.ToDecimal(reader["PricePerNight"]),
                        MaxGuests = Convert.ToInt32(reader["MaxGuests"]),
                        Bedrooms = Convert.ToInt32(reader["Bedrooms"]),
                    });
                }
                return properties;
            }
        }

        public PropertiesModel GetByPK(int PropertyID)
        {
            PropertiesModel property = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Properties_GetByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyID ", PropertyID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    property = new PropertiesModel
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        HostID = Convert.ToInt32(reader["HostID"]),
                        Title = reader["Title"].ToString(),
                        Description = reader["Description"].ToString(),
                        Address = reader["Address"].ToString(),
                        City = reader["City"].ToString(),
                        State = reader["State"].ToString(),
                        Country = reader["Country"].ToString(),
                        PricePerNight = Convert.ToDecimal(reader["PricePerNight"]),
                        MaxGuests = Convert.ToInt32(reader["MaxGuests"]),
                        Bedrooms = Convert.ToInt32(reader["Bedrooms"]),
                    };
                }
            }
            return property;
        }

        public bool Delete(int PropertyID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Properties_Delete", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyID ", PropertyID);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Insert(PropertiesModel property)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Properties_Add", conn)
                {
                    CommandType = CommandType.StoredProcedure
                }; 
                cmd.Parameters.Add("@HostID", SqlDbType.Int).Value = property.HostID;
                cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = property.Title;
                cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = property.Description;
                cmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = property.Address;
                cmd.Parameters.Add("@City", SqlDbType.VarChar).Value = property.City;
                cmd.Parameters.Add("@State", SqlDbType.VarChar).Value = property.State;
                cmd.Parameters.Add("@Country", SqlDbType.VarChar).Value = property.Country;
                cmd.Parameters.Add("@PricePerNight", SqlDbType.Decimal).Value = property.PricePerNight;
                cmd.Parameters.Add("@MaxGuests", SqlDbType.Int).Value = property.MaxGuests;
                cmd.Parameters.Add("@Bedrooms", SqlDbType.Int).Value = property.Bedrooms;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Update(PropertiesModel property)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Properties_Update", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@PropertyID", SqlDbType.Int).Value = property.PropertyID;
                cmd.Parameters.Add("@HostID", SqlDbType.Int).Value = property.HostID;
                cmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = property.Title;
                cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = property.Description;
                cmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = property.Address;
                cmd.Parameters.Add("@City", SqlDbType.VarChar).Value = property.City;
                cmd.Parameters.Add("@State", SqlDbType.VarChar).Value = property.State;
                cmd.Parameters.Add("@Country", SqlDbType.VarChar).Value = property.Country;
                cmd.Parameters.Add("@PricePerNight", SqlDbType.Decimal).Value = property.PricePerNight;
                cmd.Parameters.Add("@MaxGuests", SqlDbType.Int).Value = property.MaxGuests;
                cmd.Parameters.Add("@Bedrooms", SqlDbType.Int).Value = property.Bedrooms;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        public IEnumerable<PropertiesModel> GetPropertiesByHost(int hostID)
        {
            var properties = new List<PropertiesModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Properties_GetPropertiesByHost", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@HostID", hostID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    properties.Add(new PropertiesModel
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        HostID = Convert.ToInt32(reader["HostID"]),
                        Title = reader["Title"].ToString(),
                        Description = reader["Description"].ToString(),
                        Address = reader["Address"].ToString(),
                        City = reader["City"].ToString(),
                        State = reader["State"].ToString(),
                        Country = reader["Country"].ToString(),
                        PricePerNight = Convert.ToDecimal(reader["PricePerNight"]),
                        MaxGuests = Convert.ToInt32(reader["MaxGuests"]),
                        Bedrooms = Convert.ToInt32(reader["Bedrooms"]),
                    });
                }
            }
            return properties;
        }
        public IEnumerable<PropertiesModel> SearchProperties(string city, decimal minPrice, decimal maxPrice, int guests)
        {
            var properties = new List<PropertiesModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Properties_SearchByCity_Price_Guest", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@City", city);
                cmd.Parameters.AddWithValue("@MinPrice", minPrice);
                cmd.Parameters.AddWithValue("@MaxPrice", maxPrice);
                cmd.Parameters.AddWithValue("@Guests", guests);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    properties.Add(new PropertiesModel
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        HostID = Convert.ToInt32(reader["HostID"]),
                        Title = reader["Title"].ToString(),
                        Description = reader["Description"].ToString(),
                        Address = reader["Address"].ToString(),
                        City = reader["City"].ToString(),
                        State = reader["State"].ToString(),
                        Country = reader["Country"].ToString(),
                        PricePerNight = Convert.ToDecimal(reader["PricePerNight"]),
                        MaxGuests = Convert.ToInt32(reader["MaxGuests"]),
                        Bedrooms = Convert.ToInt32(reader["Bedrooms"]),
                    });
                }
            }
            return properties;
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
        
    }
}
