using dp.business.Enums;

namespace fox.datasimple.Models
  
{
    public class User
    {
        public int Id { get; set; }
        public UserType Role { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
    }

    public class UserResponse : User
    {
        public string Token { get; set; }
    }
    public class UserCreate { 
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType Role { get; set; }
    }
}