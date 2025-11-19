
namespace FoodOrdering.Application.DTOs.Response
{
    public class RatingDto
    {
        public Guid Id { get; set; }
        public Guid MenuId { get; set; }

        public string FullName { get; set; }

        public int Stars { get; set; }
        public string? Comment { get; set; }
        public ICollection<string> Images { get; set; } = new List<string>();
    }
}
