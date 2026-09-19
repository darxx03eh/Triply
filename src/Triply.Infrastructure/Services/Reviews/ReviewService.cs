using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Reviews;

public partial class ReviewService(
    IReviewRepository reviewRepository,
    IHotelRepository hotelRepository,
    IUserRepository userRepository) : IReviewService{}