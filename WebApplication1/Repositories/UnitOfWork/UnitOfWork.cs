using Food_Ordering.Data;

namespace Food_Ordering.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            RefreshToken = new RefreshTokenRepo(_context);   
            UserRepo = new UserRepo(_context);
            MenuCategoryRepo = new MenuCategoryRepo(_context);
            MenuItemRepo = new DishRepo(_context);
            OrderRepo = new OrderRepo(_context);
        }

        public IRefreshTokenRepo RefreshToken { get; }

        public IUserRepo UserRepo { get; }

        public IDishRepo MenuCategoryRepo { get; }

        public IMenuItemRepo MenuItemRepo { get; }
        public IOrderRepo OrderRepo { get; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
