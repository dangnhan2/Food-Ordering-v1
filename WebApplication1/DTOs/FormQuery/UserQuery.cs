namespace Food_Ordering.DTOs.FormQuery
{
    public class UserQuery
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public string? fullName { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
    }
}
