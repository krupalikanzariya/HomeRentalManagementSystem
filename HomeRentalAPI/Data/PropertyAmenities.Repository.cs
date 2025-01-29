using HomeRentalAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HomeRentalAPI.Data
{
    public class PropertyAmenitiesRepository
    {
        private readonly string _connectionString;

        public PropertyAmenitiesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public IEnumerable<PropertyAmenitiesModel> GetAll()
        {
            var propertyAmenities = new List<PropertyAmenitiesModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_PropertyAmenities_Get", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    propertyAmenities.Add(new PropertyAmenitiesModel
                    {
                        PropertyAmenityID = Convert.ToInt32(reader["PropertyAmenityID"]),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        AmenityID = Convert.ToInt32(reader["AmenityID"])                    });
                }
                return propertyAmenities;
            }
        }

        public PropertyAmenitiesModel GetByPK(int PropertyAmenityID)
        {
            PropertyAmenitiesModel propertyAmenity = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_PropertyAmenities_GetByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyAmenityID ", PropertyAmenityID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    propertyAmenity = new PropertyAmenitiesModel
                    {

                        PropertyAmenityID = Convert.ToInt32(reader["PropertyAmenityID"]),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        AmenityID = Convert.ToInt32(reader["AmenityID"])
                    };
                }
            }
            return propertyAmenity;
        }

        public bool Delete(int PropertyAmenityID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_PropertyAmenities_Delete", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyAmenityID ", PropertyAmenityID);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Insert(PropertyAmenitiesModel propertyAmenity)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_PropertyAmenities_Add", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@PropertyID", SqlDbType.Int).Value = propertyAmenity.PropertyID;
                cmd.Parameters.Add("@AmenityID", SqlDbType.Int).Value = propertyAmenity.AmenityID;

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Update(PropertyAmenitiesModel propertyAmenity)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_PropertyAmenities_Update", conn)
                {
                    CommandType = CommandType.StoredProcedure
                }; 
                cmd.Parameters.Add("@PropertyAmenityID", SqlDbType.Int).Value = propertyAmenity.PropertyAmenityID;
                cmd.Parameters.Add("@PropertyID", SqlDbType.Int).Value = propertyAmenity.PropertyID;
                cmd.Parameters.Add("@AmenityID", SqlDbType.Int).Value = propertyAmenity.AmenityID;

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        public IEnumerable<PropertyAmenitiesModel> GetAmenitiesByProperty(int PropertyID)
        {
            var propertyAmenities = new List<PropertyAmenitiesModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_PropertyAmenities_GetAmenitiesByProperty", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyID", PropertyID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    propertyAmenities.Add(new PropertyAmenitiesModel
                    {
                        PropertyAmenityID = Convert.ToInt32(reader["PropertyAmenityID"]),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        AmenityID = Convert.ToInt32(reader["AmenityID"]),
                    });
                }
            }
            return propertyAmenities;
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
        public IEnumerable<AmenitiesModel> GetAmenities()
        {
            var amenities = new List<AmenitiesModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("[PR_Amenities_DropDown]", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    amenities.Add(new AmenitiesModel
                    {
                        AmenityID = Convert.ToInt32(reader["AmenityID"]),
                        Name = reader["Name"].ToString()
                    });
                }
            }
            return amenities;
        }
    }
}
