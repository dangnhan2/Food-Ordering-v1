using Food_Ordering.Data;
using Food_Ordering.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Food_Ordering.Repositories
{
    public class MenuItemRepo : IMenuItemRepo
    {
        private readonly ApplicationDbContext _context;

        public MenuItemRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MenuItems item)
        {
            await _context.Menus.AddAsync(item);
        }

        public void Delete(MenuItems item)
        {
           _context.Menus.Remove(item);
        }

        public IQueryable<MenuItems> GetAll()
        {
            return _context.Menus.AsQueryable();
        }

        public Task<MenuItems?> GetByIdAsync(Guid id)
        {
            return _context.Menus.FirstOrDefaultAsync(m => m.Id == id);
        }

        public void Update(MenuItems item)
        {
            _context.Menus.Update(item);
        }
    }
}
