using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using FoodOrdering.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Repositories
{
    public class EmailOtpRepo : GenericRepo<EmailOtp>, IEmailOtpRepo
    {
        private readonly FoodOrderingDbContext _context;
        public EmailOtpRepo(FoodOrderingDbContext context) : base(context) {
           _context = context;
        }

        public Task<EmailOtp> GetOtp(string Otp)
        {
            throw new NotImplementedException();
        }
    }
}
