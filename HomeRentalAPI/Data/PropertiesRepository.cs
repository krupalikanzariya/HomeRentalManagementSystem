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
                        UserName = reader["UserName"].ToString(),
                        Title = reader["Title"].ToString(),
                        Description = reader["Description"].ToString(),
                        Address = reader["Address"].ToString(),
                        City = reader["City"].ToString(),
                        State = reader["State"].ToString(),
                        Country = reader["Country"].ToString(),
                        PricePerNight = Convert.ToDecimal(reader["PricePerNight"]),
                        MaxGuests = Convert.ToInt32(reader["MaxGuests"]),
                        Bedrooms = Convert.ToInt32(reader["Bedrooms"]),
                        Images = new List<ImagesModel>()
                    });
                }
                reader.Close();

                // Fetch Images for each Property
                foreach (var property in properties)
                {
                    cmd = new SqlCommand("PR_Images_GetImagesByProperty", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@PropertyID", property.PropertyID);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        property.Images.Add(new ImagesModel
                        {
                            ImageID = Convert.ToInt32(reader["ImageID"]),
                            PropertyID = Convert.ToInt32(reader["PropertyID"]),
                            ImageURL = reader["ImageURL"].ToString()
                        });
                    }
                    reader.Close();
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
                        UserName = reader["UserName"].ToString(),
                        Title = reader["Title"].ToString(),
                        Description = reader["Description"].ToString(),
                        Address = reader["Address"].ToString(),
                        City = reader["City"].ToString(),
                        State = reader["State"].ToString(),
                        Country = reader["Country"].ToString(),
                        PricePerNight = Convert.ToDecimal(reader["PricePerNight"]),
                        MaxGuests = Convert.ToInt32(reader["MaxGuests"]),
                        Bedrooms = Convert.ToInt32(reader["Bedrooms"]),
                        Images = new List<ImagesModel>()

                    };
                }
                reader.Close();
                    cmd = new SqlCommand("PR_Images_GetImagesByProperty", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@PropertyID", property.PropertyID);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        property.Images.Add(new ImagesModel
                        {
                            ImageID = Convert.ToInt32(reader["ImageID"]),
                            PropertyID = Convert.ToInt32(reader["PropertyID"]),
                            ImageURL = reader["ImageURL"].ToString()
                        });
                    }
                    reader.Close();
                
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
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("PR_Properties_Add", conn, transaction)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@HostID", property.HostID);
                    cmd.Parameters.AddWithValue("@Title", property.Title);
                    cmd.Parameters.AddWithValue("@Description", property.Description);
                    cmd.Parameters.AddWithValue("@Address", property.Address);
                    cmd.Parameters.AddWithValue("@City", property.City);
                    cmd.Parameters.AddWithValue("@State", property.State);
                    cmd.Parameters.AddWithValue("@Country", property.Country);
                    cmd.Parameters.AddWithValue("@PricePerNight", property.PricePerNight);
                    cmd.Parameters.AddWithValue("@MaxGuests", property.MaxGuests);
                    cmd.Parameters.AddWithValue("@Bedrooms", property.Bedrooms);

                    // Output parameter for new PropertyID
                    SqlParameter outputIdParam = new SqlParameter("@NewPropertyID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputIdParam);

                    cmd.ExecuteNonQuery();
                    int newPropertyID = (int)outputIdParam.Value;

                    // Insert Images if they exist
                    if (property.Images != null && property.Images.Count > 0)
                    {
                        foreach (var image in property.Images)
                        {
                            SqlCommand imgCmd = new SqlCommand("PR_Images_Add", conn, transaction)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            imgCmd.Parameters.AddWithValue("@PropertyID", newPropertyID);
                            imgCmd.Parameters.AddWithValue("@ImageURL", image.ImageURL);
                            imgCmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
        public bool Update(PropertiesModel property)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // ✅ Update Property Details (Without affecting images)
                    SqlCommand cmd = new SqlCommand("PR_Properties_Update", conn, transaction)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@PropertyID", property.PropertyID);
                    cmd.Parameters.AddWithValue("@HostID", property.HostID);
                    cmd.Parameters.AddWithValue("@Title", property.Title);
                    cmd.Parameters.AddWithValue("@Description", property.Description);
                    cmd.Parameters.AddWithValue("@Address", property.Address);
                    cmd.Parameters.AddWithValue("@City", property.City);
                    cmd.Parameters.AddWithValue("@State", property.State);
                    cmd.Parameters.AddWithValue("@Country", property.Country);
                    cmd.Parameters.AddWithValue("@PricePerNight", property.PricePerNight);
                    cmd.Parameters.AddWithValue("@MaxGuests", property.MaxGuests);
                    cmd.Parameters.AddWithValue("@Bedrooms", property.Bedrooms);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    // ✅ Get Existing Images for this Property
                    Dictionary<int, string> existingImages = new Dictionary<int, string>();

                    SqlCommand getImagesCmd = new SqlCommand("SELECT ImageID, ImageURL FROM Images WHERE PropertyID = @PropertyID", conn, transaction);
                    getImagesCmd.Parameters.AddWithValue("@PropertyID", property.PropertyID);
                    using (SqlDataReader reader = getImagesCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            existingImages.Add(reader.GetInt32(0), reader.GetString(1)); // ImageID, ImageURL
                        }
                    }

                    // ✅ Update or Delete Existing Images
                    if (property.Images != null && property.Images.Count > 0)
                    {
                        foreach (var image in property.Images)
                        {
                            if (image.ImageID.HasValue && existingImages.ContainsKey(image.ImageID.Value))
                            {
                                // If image exists but URL has changed, update it
                                if (existingImages[image.ImageID.Value] != image.ImageURL)
                                {
                                    SqlCommand updateCmd = new SqlCommand("PR_Images_Update", conn, transaction)
                                    {
                                        CommandType = CommandType.StoredProcedure
                                    };
                                    updateCmd.Parameters.AddWithValue("@ImageID", image.ImageID.Value);
                                    updateCmd.Parameters.AddWithValue("@ImageURL", image.ImageURL);
                                    updateCmd.ExecuteNonQuery();
                                }

                                // Remove from dictionary to track images that should remain
                                existingImages.Remove(image.ImageID.Value);
                            }
                            else
                            {
                                // If new image, insert it
                                SqlCommand imgCmd = new SqlCommand("PR_Images_Add", conn, transaction)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };
                                imgCmd.Parameters.AddWithValue("@PropertyID", property.PropertyID);
                                imgCmd.Parameters.AddWithValue("@ImageURL", image.ImageURL);
                                imgCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // ✅ Delete Images that were removed
                    foreach (var imageId in existingImages.Keys)
                    {
                        SqlCommand deleteCmd = new SqlCommand("DELETE FROM Images WHERE ImageID = @ImageID", conn, transaction);
                        deleteCmd.Parameters.AddWithValue("@ImageID", imageId);
                        deleteCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
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
                        Images = new List<ImagesModel>() // Initialize Images list
                    });
                }
                reader.Close();

                // Fetch images for each property
                foreach (var property in properties)
                {
                    cmd = new SqlCommand("PR_Images_GetImagesByProperty", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@PropertyID", property.PropertyID);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        property.Images.Add(new ImagesModel
                        {
                            ImageID = Convert.ToInt32(reader["ImageID"]),
                            PropertyID = Convert.ToInt32(reader["PropertyID"]),
                            ImageURL = reader["ImageURL"].ToString()
                        });
                    }
                    reader.Close();
                }

                return properties;
            }
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
                        UserName = reader["UserName"].ToString(),
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
