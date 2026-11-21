namespace RaceDay.MVC.ViewModels
{
    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Organiser" or "Participant"
    }

    // MATCHES YOUR API LoginResponse RECORD EXACTLY
    public class ApiLoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
    }
}