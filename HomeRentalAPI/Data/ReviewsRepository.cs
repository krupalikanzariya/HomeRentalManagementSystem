using HomeRentalAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HomeRentalAPI.Data
{
    public class ReviewsRepository
    {
        private readonly string _connectionString;

        public ReviewsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public IEnumerable<ReviewsModel> GetAll()
        {
            var reviews = new List<ReviewsModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Reviews_GetAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reviews.Add(new ReviewsModel
                    {
                        ReviewID = Convert.ToInt32(reader["ReviewID"]),
                        UserID = Convert.ToInt32(reader["UserID"]),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Rating = Convert.ToInt32(reader["Rating"]),
                        Comment = reader["Comment"].ToString()
                    });
                }
                return reviews;
            }
        }

        public ReviewsModel GetByPK(int ReviewID)
        {
            ReviewsModel review = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Reviews_GetByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ReviewID ", ReviewID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    review = new ReviewsModel
                    {
                        ReviewID = Convert.ToInt32(reader["ReviewID"]),
                        UserID = Convert.ToInt32(reader["UserID"]),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Rating = Convert.ToInt32(reader["Rating"]),
                        Comment = reader["Comment"].ToString()
                    };
                }
            }
            return review;
        }

        public bool Delete(int ReviewID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Reviews_Delete", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ReviewID ", ReviewID);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Insert(ReviewsModel review)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Reviews_Add", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = review.UserID;
                cmd.Parameters.Add("@PropertyID", SqlDbType.Int).Value = review.PropertyID;
                cmd.Parameters.Add("@Rating", SqlDbType.Int).Value = review.Rating;
                cmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = review.Comment;

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Update(ReviewsModel review)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Reviews_Update", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@ReviewID", SqlDbType.Int).Value = review.ReviewID;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = review.UserID;
                cmd.Parameters.Add("@PropertyID", SqlDbType.Int).Value = review.PropertyID;
                cmd.Parameters.Add("@Rating", SqlDbType.Int).Value = review.Rating;
                cmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = review.Comment;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        public ReviewsModel GetReviewsByProperty(int PropertyID)
        {
            ReviewsModel review = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Reviews_GetReviewsByProperty", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyID", PropertyID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    review = new ReviewsModel
                    {
                        ReviewID = Convert.ToInt32(reader["ReviewID"]),
                        UserID = Convert.ToInt32(reader["UserID"]),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Rating = Convert.ToInt32(reader["Rating"]),
                        Comment = reader["Comment"].ToString()
                    };
                }
            }

            return review;
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
