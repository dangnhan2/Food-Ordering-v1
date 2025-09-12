namespace Food_Ordering.Services.Email
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string body);
    }
}
