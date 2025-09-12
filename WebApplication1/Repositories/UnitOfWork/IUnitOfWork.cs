namespace Food_Ordering.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        public IRefreshTokenRepo RefreshToken { get; }
        Task SaveAsync();
    }
}
