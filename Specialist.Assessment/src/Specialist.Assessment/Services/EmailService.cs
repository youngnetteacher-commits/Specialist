namespace Specialist.Assessment.Services;

using Specialist.Assessment.Models;

public interface IEmailService
{
    Task SendAsync(string fromEmail, string toEmail, string subject, string body);
}

/// <summary>
/// Имитация основного почтового сервиса, который периодически выходит из строя.
/// </summary>
public class PrimaryEmailService : IEmailService
{
    private static bool _isDown = true; // По умолчанию сервис "сломан" для демонстрации

    public static void SetServiceStatus(bool isUp) => _isDown = !isUp;

    public async Task SendAsync(string fromEmail, string toEmail, string subject, string body)
    {
        // Имитация задержки сети
        await Task.Delay(100);

        if (_isDown)
        {
            throw new CustomStrangeException(
                "Основной почтовый сервис недоступен. Перенаправление на резервный адрес.");
        }

        // Имитация отправки письма
        await Task.CompletedTask;
    }
}
