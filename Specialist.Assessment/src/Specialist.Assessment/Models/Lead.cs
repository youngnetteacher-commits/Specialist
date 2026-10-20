using System.ComponentModel.DataAnnotations;

namespace Specialist.Assessment.Models;

public enum ClientType
{
    Individual,   // Физическое лицо
    Legal         // Юридическое лицо
}

public class CreateLeadDto
{
    [Required(ErrorMessage = "Имя обязательно для заполнения")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 100 символов")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон обязателен для заполнения")]
    [Phone(ErrorMessage = "Введите корректный номер телефона")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен для заполнения")]
    [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты")]
    public string Email { get; set; } = string.Empty;

    public ClientType ClientType { get; set; }
}

public class LeadResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? FallbackEmail { get; set; }
}
