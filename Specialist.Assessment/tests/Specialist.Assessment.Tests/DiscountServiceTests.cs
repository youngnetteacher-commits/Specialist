using Specialist.Assessment.Models;
using Specialist.Assessment.Services;

namespace Specialist.Assessment.Tests;

public class DiscountServiceTests
{
    [Fact]
    public async Task CalculateCart_BurningGroup_Returns20PercentDiscount()
    {
        // Arrange
        var service = new DiscountService();
        var courses = new List<Course>
        {
            new() { Id = 1, Name = "Fast Course", Price = 10000m, StartDate = DateTime.UtcNow.AddDays(3) }
        };
        var user = new User { Id = 1, IsGraduate = false };

        // Act
        var result = await service.CalculateCartAsync(courses, user);

        // Assert
        var courseInfo = result.Courses.First();
        Assert.Equal(20m, courseInfo.DiscountPercent);
        Assert.Equal("Горящая группа", courseInfo.DiscountReason);
        Assert.Equal(8000m, courseInfo.FinalPrice);
        Assert.Equal(2000m, result.TotalSaved);
    }

    [Fact]
    public async Task CalculateGraduateUser_Returns30PercentDiscount()
    {
        // Arrange
        var service = new DiscountService();
        var courses = new List<Course>
        {
            new() { Id = 1, Name = "Regular Course", Price = 10000m, StartDate = DateTime.UtcNow.AddDays(60) }
        };
        var user = new User { Id = 1, IsGraduate = true };

        // Act
        var result = await service.CalculateCartAsync(courses, user);

        // Assert
        var courseInfo = result.Courses.First();
        Assert.Equal(30m, courseInfo.DiscountPercent);
        Assert.Equal("Выпускник", courseInfo.DiscountReason);
        Assert.Equal(7000m, courseInfo.FinalPrice);
    }

    [Fact]
    public async Task CalculateBothDiscountsAvailable_ReturnsMaxOnly()
    {
        // Arrange: курс горящий + пользователь выпускник → max(20%, 30%) = 30%
        var service = new DiscountService();
        var courses = new List<Course>
        {
            new() { Id = 1, Name = "Burning + Graduate", Price = 10000m, StartDate = DateTime.UtcNow.AddDays(2) }
        };
        var user = new User { Id = 1, IsGraduate = true };

        // Act
        var result = await service.CalculateCartAsync(courses, user);

        // Assert
        var courseInfo = result.Courses.First();
        Assert.Equal(30m, courseInfo.DiscountPercent); // Только максимальная
        Assert.Equal("Выпускник", courseInfo.DiscountReason);
        Assert.Equal(7000m, courseInfo.FinalPrice);
    }

    [Fact]
    public async Task CalculateNoDiscounts_ReturnsOriginalPrice()
    {
        // Arrange
        var service = new DiscountService();
        var courses = new List<Course>
        {
            new() { Id = 1, Name = "Normal Course", Price = 5000m, StartDate = DateTime.UtcNow.AddDays(90) }
        };
        var user = new User { Id = 1, IsGraduate = false };

        // Act
        var result = await service.CalculateCartAsync(courses, user);

        // Assert
        var courseInfo = result.Courses.First();
        Assert.Equal(0m, courseInfo.DiscountPercent);
        Assert.Equal(5000m, courseInfo.FinalPrice);
        Assert.Equal(0m, result.TotalSaved);
    }

    [Fact]
    public async Task CalculateMultipleCourses_AppliesCorrectDiscountPerCourse()
    {
        // Arrange
        var service = new DiscountService();
        var courses = new List<Course>
        {
            new() { Id = 1, Name = "Burning", Price = 10000m, StartDate = DateTime.UtcNow.AddDays(1) },
            new() { Id = 2, Name = "Normal", Price = 8000m, StartDate = DateTime.UtcNow.AddDays(60) }
        };
        var user = new User { Id = 1, IsGraduate = true };

        // Act
        var result = await service.CalculateCartAsync(courses, user);

        // Assert
        Assert.Equal(2, result.Courses.Count);
        // Для каждого курса max(20% burning, 30% graduate) = 30% graduate
        Assert.Equal(30m, result.Courses[0].DiscountPercent);
        Assert.Equal(30m, result.Courses[1].DiscountPercent);

        Assert.Equal(18000m, result.TotalOriginalPrice);
        Assert.Equal(12600m, result.TotalFinalPrice); // 7000 + 5600
        Assert.Equal(5400m, result.TotalSaved);
    }
}
