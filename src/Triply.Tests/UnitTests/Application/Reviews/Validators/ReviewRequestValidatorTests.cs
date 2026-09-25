using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Reviews.Commands.CreateReview;
using Triply.Application.Features.Reviews.Commands.UpdateReview;
using Triply.Application.Features.Reviews.Queries.GetReviews;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Reviews.Validators;

public class ReviewRequestValidatorTests
{
    private readonly Mock<IReviewRepository> _reviews = new();
    private readonly Mock<IHotelRepository> _hotels = new();

    public ReviewRequestValidatorTests()
    {
        _hotels.Setup(x => x.IsHotelIdExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _reviews.Setup(x => x.IsReviewExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
    }

    [Fact]
    public async Task Validate_CreateReviewForExistingHotel_HasNoErrors()
    {
        var result = await new CreateReviewRequestValidator(_reviews.Object, _hotels.Object).TestValidateAsync(new CreateReviewRequest
        { HotelId = Guid.NewGuid(), UserId = Guid.NewGuid(), Rating = 5, Comment = "A very comfortable and clean place." });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_UpdateReviewWithShortComment_HasCommentError()
    {
        var result = new UpdateReviewRequestValidator().TestValidate(new UpdateReviewRequest
        { Rating = 4, Comment = "short" });

        result.ShouldHaveValidationErrorFor(x => x.Comment)
            .WithErrorMessage(ResultResponseMessages.Reviews.Validation.CommentLength.Message);
    }

    [Fact]
    public void Validate_GetReviewsWithPageZero_HasPageError()
    {
        var result = new GetReviewsRequestValidator().TestValidate(new GetReviewsRequest { Page = 0 });

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }
}
