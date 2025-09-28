using FoodOrdering.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application
{
    public interface IUnitOfWork : IDisposable
    {
       IOrderRepo Order { get; }
       IMenuRepo Menu { get; }
       IUserRepo User { get; }
       ICategoryRepo Category { get; }
       IVoucherRepo Voucher { get; }
       IRefreshTokenRepo RefreshToken { get; }
       Task SaveChangeAsync();
    }
}
