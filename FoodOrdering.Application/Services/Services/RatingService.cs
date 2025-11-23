using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Services
{
   
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private const string folder = "RatingImage";

        public RatingService(
            IUnitOfWork unitOfWork, 
            ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }
        public async Task<IEnumerable<RatingDto>> GetAllRatingsByMenuAsync(Guid menuId, RatingParams ratingParams)
        {
            var ratingsByMenu = _unitOfWork.Rating.GetAll().Where(r => r.MenuId == menuId);

            IEnumerable<RatingDto> ratings;

            if (ratingParams.Page == 0 && ratingParams.PageSize == 0)
            {
                ratings = await ratingsByMenu
                          .Select(r => new RatingDto
                          {
                              Id = r.Id,
                              MenuId = menuId,
                              FullName = r.User.FullName,
                              Comment = r.Comment,
                              Stars = r.Stars,
                              Images = r.Images.Select(i => i.ImageUrl).ToList()
                          })
                          .AsNoTracking()
                          .ToListAsync();
            }
            else
            {

                ratings = await ratingsByMenu
                          .Select(r => new RatingDto
                          {
                              Id = r.Id,
                              MenuId = menuId,
                              FullName = r.User.FullName,
                              Comment = r.Comment,
                              Stars = r.Stars,
                              Images = r.Images.Select(i => i.ImageUrl).ToList()
                          })
                          .Paging(ratingParams.Page, ratingParams.PageSize)
                          .AsNoTracking()
                          .ToListAsync();
            }
            return ratings;
        }

        public async Task RatingPaidOrderAsync(RatingRequest request)
        {
            var orders = _unitOfWork.OrderMenu
                .GetAll()
                .Where(o => o.OrderId == request.OrderId
                            && o.Orders.Status == Food_Ordering.Models.Enum.OrderStatus.Paid
                            && o.MenuId == request.MenuId
                            && o.Orders.UserId == request.UserId);

            if (!orders.Any()) throw new InvalidDataException("Bạn chưa đặt món này");

            var ratings = _unitOfWork.Rating.GetAll().Where(r => r.OrderId == request.OrderId && r.MenuId == request.MenuId);

            if (await ratings.AnyAsync()) throw new InvalidOperationException("Bạn chỉ có thể đánh giá món đã mua trong đơn hàng này");

            var newRating = await MappingRating(request);

            var avg = await _unitOfWork.Rating.GetAverageRating(request.MenuId);

            if (avg == 0) avg = request.Stars;

            var menu = await _unitOfWork.Menu.GetByIdAsync(request.MenuId);

            if (menu != null) menu.AverageRating = avg;

            await _unitOfWork.Rating.AddAsync(newRating);
            await _unitOfWork.SaveChangeAsync();
        }

        private async Task<Rating> MappingRating(RatingRequest request)
        {
            var newRating = new Rating
            {
                Id = Guid.NewGuid(),
                MenuId = request.MenuId,
                UserId = request.UserId,
                OrderId = request.OrderId,
                Stars = request.Stars,
                Comment = request.Comment,
            };

            if (request.Images.Count > 0)
            {
                foreach (var image in request.Images)
                {
                    var imageUrl = await _cloudinaryService.UploadImage(image, folder);
                    var ratingImage = new RatingImage
                    {
                        Id = Guid.NewGuid(),
                        RatingId = newRating.Id,
                        ImageUrl = imageUrl,
                    };

                    newRating.Images.Add(ratingImage);
                }
            }

            return newRating;
        }
    }
}
