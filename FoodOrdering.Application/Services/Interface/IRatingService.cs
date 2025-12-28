using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Interface
{
    public interface IRatingService
    {
        public Task<PagingReponse<RatingDto>> GetAllRatingsByMenuAsync(Guid menuId, RatingParams ratingParams);
        public Task RatingPaidOrderAsync(RatingRequestDto request);
    }
}
