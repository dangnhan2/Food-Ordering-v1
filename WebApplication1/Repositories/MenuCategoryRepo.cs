using Food_Ordering.Data;
using Food_Ordering.Models;
using Microsoft.EntityFrameworkCore;

namespace Food_Ordering.Repositories
{
    public class MenuCategoryRepo : IDishRepo
    {
        private readonly ApplicationDbContext _context;

        public MenuCategoryRepo(ApplicationDbContext context) { 
           _context = context;
        }

        public async Task AddAsync(MenuCategories category)
        {
            await _context.MenuCategories.AddAsync(category);
        }

        public void Delete(MenuCategories categories)
        {
            _context.MenuCategories.Remove(categories);
        }

        public IQueryable<MenuCategories> GetAll()
        {
            return _context.MenuCategories.AsQueryable();
        }

        public async Task<MenuCategories?> GetByIdAsync(Guid id)
        {
            return await _context.MenuCategories.FirstOrDefaultAsync(m => m.Id == id);
        }

        public void Update(MenuCategories categories)
        {
            _context.MenuCategories.Update(categories);
        }
    }
}
