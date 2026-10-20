using Microsoft.AspNetCore.Mvc;
using Specialist.Assessment.Models;
using Specialist.Assessment.Services;

namespace Specialist.Assessment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IDiscountService _discountService;
    private readonly ILogger<CoursesController> _logger;

    // InMemory-хранилище курсов (заглушка вместо СУБД)
    private static readonly List<Course> _courses = new()
    {
        new() { Id = 1, Name = "C# Developer", Price = 80000m, StartDate = DateTime.UtcNow.AddDays(2) },
        new() { Id = 2, Name = "Python Advanced", Price = 65000m, StartDate = DateTime.UtcNow.AddDays(30) },
        new() { Id = 3, Name = "DevOps Pro", Price = 95000m, StartDate = DateTime.UtcNow.AddDays(1) },
        new() { Id = 4, Name = "Data Science", Price = 75000m, StartDate = DateTime.UtcNow.AddDays(60) }
    };

    public CoursesController(IDiscountService discountService, ILogger<CoursesController> logger)
    {
        _discountService = discountService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_courses);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var course = _courses.FirstOrDefault(c => c.Id == id);
        if (course == null)
            return NotFound(new { message = $"Курс с ID {id} не найден" });

        return Ok(course);
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateCart([FromBody] CartRequest request)
    {
        var user = new User { Id = request.UserId, IsGraduate = request.IsGraduate };

        var selectedCourses = _courses.Where(c => request.CourseIds.Contains(c.Id)).ToList();

        if (!selectedCourses.Any())
            return BadRequest(new { message = "Курсы не найдены" });

        var summary = await _discountService.CalculateCartAsync(selectedCourses, user);
        return Ok(summary);
    }
}

public class CartRequest
{
    public int UserId { get; set; }
    public bool IsGraduate { get; set; }
    public List<int> CourseIds { get; set; } = new();
}
