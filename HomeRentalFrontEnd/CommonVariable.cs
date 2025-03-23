namespace HomeRentalFrontEnd
{

    public class CommonVariable
    {
        private static IHttpContextAccessor _HttpContextAccessor;

        static CommonVariable()
        {
            _HttpContextAccessor = new HttpContextAccessor();
        }


        public static int? UserID()
        {

            if (_HttpContextAccessor.HttpContext.Session.GetString("UserID") == null)
            {
                return null;
            }

            return Convert.ToInt32(_HttpContextAccessor.HttpContext.Session.GetString("UserID"));
        }

        public static string UserName()
        {
            if (_HttpContextAccessor.HttpContext.Session.GetString("UserName") == null)
            {
                return null;
            }

            return _HttpContextAccessor.HttpContext.Session.GetString("UserName");
        }

        public static string RoleID()
        {
            if (_HttpContextAccessor.HttpContext.Session.GetString("RoleID") == null)
            {
                return null;
            }
            return _HttpContextAccessor.HttpContext.Session.GetString("RoleID");
        }
        public static string HostID()
        {
            if (_HttpContextAccessor.HttpContext.Session.GetString("HostID") == null)
            {
                return null;
            }
            return _HttpContextAccessor.HttpContext.Session.GetString("HostID");
        }
        public static string ProfilePictureURL()
        {
            if (_HttpContextAccessor.HttpContext.Session.GetString("ProfilePictureURL") == null)
            {
                return null;
            }
            return _HttpContextAccessor.HttpContext.Session.GetString("ProfilePictureURL");
        }
    }
}
