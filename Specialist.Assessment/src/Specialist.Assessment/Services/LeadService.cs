using Specialist.Assessment.Models;

namespace Specialist.Assessment.Services;

public interface ILeadService
{
    Task<LeadResponse> SubmitLeadAsync(CreateLeadDto dto);
}

public class LeadService : ILeadService
{
    private const string LegalFallbackEmail = "specialist.corp@yandex.ru";
    private const string IndividualFallbackEmail = "specialist.ufl@yandex.ru";

    private readonly IEmailService _primaryEmailService;

    public LeadService(IEmailService primaryEmailService)
    {
        _primaryEmailService = primaryEmailService;
    }

    public async Task<LeadResponse> SubmitLeadAsync(CreateLeadDto dto)
    {
        var fallbackEmail = dto.ClientType == ClientType.Legal
                ? LegalFallbackEmail
                : IndividualFallbackEmail;


        var subject = $"Заявка обратного звонка — {dto.ClientType}";
        var body = BuildEmailBody(dto);

        // Пытаемся отправить через основной сервис
        try
        {
            await _primaryEmailService.SendAsync(fallbackEmail, dto.Email, subject, body);
            return new LeadResponse
            {
                Success = true,
                Message = "Заявка успешно отправлена!"
            };
        }
        catch (CustomStrangeException ex)
        {
            // Основной сервис недоступен — перенаправляем на резервный
             fallbackEmail = dto.ClientType != ClientType.Legal
                ? LegalFallbackEmail
                : IndividualFallbackEmail;

            // В реальном проекте здесь был бы вызов резервного сервиса
            // Для демонстрации — просто фиксируем факт
            await LogFallbackAsync(fallbackEmail, dto, ex);

            return new LeadResponse
            {
                Success = true,
                Message = "Заявка принята!",
                FallbackEmail = fallbackEmail
            };
        }
    }

    private static string BuildEmailBody(CreateLeadDto dto)
    {
        return $@"Имя: {dto.Name}
Телефон: {dto.Phone}
Email: {dto.Email}
Тип клиента: {dto.ClientType}";
    }

    private Task LogFallbackAsync(string fallbackEmail, CreateLeadDto dto, Exception ex)
    {
        // В реальном проекте — запись в лог/базу
        System.Diagnostics.Debug.WriteLine(
            $"[FALLBACK] Lead for {dto.Email} redirected to {fallbackEmail}. Reason: {ex.Message}");
        return Task.CompletedTask;
    }
}
