namespace EmployeeAuthAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }  // For demo only – hash in real apps
        public string Role { get; set; }
    }
}
