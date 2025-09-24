using Food_Ordering.Models;

namespace Food_Ordering.Repositories
{
    public interface IOrderRepo
    {
        public IQueryable<Orders> GetAll();
        public IEnumerable<Orders> GetAllOrdersExpired();
        public Task<Orders?> GetByIdAsync(Guid id);
        public void Add(Orders item);
        public void Update(Orders item);
        public void Delete(Orders item);
    }
}
