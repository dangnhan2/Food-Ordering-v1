namespace Food_Ordering.DTOs.Request
{
    public class PasswordRequest
    {   
        public string Password { get; set; }    
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
