namespace Food_Ordering.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        public IRefreshTokenRepo RefreshToken { get; }
        public IUserRepo UserRepo { get; }
        public IDishRepo MenuCategoryRepo { get; }
        public IMenuItemRepo MenuItemRepo { get; }
        public IOrderRepo OrderRepo { get; }
        Task SaveAsync();
    }
}
