using Specialist.Assessment.Models;

namespace Specialist.Assessment.Services;

public interface IDiscountService
{
    Task<CartSummary> CalculateCartAsync(IEnumerable<Course> courses, User user);
}

public class DiscountService : IDiscountService
{
    private const int BurningGroupDaysThreshold = 5;
    private const decimal BurningGroupDiscount = 20m;
    private const decimal GraduateDiscount = 30m;

    public async Task<CartSummary> CalculateCartAsync(IEnumerable<Course> courses, User user)
    {
        var summary = new CartSummary
        {
            Courses = new List<CourseDiscountInfo>()
        };

        foreach (var course in courses)
        {
            var discountInfo = CalculateCourseDiscount(course, user);
            summary.Courses.Add(discountInfo);
            summary.TotalOriginalPrice += course.Price;
            summary.TotalFinalPrice += discountInfo.FinalPrice;
        }

        summary.TotalSaved = summary.TotalOriginalPrice - summary.TotalFinalPrice;
        return await Task.FromResult(summary);
    }

    private CourseDiscountInfo CalculateCourseDiscount(Course course, User user)
    {
        var burningDiscount = IsBurningGroup(course) ? BurningGroupDiscount : 0m;
        var graduateDiscount = user.IsGraduate ? GraduateDiscount : 0m;

        // Скидки не суммируются — выбираем максимальную
        var bestDiscount = Math.Max(burningDiscount, graduateDiscount);
        var finalPrice = course.Price * (1m - bestDiscount / 100m);

        var reason = bestDiscount == 0
            ? "Нет скидок"
            : bestDiscount == GraduateDiscount && user.IsGraduate
                ? "Выпускник"
                : "Горящая группа";

        return new CourseDiscountInfo
        {
            CourseId = course.Id,
            CourseName = course.Name,
            OriginalPrice = course.Price,
            DiscountPercent = bestDiscount,
            FinalPrice = Math.Round(finalPrice, 2),
            DiscountReason = reason
        };
    }

    private bool IsBurningGroup(Course course)
    {
        var daysUntilStart = (course.StartDate - DateTime.UtcNow).Days;
        return daysUntilStart < BurningGroupDaysThreshold && daysUntilStart >= 0;
    }
}
