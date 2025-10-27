using FoodOrdering.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Interface
{
    public interface IDashboardService
    {
        public Task<DashboardOverviewDTO> GetInfoAsync();
    }
}
