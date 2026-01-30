using System.Linq.Expressions;

namespace FoodOrdering.Application.Services.Hangfire
{
    public interface IBackgroundJobService
    {
        string Enqueue<T>(Expression<Action<T>> methodCall);
        string Enqueue<T>(Expression<Func<T, Task>> methodCall);
        void Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);
    }
}
