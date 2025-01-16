using Microsoft.Data.SqlClient;
using System.Data;
using HomeRentalAPI.Models;

namespace HomeRentalAPI.Data
{
    public class UsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public IEnumerable<UsersModel> GetAll()
        {
            var users = new List<UsersModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Users_GetAll", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new UsersModel
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        UserName = reader["UserName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Password = reader["Password"].ToString(),
                        ProfilePictureURL = reader["ProfilePictureURL"].ToString()
                    });
                }
                return users;
            }
        }

        public UsersModel GetByPK(int userID)
        {
            UsersModel user = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Users_GetByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserID", userID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    user = new UsersModel
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        UserName = reader["UserName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Password = reader["Password"].ToString(),
                        ProfilePictureURL = reader["ProfilePictureURL"].ToString()
                    };
                }
            }
            return user;
        }

        public bool Delete(int userID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Users_Delete", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserID", userID);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Insert(UsersModel user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Users_Add", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = user.FirstName;
                cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = user.LastName;
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = user.UserName;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = user.Email;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = user.Password;
                cmd.Parameters.Add("@ProfilePictureURL", SqlDbType.VarChar).Value = user.ProfilePictureURL;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Update(UsersModel user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Users_UpdateByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = user.UserID;
                cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = user.FirstName;
                cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = user.LastName;
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = user.UserName;
                cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = user.Email;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = user.Password;
                cmd.Parameters.Add("@ProfilePictureURL", SqlDbType.VarChar).Value = user.ProfilePictureURL;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
