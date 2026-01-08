using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Repositories
{
    public interface IEmailOtpRepo : IGenericRepo<EmailOtp>
    {
        public Task<EmailOtp> GetOtp(string Otp);
    }
}
