using Food_Ordering.Repositories.UnitOfWork;

namespace Food_Ordering.Services.BackgroundJob
{
    public class BackgroundJobService : IBackgoundJobService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BackgroundJobService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CheckOrderExpired()
        {
            var expiredOrders = _unitOfWork.OrderRepo.GetAllOrdersExpired();

            foreach(var order in expiredOrders)
            {
                order.Status = Models.Enum.OrderStatus.Cancelled;
            }

            if(expiredOrders.Count() > 0)
            {
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
