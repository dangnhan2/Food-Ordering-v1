using Food_Ordering.Models;

namespace Food_Ordering.Repositories
{
    public interface IMenuItemRepo
    {
        public IQueryable<MenuItems> GetAll();
        public Task<MenuItems?> GetByIdAsync(Guid id);
        public Task AddAsync(MenuItems item);
        public void Update(MenuItems item);
        public void Delete(MenuItems item);
    }
}
