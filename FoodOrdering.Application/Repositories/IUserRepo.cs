using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Repositories
{
    public interface IUserRepo : IGenericRepo<User>
    {
        public Task<User?> GetUserByEmailAsync(string email);
        public Task<User?> GetUserContainsOtpAsync(Guid id);

    }
}
