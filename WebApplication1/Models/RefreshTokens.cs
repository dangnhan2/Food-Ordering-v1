namespace Food_Ordering.Models
{
    public class RefreshTokens
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpriedAt { get; set; }

    }
}
