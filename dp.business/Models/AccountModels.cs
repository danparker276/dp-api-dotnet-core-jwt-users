using System.Security.Claims;


namespace dp.business.Models
{
    public class AccessToken
    {
        /// <summary>
        /// Access token which will be used for authenticating each request
        /// </summary>
        public string access_token { get; set; }
    }


    public class TokenRequest
    {
        public string Email { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// UserTypeId 1=member, 2=merchant, 3=admin
        /// </summary>
        public int UserTypeId { get; set; }

    }

}
