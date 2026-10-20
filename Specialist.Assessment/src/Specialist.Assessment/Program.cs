using FluentValidation;
using Specialist.Assessment.Controllers;
using Specialist.Assessment.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// DI — сервисы
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IEmailService, PrimaryEmailService>();
builder.Services.AddScoped<ILeadService, LeadService>();

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateLeadDtoValidator).Assembly);

var app = builder.Build();

// Middleware pipeline
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
