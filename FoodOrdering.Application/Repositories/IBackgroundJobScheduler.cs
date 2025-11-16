namespace FoodOrdering.Application.Repositories
{
    public interface IBackgroundJobScheduler
    {
        public Task ScheduleDeleteExpiredOtpJob_5mins(Guid id);
        public Task ScheduleUpdateExpiredOrderJob_10mins(Guid id);
        public Task RecurringCancelledOrderJob_1month(Guid id);
        public Task RecurringDeleteExpiredRefreshTokensJob_3months();
        public Task RecurringDeleteExpiredCartsJob_3hours();
        public Task RecurringPublicVouchersJob_24hours();
        public Task RecurringRetrieveVouchersJob_24hours();
        public Task RecurringResetVoucherRedemptionsJob_24hours();
        public Task RecurringDeleteNotificationsJob_1month();
    }
}
