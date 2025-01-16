using HomeRentalAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HomeRentalAPI.Data
{
    public class AmenitiesRepository
    {
        private readonly string _connectionString;

        public AmenitiesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public IEnumerable<AmenitiesModel> GetAll()
        {
            var amenities = new List<AmenitiesModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Amenities_GetAll", conn)
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
                return amenities;
            }
        }

        public AmenitiesModel GetByPK(int AmenityID)
        {
            AmenitiesModel amenity = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Amenities_GetByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@AmenityID ", AmenityID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    amenity = new AmenitiesModel
                    {
                        AmenityID = Convert.ToInt32(reader["AmenityID"]),
                        Name = reader["Name"].ToString()
                    };
                }
            }
            return amenity;
        }

        public bool Delete(int AmenityID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Amenities_Delete", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@AmenityID ", AmenityID);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Insert(AmenitiesModel amenity)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Amenities_Add", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = amenity.Name;

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Update(AmenitiesModel amenity)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Amenities_Update", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@AmenityID", SqlDbType.Int).Value = amenity.AmenityID;
                cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = amenity.Name;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
