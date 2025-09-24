namespace Food_Ordering.Services.BackgroundJob
{
    public interface IBackgoundJobService
    {
        public Task CheckOrderExpired();
    }
}
