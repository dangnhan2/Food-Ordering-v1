using FoodOrdering.Application.Repositories;
using Hangfire;

namespace FoodOrdering.Presentation.Extensions
{
    public static class RecurringJobExtensions
    {
        public static void UseRecurringJobs (this IApplicationBuilder application)
        {
            RecurringJob.AddOrUpdate<IBackgroundJobs>(
               "DeleteExpiredCarts_3hours",
               j => j.RecurringDeleteExpiredCartsJob_3hours(),
               Cron.Hourly);

            RecurringJob.AddOrUpdate<IBackgroundJobs>(
                "DeleteExpiredRefreshTokens_3months",
                j => j.RecurringDeleteExpiredRefreshTokensJob_3months(),
                Cron.Hourly());

            RecurringJob.AddOrUpdate<IBackgroundJobs>(
                "PublicVouchers_24hours",
                j => j.RecurringPublicVouchersJob_24hours(),
                Cron.Daily(),
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            RecurringJob.AddOrUpdate<IBackgroundJobs>(
                "RetrieveVouchers_24hours",
                j => j.RecurringRetrieveVouchersJob_24hours(),
                Cron.Daily(),
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            RecurringJob.AddOrUpdate<IBackgroundJobs>(
                "ResetVoucherRedemptions_24hours",
                j => j.RecurringResetVoucherRedemptionsJob_24hours(),
                Cron.Daily(),
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            RecurringJob.AddOrUpdate<IBackgroundJobs>(
                "DeleteNotifications_1month",
                j => j.RecurringDeleteNotificationsJob_1month(),
                Cron.Monthly(),
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            RecurringJob.AddOrUpdate<IBackgroundJobs>(
                "DeleteExpiredOtpJob",
                j => j.RecurringDeleteExpiredOtpJob_5mins(),
                Cron.MinuteInterval(5));

            //RecurringJob.AddOrUpdate<IBackgroundJobs>(
            //    "RecurringUpdateExpiredOrderJob_2mins",
            //    j => j.RecurringUpdateExpiredOrderJob_2mins(),
            //    Cron.MinuteInterval(2));
        }
    }
}
