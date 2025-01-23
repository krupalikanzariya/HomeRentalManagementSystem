using System.ComponentModel.DataAnnotations;
namespace HomeRentalFrontEnd.Models
{
    public class UsersModel
    {
        public int? UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        
        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public string ProfilePictureURL { get; set; }
        public int RoleID { get; set; }

    }
    public class UserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        public int RoleID { get; set; }
    }
    public class UserRegisterModel
    {
        public int? UserID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ProfilePictureURL { get; set; }
        public int RoleID { get; set; }

    }

}
