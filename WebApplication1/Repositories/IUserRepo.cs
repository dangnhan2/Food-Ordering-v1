using Food_Ordering.Models;

namespace Food_Ordering.Repositories
{
    public interface IUserRepo
    {
        public IQueryable<User> GetUsers();
        public Task<User?> GetUserAsync(string id);
        public void UpdateUser(User user);
    }
}
