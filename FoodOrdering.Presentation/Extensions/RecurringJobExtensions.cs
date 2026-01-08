using FoodOrdering.Application.Repositories;
using Hangfire;

namespace FoodOrdering.Presentation.Extensions
{
    public static class RecurringJobExtensions
    {
        public static void UseRecurringJobs (this IApplicationBuilder application)
        {
            RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
               "DeleteExpiredCarts_3hours",
               j => j.RecurringDeleteExpiredCartsJob_3hours(),
               Cron.Hourly);

            RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
                "DeleteExpiredRefreshTokens_3months",
                j => j.RecurringDeleteExpiredRefreshTokensJob_3months(),
                Cron.Hourly());

            RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
                "PublicVouchers_24hours",
                j => j.RecurringPublicVouchersJob_24hours(),
                Cron.Daily(),
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
                "RetrieveVouchers_24hours",
                j => j.RecurringRetrieveVouchersJob_24hours(),
                Cron.Daily(),
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
                "ResetVoucherRedemptions_24hours",
                j => j.RecurringResetVoucherRedemptionsJob_24hours(),
                Cron.Daily(),
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
                "DeleteNotifications_1month",
                j => j.RecurringDeleteNotificationsJob_1month(),
                Cron.Monthly(),
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            RecurringJob.AddOrUpdate<IBackgroundJobScheduler>(
                "DeleteExpiredOtpJob",
                j => j.RecurringDeleteExpiredOtpJob_5mins(),
                Cron.MinuteInterval(5));
        }
    }
}
