namespace API.ViewModels
{
    public class LoginVM
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class PayloadVM
    {
        public string Id_Account { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Roles { get; set; }
    }
}
