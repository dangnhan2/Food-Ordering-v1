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
        }

        public IRefreshTokenRepo RefreshToken { get; }

        public IUserRepo UserRepo { get; }

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
