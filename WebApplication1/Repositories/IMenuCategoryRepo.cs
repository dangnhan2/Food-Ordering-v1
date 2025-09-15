using Food_Ordering.Models;

namespace Food_Ordering.Repositories
{
    public interface IMenuCategoryRepo
    {
        public IQueryable<MenuCategories> GetAll();
        public Task<MenuCategories?> GetByIdAsync(Guid id);
        public Task AddAsync(MenuCategories category);
        public void Update(MenuCategories categories);
        public void Delete(MenuCategories categories);
    }
}
