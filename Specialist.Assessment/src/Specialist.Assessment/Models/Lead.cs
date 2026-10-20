namespace Specialist.Assessment.Models;

public enum ClientType
{
    Individual,   // Физическое лицо
    Legal         // Юридическое лицо
}

public class CreateLeadDto
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ClientType ClientType { get; set; }
}

public class LeadResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? FallbackEmail { get; set; }
}
