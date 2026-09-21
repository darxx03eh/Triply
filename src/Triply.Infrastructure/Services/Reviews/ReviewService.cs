using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Services.Reviews;

/// <summary>Implements the review operations.</summary>
public partial class ReviewService(
    IReviewRepository reviewRepository,
    IHotelRepository hotelRepository,
    IBookingRepository bookingRepository,
    UserManager<TriplyUser> userManager,
    ILogger<ReviewService> logger) : IReviewService{}