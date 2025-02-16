using HomeRentalAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace HomeRentalAPI.Data
{
    public class ImagesRepository
    {
        private readonly string _connectionString;

        public ImagesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }
        public IEnumerable<ImagesModel> GetImages()
        {
            var images = new List<ImagesModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Images_Get", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    images.Add(new ImagesModel
                    {
                        ImageID = Convert.ToInt32(reader["ImageID"]),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Title = reader["Title"].ToString(),
                        ImageURL = reader["ImageURL"].ToString()
                    });
                }
            }
            return images;
        }
        public ImagesModel GetImagesByID(int ImageID)
        {
            ImagesModel image = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Images_GetByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ImageID ", ImageID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    image = new ImagesModel
                    {
                        ImageID = Convert.ToInt32(reader["ImageID"]),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Title = reader["Title"].ToString(),
                        ImageURL = reader["ImageURL"].ToString()
                    };
                }
            }
            return image;
        }
            public bool Delete(int ImageID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Images_Delete", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ImageID ", ImageID);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Insert(ImagesModel image)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Images_Add", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@PropertyID", SqlDbType.Int).Value = image.PropertyID;
                cmd.Parameters.Add("@ImageURL", SqlDbType.VarChar).Value = image.ImageURL;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Update(ImagesModel image)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Images_Update", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@ImageID", SqlDbType.Int).Value = image.ImageID;
                cmd.Parameters.Add("@PropertyID", SqlDbType.Int).Value = image.PropertyID;
                cmd.Parameters.Add("@ImageURL", SqlDbType.VarChar).Value = image.ImageURL;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        public IEnumerable<ImagesModel> GetImagesByProperty(int PropertyID)
        {
            var images = new List<ImagesModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Images_GetImagesByProperty", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyID", PropertyID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    images.Add(new ImagesModel
                    {
                        ImageID = Convert.ToInt32(reader["ImageID"]),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Title = reader["Title"].ToString(),
                        ImageURL = reader["ImageURL"].ToString()
                    });
                }
            }
            return images;
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
