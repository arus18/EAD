namespace WebApplication1

{
    public class User
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string NIC { get; set; }

        public User(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}