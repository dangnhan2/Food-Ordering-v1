using Food_Ordering.Data;
using Food_Ordering.Models;
using Microsoft.EntityFrameworkCore;

namespace Food_Ordering.Repositories
{
    public class OrderRepo : IOrderRepo
    {
        private readonly ApplicationDbContext _context;

        public OrderRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Orders item)
        {
           _context.Orders.Add(item);
        }

        public void Delete(Orders item)
        {
            _context.Orders.Remove(item);
        }

        public IQueryable<Orders> GetAll()
        {
            return _context.Orders.AsQueryable();
        }

        public async Task<Orders?> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                   .ThenInclude(o => o.MenuItems)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public void Update(Orders item)
        {
            _context.Orders.Update(item);
        }
    }
}
