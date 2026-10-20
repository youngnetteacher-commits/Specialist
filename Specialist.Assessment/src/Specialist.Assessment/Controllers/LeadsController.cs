using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Specialist.Assessment.Models;
using Specialist.Assessment.Services;

namespace Specialist.Assessment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly ILeadService _leadService;
    private readonly ILogger<LeadsController> _logger;

    public LeadsController(ILeadService leadService, ILogger<LeadsController> logger)
    {
        _leadService = leadService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] CreateLeadDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }

        try
        {
            var result = await _leadService.SubmitLeadAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while processing lead");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
        }
    }
}

public class CreateLeadDtoValidator : AbstractValidator<CreateLeadDto>
{
    public CreateLeadDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя обязательно для заполнения")
            .MinimumLength(2).WithMessage("Имя должно содержать минимум 2 символа");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Телефон обязателен для заполнения")
            .Matches(@"^[\+]?[\d\s\-\(\)]{7,20}$").WithMessage("Введите корректный номер телефона");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен для заполнения")
            .EmailAddress().WithMessage("Введите корректный адрес электронной почты");
    }
}
