namespace Food_Ordering.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        public IRefreshTokenRepo RefreshToken { get; }
        public IUserRepo UserRepo { get; }
        public IMenuCategoryRepo MenuCategoryRepo { get; }
        Task SaveAsync();
    }
}
