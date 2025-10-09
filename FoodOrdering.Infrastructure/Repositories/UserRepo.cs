using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Repository
{
    public class UserRepo : GenericRepo<User>, IUserRepo
    {
        private readonly FoodOrderingDbContext _context;
        public UserRepo(FoodOrderingDbContext context) : base(context) {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.EmailOtp).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserContainsOtpAsync(Guid id)
        {
            return await _context.Users.Include(u => u.EmailOtp).FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
