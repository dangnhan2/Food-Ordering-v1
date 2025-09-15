using Food_Ordering.Data;
using Food_Ordering.Models;
using Microsoft.EntityFrameworkCore;

namespace Food_Ordering.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _context;
        
        public UserRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public IQueryable<User> GetUsers()
        {
            return _context.Users.AsQueryable();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }
    }
}
